namespace Shooter {
    public class GameEntityProvider : StaticEcsEntityProvider<GameWT>
    {
        public event Action<EntityGID> OnEntityCreated;

        public override bool CreateEntity()
        {
            bool entity = base.CreateEntity();
            OnEntityCreated?.Invoke(Entity.GID);
            return entity;
        }
    }
}
