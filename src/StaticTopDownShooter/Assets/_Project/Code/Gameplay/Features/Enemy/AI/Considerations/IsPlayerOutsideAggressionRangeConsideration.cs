using NoOpArmy.WiseFeline;

namespace Shooter
{
    public sealed class IsPlayerOutsideAggressionRangeConsideration : ConsiderationBase
    {
        // Evaluate score based on distance to player and aggression range. Returns 1 if player is outside aggression range, otherwise 0.
        protected override float GetValue(Component target, Game.Entity entity)
        {
            // Try Get Player position, if fails return 1 (consider player outside aggression range)
            if (!EnemyAiUtils.TryGetPlayerPosition(out var playerPos))
                return 1f;

            // Get self position and aggression range, then calculate distance to player and compare with aggression range
            Vector2 selfPos = entity.Read<TransformComponent>().Value.position;
            float aggression = entity.Read<AggressionRange>().Value;
            return Vector2.Distance(selfPos, playerPos) > aggression ? 1f : 0f;
        }
    }
}
