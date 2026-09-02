namespace AzzyShell;

using System.Globalization;
using System.Text;
using AzzyShell.Commands;

internal enum ScriptTokenKind
{
    Word,
    String,
    Operator,
    NewLine,
    EndOfFile,
}

internal readonly record struct ScriptToken(ScriptTokenKind Kind, string Value);

internal static class ScriptText
{
    private const char LiteralMarker = '\u0001';

    public static string MarkLiteral(string value) => $"{LiteralMarker}{value}{LiteralMarker}";

    public static bool IsLiteral(string value)
    {
        return value.Length >= 2 && value[0] == LiteralMarker && value[^1] == LiteralMarker;
    }

    public static string UnwrapLiteral(string value)
    {
        return IsLiteral(value) ? value[1..^1] : value;
    }
}

internal sealed class ScriptEngine
{
    private readonly Azzy shell;

    public ScriptEngine(Azzy shell)
    {
        this.shell = shell;
    }

    public int Execute(string script)
    {
        try
        {
            var parser = new Parser(ScriptTokenizer.Tokenize(script));
            var program = parser.ParseProgram();
            return ExecuteProgram(program);
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            Console.Error.WriteLine($"[Error] {ex.Message}");
            return (int)ErrorCode.ExecutionFailure;
        }
    }

    public static bool NeedsMoreInput(string script)
    {
        return GetBlockDepth(script) > 0;
    }

    public static int GetBlockDepth(string script)
    {
        int blockDepth = 0;

        foreach (var token in ScriptTokenizer.Tokenize(script))
        {
            if (token.Kind is not ScriptTokenKind.Word)
                continue;

            if (string.Equals(token.Value, "if", StringComparison.OrdinalIgnoreCase))
            {
                blockDepth++;
                continue;
            }

            if (string.Equals(token.Value, "end", StringComparison.OrdinalIgnoreCase) && blockDepth > 0)
            {
                blockDepth--;
            }
        }

        return blockDepth;
    }

    private int ExecuteProgram(IReadOnlyList<ProgramItem> program)
    {
        int lastStatus = 0;

        foreach (var item in program)
        {
            bool shouldExecute = item.SeparatorBefore switch
            {
                SequenceSeparator.And => lastStatus == 0,
                SequenceSeparator.Or => lastStatus != 0,
                _ => true,
            };

            if (!shouldExecute)
                continue;

            lastStatus = ExecuteNode(item.Node);
        }

        return lastStatus;
    }

    private int ExecuteNode(Node node)
    {
        return node switch
        {
            CommandNode command => ExecuteCommandNode(command, null, allowDirect: true).Status,
            PipelineNode pipeline => ExecutePipeline(pipeline),
            ConditionalNode conditional => ExecuteConditional(conditional),
            _ => (int)ErrorCode.UnknownError,
        };
    }

    private int ExecuteConditional(ConditionalNode conditional)
    {
        foreach (var branch in conditional.Branches)
        {
            if (EvaluateCondition(branch.ConditionTokens))
                return ExecuteProgram(branch.Body);
        }

        if (conditional.ElseBody is not null)
            return ExecuteProgram(conditional.ElseBody);

        return (int)ErrorCode.Success;
    }

    private int ExecutePipeline(PipelineNode pipeline)
    {
        string? currentInput = null;
        int lastStatus = (int)ErrorCode.Success;

        for (int index = 0; index < pipeline.Stages.Count; index++)
        {
            bool isLastStage = index == pipeline.Stages.Count - 1;
            var stage = pipeline.Stages[index];
            var result = ExecuteCommandNode(stage, currentInput, allowDirect: false);

            lastStatus = result.Status;
            currentInput = result.StandardOutput;

            if (!string.IsNullOrEmpty(result.StandardError))
                Console.Error.Write(result.StandardError);

            if (isLastStage && !string.IsNullOrEmpty(result.StandardOutput))
                Console.Write(result.StandardOutput);
        }

        return lastStatus;
    }

    private CommandExecutionResult ExecuteCommandNode(CommandNode commandNode, string? standardInput, bool allowDirect)
    {
        string[] arguments = shell.ResolveScriptTokens(commandNode.Arguments.ToArray());
        string? redirectedInput = standardInput;

        foreach (var redirect in commandNode.Redirects)
        {
            if (redirect.Kind != RedirectKind.Input)
                continue;

            redirectedInput = File.Exists(redirect.Target)
                ? File.ReadAllText(redirect.Target)
                : string.Empty;
        }

        bool needsCapture = !allowDirect || commandNode.Redirects.Count > 0 || redirectedInput is not null;

        if (!needsCapture)
        {
            int directStatus = shell.ExecuteCommandArray(arguments);
            return new CommandExecutionResult(directStatus, string.Empty, string.Empty);
        }

        var outputWriter = new StringWriter();
        var errorWriter = new StringWriter();
        var context = new CommandContext(
            new StringReader(redirectedInput ?? string.Empty),
            outputWriter,
            errorWriter);

        int status = shell.ExecuteCommandArray(
            arguments,
            redirectedInput,
            forceCaptureStreams: true,
            context: context);
        string standardOutput = outputWriter.ToString();
        string standardError = errorWriter.ToString();

        foreach (var redirect in commandNode.Redirects)
        {
            switch (redirect.Kind)
            {
                case RedirectKind.Output:
                    File.WriteAllText(redirect.Target, standardOutput);
                    standardOutput = string.Empty;
                    break;
                case RedirectKind.AppendOutput:
                    File.AppendAllText(redirect.Target, standardOutput);
                    standardOutput = string.Empty;
                    break;
                case RedirectKind.Error:
                    File.WriteAllText(redirect.Target, standardError);
                    standardError = string.Empty;
                    break;
                case RedirectKind.All:
                    File.WriteAllText(redirect.Target, standardOutput + standardError);
                    standardOutput = string.Empty;
                    standardError = string.Empty;
                    break;
            }
        }

        return new CommandExecutionResult(status, standardOutput, standardError);
    }

    private bool EvaluateCondition(IReadOnlyList<ScriptToken> tokens)
    {
        var parser = new ConditionParser(tokens, shell);
        return parser.ParseExpression();
    }

    private sealed record CommandExecutionResult(int Status, string StandardOutput, string StandardError);

    private abstract record Node;

    private sealed record CommandNode(IReadOnlyList<string> Arguments, IReadOnlyList<Redirection> Redirects) : Node;

    private sealed record PipelineNode(IReadOnlyList<CommandNode> Stages) : Node;

    private sealed record ConditionalNode(IReadOnlyList<ConditionalBranch> Branches, IReadOnlyList<ProgramItem>? ElseBody) : Node;

    private sealed record ConditionalBranch(IReadOnlyList<ScriptToken> ConditionTokens, IReadOnlyList<ProgramItem> Body);

    private sealed record ProgramItem(SequenceSeparator SeparatorBefore, Node Node);

    private sealed record Redirection(RedirectKind Kind, string Target);

    private enum RedirectKind
    {
        Output,
        AppendOutput,
        Error,
        Input,
        All,
    }

    private enum SequenceSeparator
    {
        None,
        And,
        Or,
    }

    private sealed class Parser
    {
        private readonly IReadOnlyList<ScriptToken> tokens;
        private int position;

        public Parser(IReadOnlyList<ScriptToken> tokens)
        {
            this.tokens = tokens;
        }

        public List<ProgramItem> ParseProgram(HashSet<string>? stopWords = null)
        {
            var items = new List<ProgramItem>();
            SequenceSeparator pendingSeparator = SequenceSeparator.None;

            while (!IsAtEnd())
            {
                SkipBlankLines();

                if (IsAtEnd())
                    break;

                if (stopWords is not null && IsStopWord(Peek(), stopWords))
                    break;

                var node = ParseStatement(stopWords);
                items.Add(new ProgramItem(pendingSeparator, node));
                pendingSeparator = ConsumeSeparators(stopWords);
            }

            return items;
        }

        private Node ParseStatement(HashSet<string>? stopWords)
        {
            if (MatchWord("if"))
                return ParseConditional();

            return ParsePipeline(stopWords);
        }

        private Node ParseConditional()
        {
            var branches = new List<ConditionalBranch>();
            var conditionTokens = ParseConditionTokens();

            ConsumeOptionalWord("begin");
            SkipBlankLines();

            var branchStopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "else",
                "end",
            };

            branches.Add(new ConditionalBranch(conditionTokens, ParseProgram(branchStopWords)));

            while (MatchWord("else"))
            {
                if (MatchWord("if"))
                {
                    var nextConditionTokens = ParseConditionTokens();
                    ConsumeOptionalWord("begin");
                    SkipBlankLines();
                    branches.Add(new ConditionalBranch(nextConditionTokens, ParseProgram(branchStopWords)));
                    continue;
                }

                SkipBlankLines();
                var elseBody = ParseProgram(new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "end" });
                ExpectWord("end");
                return new ConditionalNode(branches, elseBody);
            }

            ExpectWord("end");
            return new ConditionalNode(branches, null);
        }

        private IReadOnlyList<ScriptToken> ParseConditionTokens()
        {
            var conditionTokens = new List<ScriptToken>();

            while (!IsAtEnd())
            {
                var token = Peek();

                if (token.Kind is ScriptTokenKind.NewLine)
                    break;

                if (token.Kind is ScriptTokenKind.Operator && token.Value is ";")
                    break;

                if (token.Kind is ScriptTokenKind.Word && string.Equals(token.Value, "begin", StringComparison.OrdinalIgnoreCase))
                    break;

                if (token.Kind is ScriptTokenKind.Word && string.Equals(token.Value, "else", StringComparison.OrdinalIgnoreCase))
                    break;

                if (token.Kind is ScriptTokenKind.Word && string.Equals(token.Value, "end", StringComparison.OrdinalIgnoreCase))
                    break;

                conditionTokens.Add(Advance());
            }

            return conditionTokens;
        }

        private Node ParsePipeline(HashSet<string>? stopWords)
        {
            var stages = new List<CommandNode> { ParseCommand(stopWords) };

            while (MatchOperator("|"))
            {
                stages.Add(ParseCommand(stopWords));
            }

            return stages.Count == 1 ? stages[0] : new PipelineNode(stages);
        }

        private CommandNode ParseCommand(HashSet<string>? stopWords)
        {
            var arguments = new List<string>();
            var redirects = new List<Redirection>();

            while (!IsAtEnd())
            {
                var token = Peek();

                if (token.Kind is ScriptTokenKind.NewLine)
                    break;

                if (token.Kind is ScriptTokenKind.Operator && (token.Value is ";" or "|" or "&&" or "||"))
                    break;

                if (stopWords is not null && token.Kind is ScriptTokenKind.Word && IsStopWord(token, stopWords))
                    break;

                if (token.Kind is ScriptTokenKind.Operator)
                {
                    redirects.Add(ParseRedirect());
                    continue;
                }

                arguments.Add(Advance().Value);
            }

            if (arguments.Count == 0)
                throw new InvalidOperationException("Expected a command name.");

            return new CommandNode(arguments, redirects);
        }

        private Redirection ParseRedirect()
        {
            var token = Advance();

            if (token.Value is ">")
                return new Redirection(RedirectKind.Output, ParseRedirectTarget());

            if (token.Value is ">>")
                return new Redirection(RedirectKind.AppendOutput, ParseRedirectTarget());

            if (token.Value is "2>")
                return new Redirection(RedirectKind.Error, ParseRedirectTarget());

            if (token.Value is "&>")
                return new Redirection(RedirectKind.All, ParseRedirectTarget());

            if (token.Value is "<")
                return new Redirection(RedirectKind.Input, ParseRedirectTarget());

            if (token.Value.EndsWith('>') && int.TryParse(token.Value[..^1], out _))
                return new Redirection(token.Value.StartsWith('2') ? RedirectKind.Error : RedirectKind.Output, ParseRedirectTarget());

            throw new InvalidOperationException($"Unsupported redirect operator '{token.Value}'.");
        }

        private string ParseRedirectTarget()
        {
            if (IsAtEnd())
                throw new InvalidOperationException("Missing redirect target.");

            var token = Advance();

            if (token.Kind is not (ScriptTokenKind.Word or ScriptTokenKind.String))
                throw new InvalidOperationException("Missing redirect target.");

            return token.Value;
        }

        private SequenceSeparator ConsumeSeparators(HashSet<string>? stopWords)
        {
            SequenceSeparator separator = SequenceSeparator.None;

            while (!IsAtEnd())
            {
                var token = Peek();

                if (stopWords is not null && IsStopWord(token, stopWords))
                    break;

                if (token.Kind is ScriptTokenKind.NewLine)
                {
                    separator = SequenceSeparator.None;
                    Advance();
                    continue;
                }

                if (token.Kind is ScriptTokenKind.Operator && token.Value is ";")
                {
                    separator = SequenceSeparator.None;
                    Advance();
                    continue;
                }

                if (token.Kind is ScriptTokenKind.Operator && token.Value is "&&")
                {
                    separator = SequenceSeparator.And;
                    Advance();
                    continue;
                }

                if (token.Kind is ScriptTokenKind.Operator && token.Value is "||")
                {
                    separator = SequenceSeparator.Or;
                    Advance();
                    continue;
                }

                break;
            }

            return separator;
        }

        private void SkipBlankLines()
        {
            while (!IsAtEnd() && Peek().Kind is ScriptTokenKind.NewLine)
                Advance();
        }

        private bool MatchWord(string word)
        {
            if (IsAtEnd())
                return false;

            var token = Peek();
            if (token.Kind is not ScriptTokenKind.Word || !string.Equals(token.Value, word, StringComparison.OrdinalIgnoreCase))
                return false;

            Advance();
            return true;
        }

        private bool ConsumeOptionalWord(string word) => MatchWord(word);

        private void ExpectWord(string word)
        {
            if (!MatchWord(word))
                throw new InvalidOperationException($"Expected '{word}'.");
        }

        private bool MatchOperator(string value)
        {
            if (IsAtEnd())
                return false;

            var token = Peek();
            if (token.Kind is not ScriptTokenKind.Operator || token.Value != value)
                return false;

            Advance();
            return true;
        }

        private bool IsStopWord(ScriptToken token, HashSet<string> stopWords)
        {
            return token.Kind is ScriptTokenKind.Word && stopWords.Contains(token.Value);
        }

        private ScriptToken Peek() => position < tokens.Count ? tokens[position] : new ScriptToken(ScriptTokenKind.EndOfFile, string.Empty);

        private ScriptToken Advance() => tokens[position++];

        private bool IsAtEnd() => Peek().Kind is ScriptTokenKind.EndOfFile;
    }

    private sealed class ConditionParser
    {
        private readonly IReadOnlyList<ScriptToken> tokens;
        private readonly Azzy shell;
        private int position;

        public ConditionParser(IReadOnlyList<ScriptToken> tokens, Azzy shell)
        {
            this.tokens = tokens;
            this.shell = shell;
        }

        public bool ParseExpression()
        {
            if (tokens.Count == 0)
                return false;

            return ParseOr();
        }

        private bool ParseOr()
        {
            bool result = ParseAnd();

            while (MatchOperator("||"))
                result = result || ParseAnd();

            return result;
        }

        private bool ParseAnd()
        {
            bool result = ParseComparison();

            while (MatchOperator("&&"))
                result = result && ParseComparison();

            return result;
        }

        private bool ParseComparison()
        {
            var left = ParseValue();

            if (MatchWord("gt")) return Compare(left, ParseValue(), ComparisonKind.GreaterThan);
            if (MatchWord("ge")) return Compare(left, ParseValue(), ComparisonKind.GreaterThanOrEqual);
            if (MatchWord("is")) return Compare(left, ParseValue(), ComparisonKind.Equal);
            if (MatchWord("lt")) return Compare(left, ParseValue(), ComparisonKind.LessThan);
            if (MatchWord("le")) return Compare(left, ParseValue(), ComparisonKind.LessThanOrEqual);
            if (MatchWord("not")) return Compare(left, ParseValue(), ComparisonKind.NotEqual);

            return IsTruthy(left);
        }

        private object? ParseValue()
        {
            if (IsAtEnd())
                return null;

            var token = Advance();
            string resolved = shell.ResolveScriptTokens([token.Value])[0];

            if (string.Equals(resolved, "null", StringComparison.OrdinalIgnoreCase))
                return null;

            if (bool.TryParse(resolved, out bool booleanValue))
                return booleanValue;

            if (int.TryParse(resolved, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intValue))
                return intValue;

            if (double.TryParse(resolved, NumberStyles.Float, CultureInfo.InvariantCulture, out double doubleValue))
                return doubleValue;

            return resolved;
        }

        private bool Compare(object? left, object? right, ComparisonKind kind)
        {
            if (left is null || right is null)
            {
                return kind switch
                {
                    ComparisonKind.Equal => left is null && right is null,
                    ComparisonKind.NotEqual => left is null != (right is null),
                    _ => false,
                };
            }

            if (left is bool leftBool && right is bool rightBool)
            {
                return kind switch
                {
                    ComparisonKind.Equal => leftBool == rightBool,
                    ComparisonKind.NotEqual => leftBool != rightBool,
                    _ => CompareStrings(leftBool.ToString(), rightBool.ToString(), kind),
                };
            }

            if (TryConvertDouble(left, out double leftDouble) && TryConvertDouble(right, out double rightDouble))
            {
                return kind switch
                {
                    ComparisonKind.GreaterThan => leftDouble > rightDouble,
                    ComparisonKind.GreaterThanOrEqual => leftDouble >= rightDouble,
                    ComparisonKind.LessThan => leftDouble < rightDouble,
                    ComparisonKind.LessThanOrEqual => leftDouble <= rightDouble,
                    ComparisonKind.Equal => Math.Abs(leftDouble - rightDouble) < double.Epsilon,
                    ComparisonKind.NotEqual => Math.Abs(leftDouble - rightDouble) >= double.Epsilon,
                    _ => false,
                };
            }

            return CompareStrings(left.ToString() ?? string.Empty, right.ToString() ?? string.Empty, kind);
        }

        private static bool CompareStrings(string left, string right, ComparisonKind kind)
        {
            int comparison = string.Compare(left, right, StringComparison.Ordinal);

            return kind switch
            {
                ComparisonKind.GreaterThan => comparison > 0,
                ComparisonKind.GreaterThanOrEqual => comparison >= 0,
                ComparisonKind.LessThan => comparison < 0,
                ComparisonKind.LessThanOrEqual => comparison <= 0,
                ComparisonKind.Equal => comparison == 0,
                ComparisonKind.NotEqual => comparison != 0,
                _ => false,
            };
        }

        private static bool TryConvertDouble(object value, out double result)
        {
            switch (value)
            {
                case int integerValue:
                    result = integerValue;
                    return true;
                case double doubleValue:
                    result = doubleValue;
                    return true;
                case string stringValue:
                    return double.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
                default:
                    result = default;
                    return false;
            }
        }

        private static bool IsTruthy(object? value)
        {
            return value switch
            {
                null => false,
                bool booleanValue => booleanValue,
                int integerValue => integerValue != 0,
                double doubleValue => Math.Abs(doubleValue) > double.Epsilon,
                string stringValue => !string.IsNullOrWhiteSpace(stringValue) && !string.Equals(stringValue, "false", StringComparison.OrdinalIgnoreCase),
                _ => true,
            };
        }

        private bool MatchWord(string word)
        {
            if (IsAtEnd())
                return false;

            var token = Peek();
            if (token.Kind is not ScriptTokenKind.Word || !string.Equals(token.Value, word, StringComparison.OrdinalIgnoreCase))
                return false;

            Advance();
            return true;
        }

        private bool MatchOperator(string value)
        {
            if (IsAtEnd())
                return false;

            var token = Peek();
            if (token.Kind is not ScriptTokenKind.Operator || token.Value != value)
                return false;

            Advance();
            return true;
        }

        private ScriptToken Peek() => position < tokens.Count ? tokens[position] : new ScriptToken(ScriptTokenKind.EndOfFile, string.Empty);

        private ScriptToken Advance() => tokens[position++];

        private bool IsAtEnd() => Peek().Kind is ScriptTokenKind.EndOfFile;

        private enum ComparisonKind
        {
            GreaterThan,
            GreaterThanOrEqual,
            Equal,
            LessThan,
            LessThanOrEqual,
            NotEqual,
        }
    }
}

internal static class ScriptTokenizer
{
    public static IReadOnlyList<ScriptToken> Tokenize(string input)
    {
        var tokens = new List<ScriptToken>();
        int index = 0;

        while (index < input.Length)
        {
            char current = input[index];

            if (current == '\r')
            {
                index++;
                continue;
            }

            if (current == '\n')
            {
                tokens.Add(new ScriptToken(ScriptTokenKind.NewLine, "\n"));
                index++;
                continue;
            }

            if (char.IsWhiteSpace(current))
            {
                index++;
                continue;
            }

            if (current == '#')
            {
                while (index < input.Length && input[index] != '\n')
                    index++;

                continue;
            }

            if (current is '\'' or '"')
            {
                tokens.Add(ReadStringToken(input, ref index));
                continue;
            }

            if (TryReadOperator(input, ref index, out var operatorToken))
            {
                tokens.Add(operatorToken);
                continue;
            }

            tokens.Add(ReadWordToken(input, ref index));
        }

        tokens.Add(new ScriptToken(ScriptTokenKind.EndOfFile, string.Empty));
        return tokens;
    }

    private static ScriptToken ReadWordToken(string input, ref int index)
    {
        int start = index;

        while (index < input.Length)
        {
            char current = input[index];

            if (char.IsWhiteSpace(current) || current is '#' or '"' or '\'' or ';' or '|' or '&' or '>' or '<')
                break;

            index++;
        }

        return new ScriptToken(ScriptTokenKind.Word, input[start..index]);
    }

    private static ScriptToken ReadStringToken(string input, ref int index)
    {
        char quote = input[index++];
        var builder = new StringBuilder();
        bool escaping = false;

        while (index < input.Length)
        {
            char current = input[index++];

            if (escaping)
            {
                builder.Append(current switch
                {
                    'n' => '\n',
                    't' => '\t',
                    '0' => '\0',
                    '\\' => '\\',
                    '"' => '"',
                    '\'' => '\'',
                    _ => current,
                });
                escaping = false;
                continue;
            }

            if (current == '\\')
            {
                escaping = true;
                continue;
            }

            if (current == quote)
                return new ScriptToken(ScriptTokenKind.String, ScriptText.MarkLiteral(builder.ToString()));

            builder.Append(current);
        }

        return new ScriptToken(ScriptTokenKind.String, ScriptText.MarkLiteral(builder.ToString()));
    }

    private static bool TryReadOperator(string input, ref int index, out ScriptToken token)
    {
        token = new ScriptToken(ScriptTokenKind.Operator, string.Empty);

        if (index >= input.Length)
            return false;

        char current = input[index];

        if (char.IsDigit(current))
        {
            int start = index;

            while (index < input.Length && char.IsDigit(input[index]))
                index++;

            if (index < input.Length && input[index] == '>')
            {
                index++;
                token = new ScriptToken(ScriptTokenKind.Operator, input[start..index]);
                return true;
            }

            index = start;
        }

        if (current == '&')
        {
            if (index + 1 < input.Length && input[index + 1] == '>')
            {
                index += 2;
                token = new ScriptToken(ScriptTokenKind.Operator, "&>");
                return true;
            }

            if (index + 1 < input.Length && input[index + 1] == '&')
            {
                index += 2;
                token = new ScriptToken(ScriptTokenKind.Operator, "&&");
                return true;
            }
        }

        if (current == '|')
        {
            if (index + 1 < input.Length && input[index + 1] == '|')
            {
                index += 2;
                token = new ScriptToken(ScriptTokenKind.Operator, "||");
                return true;
            }

            index++;
            token = new ScriptToken(ScriptTokenKind.Operator, "|");
            return true;
        }

        if (current == '>')
        {
            if (index + 1 < input.Length && input[index + 1] == '>')
            {
                index += 2;
                token = new ScriptToken(ScriptTokenKind.Operator, ">>");
                return true;
            }

            index++;
            token = new ScriptToken(ScriptTokenKind.Operator, ">");
            return true;
        }

        if (current == '<')
        {
            index++;
            token = new ScriptToken(ScriptTokenKind.Operator, "<");
            return true;
        }

        if (current == ';')
        {
            index++;
            token = new ScriptToken(ScriptTokenKind.Operator, ";");
            return true;
        }

        return false;
    }
}