using NoOpArmy.WiseFeline;

namespace Shooter
{
    public sealed class IdleAction : ActionBase
    {
        // Called one time when action starts
        protected override void OnStart(Game.Entity entity)
        {
            entity.Set<IsIdling>();
            entity.Mut<IdleTimer>().Value = entity.Read<IdleTime>().Value;

            var agent = entity.Read<AgentBehaviour>().Value;
            if (agent != null && agent.isOnNavMesh && agent.hasPath)
                agent.ResetPath();
        }

        // Called every frame while the action is active
        protected override void OnUpdate(Game.Entity entity)
        {
            if (entity.Read<IdleTimer>().Value <= 0f) // Success action when idle timer expired
                ActionSucceeded(entity);
        }

        // Called once when action is finished whether succeeded or not
        protected override void OnFinish(Game.Entity entity)
        {
            if (entity.Has<IsIdling>()) // Deleting IsIdling component on action finish to indicate the entity is no longer idling
                entity.Delete<IsIdling>();
        }
    }
}
