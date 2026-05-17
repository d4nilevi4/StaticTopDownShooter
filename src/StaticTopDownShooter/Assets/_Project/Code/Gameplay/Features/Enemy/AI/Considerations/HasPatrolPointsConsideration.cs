using NoOpArmy.WiseFeline;

namespace Shooter
{
    public sealed class HasPatrolPointsConsideration : ConsiderationBase
    {
        protected override float GetValue(Component target, Game.Entity entity)
        {
            var points = entity.Read<PatrolPoints>().Value;
            return points != null && points.Length > 0 ? 1f : 0f;
        }
    }
}
