using Heroes.Internal;

// The framework looks for GameCode.Loader.HeroesFSLoader.Load() to load the gamecode
// This code should be in a file called gamecode/Loader/HeroesFSLoader.cs
// It is responsible for creating the heroes present when the game first starts up

namespace GameCode.Loader {
    public class HeroesFSLoader {
        public static void Load() {

            // Create a new hero (this method should only be used by the loader script to create the heroes, use Hero.Create<T>() in game code)
            HeroManager.WorldLoadHero<AzzyShell>();
        }
    }
}