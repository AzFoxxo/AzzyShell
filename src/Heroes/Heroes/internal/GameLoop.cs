using Heroes.Debug.Paws;
namespace Heroes.Internal;

public class GameLoop
{
    public static bool running = true;

    // Game loop methods
    public void Enter()
    {
        // Loader code
        HeroManager.WorldLoadHero<EngineHeroInitialisation>();

        // Create a copy of the list of heroes
        List<Hero> heroes = new(HeroManager.heroes);

        // Update the game loop
        while (running)
        {
            // Calculate the delta time
            Time.CalculateDeltaTime();

            // If the only hero is the persistent hero manager, then destroy it and exit the game loop
            if (heroes.Count == 1 && heroes[0].GetType() == typeof(EngineHeroInitialisation))
            {
                Hero.Destroy(heroes[0]);
                PawsLogger.Error("Aborting! At least one hero must be present!");
                break;
            }
            
            // Run early update methods
            foreach (Hero hero in heroes)
            {
                hero.OnEarlyUpdate();

                if (!running) break; // Quit

                // Rebuild the list of heroes
                if (HeroManager.RebuildList)
                {
                    // Rebuild the copied list of heroes
                    heroes = new(HeroManager.heroes);

                    // Reset the rebuild list
                    HeroManager.RebuildList = false;
                }
            }

            // Run update methods
            foreach (Hero hero in heroes)
            {
                hero.OnUpdate();

                // Update all the attachables
                var attachables = hero.GetAttachables(); // Get the list of attachables
                if (attachables != null) // Check if the list is not null, if not, loop through running the update method
                {
                    foreach (var attachable in attachables)
                    {
                        attachable.Update();
                    }
                }

                if (!running) break; // Quit

                // Rebuild the list of heroes
                if (HeroManager.RebuildList)
                {
                    // Rebuild the copied list of heroes
                    heroes = new(HeroManager.heroes);

                    // Reset the rebuild list
                    HeroManager.RebuildList = false;
                }
            }

            // Run late update methods
            foreach (Hero hero in heroes)
            {
                hero.OnLateUpdate();

                if (!running) break; // Quit

                // Rebuild the list of heroes
                if (HeroManager.RebuildList)
                {
                    // Rebuild the copied list of heroes
                    heroes = new(HeroManager.heroes);

                    // Reset the rebuild list
                    HeroManager.RebuildList = false;
                }
            }
        }

        // Invoke the destroy method for each hero
        foreach (Hero hero in HeroManager.heroes)
        {
            hero.OnDestroy();
        }
    }
    ///<summary>Restart the game loop</summary>
    public void Restart()
    {
        // Reset the game loop
        running = true;

        // Destroy all heroes
        HeroManager.WorldDestroyAllHeroes();

        // Restart the game loop
        Enter();
    }
}
