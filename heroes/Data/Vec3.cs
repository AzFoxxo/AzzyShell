namespace Heroes;

/// <summary> Represents a 3D vector. </summary>
public struct Vec3 {
    internal double x { get; }
    internal double y { get; }
    internal double z { get; }
    public Vec3(double x, double y, double z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /// <summary> Returns the printable representation. </summary>
    public override string ToString() {
        return $"({x}, {y}, {z})";
    }

    // When printing the struct, print the printable representation
    public static implicit operator string(Vec3 vec) {
        return vec.ToString();
    }

    /// <summary> Convert Vec2 to Vec3 </summary>
    public static implicit operator Vec3(Vec2 value) {
        return new Vec3(value.x, value.y, 0);
    }
    
    /// <summary> Add two vectors together. </summary>
    public static Vec3 operator +(Vec3 left, Vec3 right) {
        return new Vec3(left.x + right.x, left.y + right.y, left.z + right.z);
    }

    /// <summary> Subtract two vectors. </summary>
    public static Vec3 operator -(Vec3 left, Vec3 right) {
        return new Vec3(left.x - right.x, left.y - right.y, left.z - right.z);
    }

    /// <summary> Multiply two vectors. </summary>
    public static Vec3 operator *(Vec3 left, Vec3 right) {
        return new Vec3(left.x * right.x, left.y * right.y, left.z * right.z);
    }

    /// <summary> Divide two vectors. </summary>
    public static Vec3 operator /(Vec3 left, Vec3 right) {
        return new Vec3(left.x / right.x, left.y / right.y, left.z / right.z);
    }

    /// <summary> Normalize a vector. </summary>
    public static Vec3 Normalize(Vec3 value) {
        double length = Math.Sqrt(value.x * value.x + value.y * value.y + value.z * value.z);
        return new Vec3(value.x / length, value.y / length, value.z / length);
    }

    /// <summary> Get the length of a vector. </summary>
    public static double Length(Vec3 value) {
        return Math.Sqrt(value.x * value.x + value.y * value.y + value.z * value.z);
    }

    /// <summary> Get the distance between two vectors. </summary>
    public static double Distance(Vec3 left, Vec3 right) {
        return Length(left - right);
    }

    /// <summary> Get the dot product of two vectors. </summary>
    public static double Dot(Vec3 left, Vec3 right) {
        return left.x * right.x + left.y * right.y + left.z * right.z;
    }

    /// <summary> Get the cross product of two vectors. </summary>
    public static Vec3 Cross(Vec3 left, Vec3 right) {
        return new Vec3(left.y * right.z - left.z * right.y, left.z * right.x - left.x * right.z, left.x * right.y - left.y * right.x);
    }

    /// <summary> Get the angle between two vectors. </summary>
    public static double Angle(Vec3 left, Vec3 right) {
        return Math.Acos(Dot(left, right) / (Length(left) * Length(right)));
    }
}