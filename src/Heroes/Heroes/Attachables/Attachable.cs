namespace Heroes.Attachables;

public class Attachable : Common {

    // Parent
    internal Hero? Parent;

    // Constructor
    public Attachable() {
        // Run initialisation code
        Initialisation();
    }

    public virtual void Initialisation() {}
    public virtual void Update() {}
    public virtual void Destroy() {}
}