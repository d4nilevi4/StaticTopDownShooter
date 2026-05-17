namespace Shooter
{
    public sealed class EntityColliderRegistrar : GameEntityComponentsRegistrar
    {
        public Collider2D Collider;

        public override void RegisterComponents(World<GameWT>.Entity entity)
        {
            ColliderRegistry.RegisterColliderId(Collider.GetInstanceID(), entity.GID);
        }
    }
}