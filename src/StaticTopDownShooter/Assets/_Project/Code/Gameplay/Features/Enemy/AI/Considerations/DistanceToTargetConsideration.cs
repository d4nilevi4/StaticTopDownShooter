using NoOpArmy.WiseFeline;

namespace Shooter
{
    public sealed class DistanceToTargetConsideration : ConsiderationBase
    {
        protected override float GetValue(Component target, Game.Entity entity)
        {
            if (target == null) return 0f;
            Vector2 selfPos = entity.Read<TransformComponent>().Value.position;
            Vector2 targetPos = target.transform.position;
            return Vector2.Distance(selfPos, targetPos) / 16;
        }
    }
}
