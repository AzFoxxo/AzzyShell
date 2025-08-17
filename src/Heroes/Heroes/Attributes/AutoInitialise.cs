namespace Heroes;

/// <summary> Automatically initialise a hero on engine start. </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AutoInitialiseAttribute : Attribute {}