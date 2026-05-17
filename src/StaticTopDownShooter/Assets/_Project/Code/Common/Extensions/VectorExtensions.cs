namespace Shooter;

public static class VectorExtensions
{
    public static Vector3 SetX(this Vector3 v, float x)
    {
        var tmp = v;
        tmp.x = x;
        v = tmp;
        return v;
    }

    public static Vector3 SetY(this Vector3 v, float y)
    {
        var tmp = v;
        tmp.y = y;
        v = tmp;
        return v;
    }

    public static Vector3 SetZ(this Vector3 v, float z)
    {
        var tmp = v;
        tmp.z = z;
        v = tmp;
        return v;
    }

    public static Vector2 SetX(this Vector2 v, float x)
    {
        var tmp = v;
        tmp.x = x;
        v = tmp;
        return v;
    }

    public static Vector2 SetY(this Vector2 v, float y)
    {
        var tmp = v;
        tmp.y = y;
        v = tmp;
        return v;
    }

    public static Vector3 AddX(this Vector3 v, float xDelta)
    {
        var tmp = v;
        tmp.x = tmp.x + xDelta;
        v = tmp;
        return v;
    }

    public static Vector3 AddY(this Vector3 v, float yDelta)
    {
        var tmp = v;
        tmp.y = tmp.y + yDelta;
        v = tmp;
        return v;
    }

    public static Vector2 AddX(this Vector2 v, float xDelta)
    {
        var tmp = v;
        tmp.x = tmp.x + xDelta;
        v = tmp;
        return v;
    }

    public static Vector2 AddY(this Vector2 v, float yDelta)
    {
        var tmp = v;
        tmp.y = tmp.y + yDelta;
        v = tmp;
        return v;
    }

    public static Vector3 InvertX(this Vector3 vector3)
    {
        var tmp = vector3;
        tmp.x = -tmp.x;
        vector3 = tmp;
        return vector3;
    }

    public static Vector3 InvertY(this Vector3 vector3)
    {
        var tmp = vector3;
        tmp.y = -tmp.y;
        vector3 = tmp;
        return vector3;
    }

    public static Vector3 InvertZ(this Vector3 vector3)
    {
        var tmp = vector3;
        tmp.z = -tmp.z;
        vector3 = tmp;
        return vector3;
    }

    public static Vector3 FarAway()
    {
        return new Vector3(-9999, -9999, -9999);
    }

    public static Vector3 XZProjection(this Vector3 vec) => vec.SetY(0);
    public static Vector2 XZ2Projection(this Vector3 vec) => new(vec.x, vec.z);

    public static Vector2 ToVector2(this Vector3 vec) => new(vec.x, vec.y);
}