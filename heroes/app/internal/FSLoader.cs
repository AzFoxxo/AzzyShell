namespace App.Internal;
using Heroes.Internal;

// Game loader script, do not modify this file, instead modify the persistent hero manager
// PersistentHero should be used instead as that is a hero that is always loaded not a static class
public static class FSLoader
{
    public static void Load()
    {
        // Load the persistent hero manager
        HeroManager.WorldLoadHero<PersistentHeroManager>();
    }
}
