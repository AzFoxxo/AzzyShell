namespace Heroes;

// Hero object class (base class)
public class Hero : Attachables.HeroAttachableSystem
{ // Inherits from Common so print method is available, etc.
  // Properties
    private bool persistent = false; // Whether the hero is persistent (should not be destroyed when the world is destroyed)

    // Hero object constructor
    protected Hero()
    {
        // Hero early start method
        OnEarlyStart();

        // Hero start method
        OnStart();

        // Set the parent to the hero
        Parent = this;
    }

    // Events
    public virtual void OnEarlyStart() { }
    public virtual void OnStart() { }
    public virtual void OnEarlyUpdate() { }
    public virtual void OnUpdate() { }
    public virtual void OnLateUpdate() { }
    public virtual void OnDestroy() { }

    // Methods
    ///<summary>Create a hero</summary>
    ///<param name="hero">The hero to create</param>
    ///<returns>The created hero</returns>
    protected static Hero Create<T>() where T : Hero, new()
    {
        // Create a new hero
        Hero hero = new T();

        // Add the hero to the list of heroes
        Heroes.Internal.HeroManager.heroes.Add(hero);

        // Rebuild the list of heroes
        Heroes.Internal.HeroManager.MarkHeroesForRebuild();

        // Return the hero
        return hero;
    }

    ///<summary>Create a hero</summary>
    ///<param name="hero">The hero to create (object reference)</param>
    ///<returns>The created hero</returns>
    protected static Hero Create(Type hero)
    {
        // Make sure the type is derived from Hero
        if (!hero.IsSubclassOf(typeof(Hero))) throw new Exception("Type is not derived from Hero");

        // Create a new hero
        Hero newHero = (Hero)Activator.CreateInstance(hero)!;

        // Add the hero to the list of heroes
        Heroes.Internal.HeroManager.heroes.Add(newHero);

        // Rebuild the list of heroes
        Heroes.Internal.HeroManager.MarkHeroesForRebuild();

        // Return the hero
        return newHero;
    }

    ///<summary>Destroy a hero</summary>
    ///<param name="hero">The hero to destroy</param>
    public static void Destroy(Hero? hero)
    {
        // Make sure the hero is not null in order to prevent errors when destroying a hero which has already been destroyed
        if (hero == null) return;

        // Run de-initialisation methods
        hero.OnDestroy();

        // Remove the hero from the list of heroes
        Heroes.Internal.HeroManager.heroes.Remove(hero);

        // Rebuild the list of heroes
        Heroes.Internal.HeroManager.MarkHeroesForRebuild();
    }

    ///<summary>Set the hero to persistent</summary>
    ///<param name="hero">The hero to set to persistent</param>
    ///<param name="persistent">Whether the hero should be persistent</param>
    public static void MakePersistent(Hero hero, bool persistent) => hero.persistent = true;


    ///<summary>Get the persistence of the hero.</summary>
    ///<param name="hero">The class which derives from hero to get the persistence of</param>
    ///<returns>Whether the hero is persistent</returns>
    public static bool IsPersistent(Hero hero) => hero.persistent;
    
    /// <summary> Returns the printable representation. </summary>
    public override string ToString() => GetType().Name;

    // When printing the hero, print the printable representation
    public static implicit operator string(Hero hero) => hero.ToString();
}