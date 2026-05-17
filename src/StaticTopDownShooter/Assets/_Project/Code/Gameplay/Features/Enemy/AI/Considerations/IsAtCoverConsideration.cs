using NoOpArmy.WiseFeline;

namespace Shooter
{
    public sealed class IsAtCoverConsideration : ConsiderationBase
    {
        protected override float GetValue(Component target, Game.Entity entity)
        {
            return entity.Has<IsAtCover>() ? 1f : 0f;
        }
    }
}
