# Azzy Shell Script Language Specification 3.0

This document defines the Azzy Shell Script (ASS) language specification for version 3.0. It contains breaking changes, bug fixes, and formalises existing behaviour.

**Note:** V3 spec requires a complete rewrite of the shell and requires implementing a multi-stage scripting language like any other shell. 

---

# Variables

## Types

Azzy Shell supports the following variable types:

* `Int` — Integer values
* `Double` — Floating-point values
* `Bool` — Boolean values (`true` / `false`)
* `String` — Text values
* `Null` — A defined variable without an assigned value

---

## Assignment

Variables are created and modified using the `set` command.

### Syntax

```sh
set <variable> [value]
```

### Examples

```sh
set age 23
set username "Az"
set empty
```

A variable created without a value is assigned the `Null` type.

---

## Viewing Variables

All currently defined variables can be displayed using:

```sh
vars
```

---

## Variable Referencing

Variables can be referenced using different brackets depending on the desired output:

| Syntax       | Description                  |
| ------------ | ---------------------------- |
| `{variable}` | Returns the variable's value |
| `[variable]` | Returns the variable's type  |
| `(variable)` | Returns the variable's name  |
| `<variable>` | Returns the variable's index |

Example:

```sh
set name "Az"

log {name}
log [name]
log (name)
log <name>
```

---

# Strings and Escape Characters

Previous escape sequences are removed and replaced with C-style escape sequences.

## Special Characters

| Escape | Description           |
| ------ | --------------------- |
| `\n`   | Newline               |
| `\0`   | Null terminator / EOF |
| `\t`   | Tab                   |

Strings may be enclosed using either single or double quotes:

```sh
log "Hello World"
log 'Hello World'
```

Quotes can be escaped when required:

```sh
log "He said \"Hello\""
```

---

# Comments

Comments begin with `#` and continue until the end of the current line.

Example:

```sh
# This is a comment
log "Hello" # This is also a comment
```

---

# Command Operators

Commands can be connected, separated, and redirected using command operators.

---

## Sequential Execution

`;` separates commands that are always executed sequentially.

Example:

```sh
command1 ; command2
```

`command2` will execute regardless of the result of `command1`.

---

## Conditional Execution

`&&` separates commands where the next command only executes if the previous command succeeds.

Example:

```sh
build && run
```

If `build` returns a non-success error code, `run` will not execute.

---

## Logical OR Execution

`||` separates commands where the next command only executes if the previous command fails.

Example:

```sh
build || recover
```

If `build` succeeds, `recover` will not execute.

---

## Pipe Execution

`|` sends the standard output of one command as the standard input of another command.

Example:

```sh
list | filter "png"
```

The output from `list` becomes the input of `filter`.

Pipes may be chained:

```sh
list | filter "png" | count
```

Each command receives the output from the previous command.

---

# Output and Input Redirection

Commands may redirect their input and output streams.

Azzy Shell supports three standard streams:

| Stream | Name   | Description     |
| ------ | ------ | --------------- |
| `0`    | stdin  | Standard input  |
| `1`    | stdout | Standard output |
| `2`    | stderr | Standard error  |

---

## Output Redirection

`>` redirects standard output to a file.

Example:

```sh
list > files.txt
```

The output of `list` is written to `files.txt`.

If the file exists, it is replaced.

---

## Append Output Redirection

`>>` redirects standard output to a file while preserving existing contents.

Example:

```sh
log "New entry" >> log.txt
```

The output is appended to the end of `log.txt`.

---

## Stream-Specific Redirection

A stream can be selected by placing its number before the redirect operator.

### Standard Output

Equivalent to `1>`:

```sh
command 1> output.txt
```

### Standard Error

Redirect only errors:

```sh
command 2> errors.txt
```

Example:

```sh
build 2> errors.log
```

Normal output remains visible, while errors are written to `errors.log`.

---

## Redirecting Multiple Streams

Standard output and standard error can be redirected separately:

```sh
command 1> output.txt 2> errors.txt
```

Both streams are written to different files.

---

## Redirecting All Output

`&>` redirects both standard output and standard error.

Example:

```sh
command &> all.log
```

Equivalent to:

```sh
command 1> all.log 2> all.log
```

---

## Redirecting Input

`<` redirects a file into standard input.

Example:

```sh
sort < names.txt
```

The contents of `names.txt` become the input of `sort`.

---

## Combining Pipes and Redirection

Pipes and redirection may be combined.

Example:

```sh
list | filter "txt" > results.txt
```

The filtered output is written to `results.txt`.

Example:

```sh
build 2> errors.log | report
```

Errors are redirected while standard output continues through the pipe.

---

# Comparisons

Comparisons allow values to be evaluated within expressions and conditionals.

## Supported Operators

| Operator | Description           |
| -------- | --------------------- |
| `gt`     | Greater than          |
| `ge`     | Greater than or equal |
| `is`     | Equal to              |
| `lt`     | Less than             |
| `le`     | Less than or equal    |
| `not`    | Not equal to          |

Examples:

```sh
age gt 18
name is "Az"
status not "failed"
```

Comparisons may operate on variable references:

```sh
{age} gt 18
[type] is Int
```

---

# Conditionals

Conditionals allow scripts to execute commands based on comparison results.

Azzy Shell supports two forms:

* Inline (ternary) conditionals
* Block conditionals

---

## Block Conditionals

Block conditionals use:

* `if`
* `else if`
* `else`
* `end`

### Syntax

```sh
if <condition> begin
    <commands>
else if <condition>
    <commands>
else
    <commands>
end
```

### Example

```sh
if {age} gt 23 && [age] is Int begin
    gaytext "Happy Birthday"
else if {age} lt 23
    gaytext "You got younger, impossible!"
else
    gaytext "Same age"
end
```

The first matching condition executes. If no condition evaluates as true, the `else` block is executed.

---

# Logical Operators

Conditions may be combined using logical operators.

| Operator | Description |     |            |
| -------- | ----------- | --- | ---------- |
| `&&`     | Logical AND |     |            |
| `        |             | `   | Logical OR |

Example:

```sh
if {age} gt 18 && {country} is "UK" begin
    log "Allowed"
end
```

---

# Version 3.0 Breaking Changes Summary

* `&&` no longer acts as a simple command separator.
* `;` replaces the previous unconditional command separator behaviour.
* Escape characters have been replaced with C-style escapes.
* Conditionals are introduced.
* Comparison expressions are introduced.
* Variable types now include `Double`, `Bool`, and `Null`.
* String parsing now supports both single and double quotes.
* Command piping and stream redirection are introduced.
