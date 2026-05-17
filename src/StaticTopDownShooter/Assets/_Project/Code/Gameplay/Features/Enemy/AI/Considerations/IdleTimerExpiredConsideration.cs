using NoOpArmy.WiseFeline;

namespace Shooter
{
    public sealed class IdleTimerExpiredConsideration : ConsiderationBase
    {
        protected override float GetValue(Component target, Game.Entity entity)
        {
            return entity.Read<IdleTimer>().Value <= 0f ? 1f : 0f;
        }
    }
}
