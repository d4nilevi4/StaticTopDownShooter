namespace Shooter;

internal static class EnemyAiUtils
{
    // Tries to get the player's position. Returns true if successful, false otherwise.
    public static bool TryGetPlayerPosition(out Vector2 pos)
    {
        // Query for a single entity that has both IsPlayer tag and TransformComponent
        if (Game.Query<All<IsPlayer, TransformComponent>>().One(out var player))
        {
            // Read the TransformComponent and extract the world position
            pos = player.Read<TransformComponent>().Value.position;
            // Successfully retrieved player position
            return true;
        }

        // No player entity found, set default position
        pos = default;
        // Failed to retrieve player position
        return false;
    }
}
