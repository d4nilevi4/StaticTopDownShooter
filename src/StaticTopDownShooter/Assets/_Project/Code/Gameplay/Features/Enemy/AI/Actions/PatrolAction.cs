using NoOpArmy.WiseFeline;

namespace Shooter
{
    public sealed class PatrolAction : ActionBase
    {
        protected override void OnStart(Game.Entity entity)
        {
            if (entity.Has<IsAtCover>())
                entity.Delete<IsAtCover>();

            entity.Set<IsPatrolling>();
            entity.Mut<Speed>().Value = entity.Read<WalkSpeed>().Value;

            if (entity.Read<PatrolPoints>().Value.Length == 0)
            {
                ActionFailed(entity);
            }
        }

        protected override void OnUpdate(Game.Entity entity)
        {
            var points = entity.Read<PatrolPoints>().Value;
            if (points.Length == 0)
            {
                ActionFailed(entity);
                return;
            }

            ref var indexComponent = ref entity.Mut<PatrolPointIndex>();
            var idx = indexComponent.Value;
            if (idx < 0 || idx >= points.Length) idx = 0;

            var targetTransform = points[idx];
            if (targetTransform == null)
            {
                ActionFailed(entity);
                return;
            }

            var agent = entity.Read<AgentBehaviour>().Value;
            if (agent == null || !agent.isOnNavMesh)
            {
                ActionFailed(entity);
                return;
            }

            agent.SetDestination(targetTransform.position);

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                indexComponent.Value = (idx + 1) % points.Length;
                entity.Mut<IdleTimer>().Value = entity.Read<IdleTime>().Value;
                agent.ResetPath();
                ActionSucceeded(entity);
            }
        }

        protected override void OnFinish(Game.Entity entity)
        {
            if (entity.Has<IsPatrolling>())
                entity.Delete<IsPatrolling>();

            var agent = entity.Read<AgentBehaviour>().Value;
            if (agent != null && agent.isOnNavMesh && agent.hasPath)
                agent.ResetPath();
        }
    }
}
