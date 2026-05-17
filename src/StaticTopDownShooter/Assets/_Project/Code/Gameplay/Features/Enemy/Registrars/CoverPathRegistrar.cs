namespace Shooter
{
    public sealed class CoverPathRegistrar : GameEntityComponentsRegistrar
    {
        public CoverObstacle[] Obstacles;

        public override void RegisterComponents(World<GameWT>.Entity entity)
        {
            entity.Set(new CoverPoints { Value = Obstacles ?? Array.Empty<CoverObstacle>() });
            entity.Set(new CurrentCoverIndex { Value = -1 });
            entity.Set(new CurrentCoverWorldPos { Value = Vector2.zero });
        }
    }
}
