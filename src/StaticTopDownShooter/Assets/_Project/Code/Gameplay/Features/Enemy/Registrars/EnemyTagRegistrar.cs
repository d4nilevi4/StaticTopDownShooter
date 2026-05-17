namespace Shooter
{
    public sealed class EnemyTagRegistrar : GameEntityComponentsRegistrar
    {
        public override void RegisterComponents(World<GameWT>.Entity entity)
        {
            entity.Set<IsEnemy>();
            entity.Set<IsIdling>();
            ref var timer = ref entity.Add<IdleTimer>();
            timer.Value = entity.Has<IdleTime>() ? entity.Read<IdleTime>().Value : 0f;
        }
    }
}
