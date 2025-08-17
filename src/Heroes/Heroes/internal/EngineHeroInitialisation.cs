namespace Heroes.Internal;

using System.Reflection;
using Heroes;

// The persistent hero manager
internal class EngineHeroInitialisation : Hero
{
    internal static EngineHeroInitialisation? Instance;

    private static List<Hero> heroes = new();
    
    override public void OnEarlyStart()
    {
        // Set the instance
        Instance = this;

        // Find all the heroes which have the InitialiseOnEngineStart attribute and initialise them on engine start
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            // Check if the type is a hero
            if (!type.IsSubclassOf(typeof(Hero))) continue;

            // Check if the hero has the InitialiseOnEngineStart attribute
            if (type.GetCustomAttribute<AutoInitialiseAttribute>() == null) continue;

            // Create the hero
            Create(type);
        }
    }

    internal static void AddHero(Hero hero)
    {
        // Add the hero to the list of heroes
        heroes.Add(hero);
    }
}
