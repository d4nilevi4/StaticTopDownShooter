using NoOpArmy.WiseFeline;

namespace Shooter
{
    public sealed class HasAmmoConsideration : ConsiderationBase
    {
        protected override float GetValue(Component target, Game.Entity entity)
        {
            return entity.Read<AmmoCount>().Value > 0 ? 1f : 0f;
        }
    }
}
