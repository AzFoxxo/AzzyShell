namespace Heroes;

public class Transformation : Attachables.Attachable {
    public Vec3 position = new(0, 0, 0);
    public Vec3 rotation = new(0, 0, 0);
    public Vec3 scale = new(1, 1, 1);
}