using NoOpArmy.WiseFeline;

namespace Shooter
{
    public sealed class HasCoverPointsConsideration : ConsiderationBase
    {
        protected override float GetValue(Component target, Game.Entity entity)
        {
            var areas = entity.Read<CoverPoints>().Value;
            return areas != null && areas.Length > 0 ? 1f : 0f;
        }
    }
}
