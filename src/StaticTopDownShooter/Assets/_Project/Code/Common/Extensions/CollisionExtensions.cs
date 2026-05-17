namespace Shooter;

[Flags]
public enum CollisionLayer
{
    // Interactable = 1 << 8,
}

public static class CollisionExtensions
{
    public static int AsMask(this CollisionLayer layer) =>
        (int)layer;
}