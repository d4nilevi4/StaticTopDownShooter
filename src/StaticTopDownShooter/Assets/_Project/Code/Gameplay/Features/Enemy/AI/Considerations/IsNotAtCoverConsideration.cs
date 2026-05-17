using NoOpArmy.WiseFeline;

namespace Shooter
{
    public sealed class IsNotAtCoverConsideration : ConsiderationBase
    {
        protected override float GetValue(Component target, Game.Entity entity)
        {
            return entity.Has<IsAtCover>() ? 0f : 1f;
        }
    }
}
