namespace App;
using Heroes;

// The persistent hero manager
// Any global variables can be stored here
public class PersistentHeroManager : Hero
{
    #region Global Variables
    public static PersistentHeroManager? Instance;
    #endregion

    #region App persistent hero manager early start (OnEarlyStart)
    public override void OnEarlyStart()
    {
        // Make persistent hero manager persistent
        MakePersistent(this, true);

        // Set the static reference to the persistent hero manager
        Instance = this;
    }
    #endregion

    #region App persistent hero manager start (OnStart) - Create heroes present at start here
    public override void OnStart()
    {
        // Create heroes here
        Create<AzzyShell>();
    }
    #endregion
}