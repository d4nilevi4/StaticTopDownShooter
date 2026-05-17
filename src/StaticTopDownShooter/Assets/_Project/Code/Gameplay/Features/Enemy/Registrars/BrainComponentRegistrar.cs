using NoOpArmy.WiseFeline;

namespace Shooter
{
    public sealed class BrainComponentRegistrar : GameEntityComponentsRegistrar
    {
        public Brain Brain;

        public override void RegisterComponents(World<GameWT>.Entity entity)
        {
            entity.Set(new BrainComponent { Value = Brain });
            entity.Set<IsNeedRethink>();
            Brain.InitializeBrain(entity);
        }
    }
}
