using NoOpArmy.WiseFeline;

namespace Shooter
{
    public sealed class IsPlayerInAggressionRangeConsideration : ConsiderationBase
    {
        protected override float GetValue(Component target, Game.Entity entity)
        {
            if (!EnemyAiUtils.TryGetPlayerPosition(out var playerPos))
                return 0f;

            Vector2 selfPos = entity.Read<TransformComponent>().Value.position;
            float aggression = entity.Read<AggressionRange>().Value;
            return Vector2.Distance(selfPos, playerPos) <= aggression ? 1f : 0f;
        }
    }
}
