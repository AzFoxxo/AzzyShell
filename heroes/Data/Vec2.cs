namespace Heroes;

/// <summary> Represents a 2D vector. </summary>
public struct Vec2 {
    internal double x { get; }
    internal double y { get; }
    public Vec2(double x, double y) {
        this.x = x;
        this.y = y;
    }

    /// <summary> Returns the printable representation. </summary>
    public override string ToString() {
        return $"({x}, {y})";
    }

    // When printing the struct, print the printable representation
    public static implicit operator string(Vec2 vec) {
        return vec.ToString();
    }

    /// <summary> Add two vectors together. </summary>
    public static Vec2 operator +(Vec2 left, Vec2 right) {
        return new Vec2(left.x + right.x, left.y + right.y);
    }

    /// <summary> Subtract two vectors. </summary>
    public static Vec2 operator -(Vec2 left, Vec2 right) {
        return new Vec2(left.x - right.x, left.y - right.y);
    }

    /// <summary> Multiply two vectors. </summary>
    public static Vec2 operator *(Vec2 left, Vec2 right) {
        return new Vec2(left.x * right.x, left.y * right.y);
    }

    /// <summary> Divide two vectors. </summary>
    public static Vec2 operator /(Vec2 left, Vec2 right) {
        return new Vec2(left.x / right.x, left.y / right.y);
    }

    /// <summary> Normalize a vector. </summary>
    public static Vec2 Normalize(Vec2 value) {
        double length = Math.Sqrt(value.x * value.x + value.y * value.y);
        return new Vec2(value.x / length, value.y / length);
    }

    /// <summary> Get the length of a vector. </summary>
    public static double Length(Vec2 value) {
        return Math.Sqrt(value.x * value.x + value.y * value.y);
    }

    /// <summary> Get the distance between two vectors. </summary>
    public static double Distance(Vec2 left, Vec2 right) {
        return Length(left - right);
    }

    /// <summary> Get the dot product of two vectors. </summary>
    public static double Dot(Vec2 left, Vec2 right) {
        return left.x * right.x + left.y * right.y;
    }

    /// <summary> Get the cross product of two vectors. </summary>
    public static double Cross(Vec2 left, Vec2 right) {
        return left.x * right.y - left.y * right.x;
    }

    /// <summary> Get the angle between two vectors. </summary>
    public static double Angle(Vec2 left, Vec2 right) {
        return Math.Acos(Dot(left, right) / (Length(left) * Length(right)));
    }
}