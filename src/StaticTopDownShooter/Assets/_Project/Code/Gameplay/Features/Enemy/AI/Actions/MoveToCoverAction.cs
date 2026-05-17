using NoOpArmy.WiseFeline;

namespace Shooter
{
    public sealed class MoveToCoverAction : ActionBase
    {
        protected override void UpdateTargets(Game.Entity entity)
        {
            ClearTargets();
            var obstacles = entity.Read<CoverPoints>().Value;
            if (obstacles == null || obstacles.Length == 0) return;

            Vector2 selfPos = entity.Read<TransformComponent>().Value.position;

            CoverObstacle closest = null;
            float bestSqr = float.PositiveInfinity;
            for (int i = 0; i < obstacles.Length; i++)
            {
                var ob = obstacles[i];
                if (ob == null) continue;
                float sqr = ((Vector2)ob.transform.position - selfPos).sqrMagnitude;
                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    closest = ob;
                }
            }

            if (closest != null)
                AddTarget(closest.transform);
        }

        protected override void OnStart(Game.Entity entity)
        {
            if (entity.Has<IsAtCover>())
                entity.Delete<IsAtCover>();

            var obstacles = entity.Read<CoverPoints>().Value;
            int idx = -1;
            for (int i = 0; i < obstacles.Length; i++)
            {
                if (obstacles[i] != null && obstacles[i].transform == ChosenTarget)
                {
                    idx = i;
                    break;
                }
            }
            if (idx < 0)
            {
                ActionFailed(entity);
                return;
            }

            var obstacle = obstacles[idx];
            var points = obstacle.Points;
            if (points == null || points.Length == 0)
            {
                ActionFailed(entity);
                return;
            }

            bool hasPlayer = EnemyAiUtils.TryGetPlayerPosition(out Vector2 playerPos);

            Transform chosenPoint = null;
            float bestSqr = -1f;
            for (int i = 0; i < points.Length; i++)
            {
                var p = points[i];
                if (p == null) continue;
                if (!hasPlayer)
                {
                    chosenPoint = p;
                    break;
                }
                float sqr = ((Vector2)p.position - playerPos).sqrMagnitude;
                if (sqr > bestSqr)
                {
                    bestSqr = sqr;
                    chosenPoint = p;
                }
            }
            if (chosenPoint == null)
            {
                ActionFailed(entity);
                return;
            }

            entity.Mut<CurrentCoverIndex>().Value = idx;
            Vector2 worldPos = chosenPoint.position;
            entity.Mut<CurrentCoverWorldPos>().Value = worldPos;

            entity.Set<IsMovingToCover>();
            entity.Mut<Speed>().Value = entity.Read<RunSpeed>().Value;

            var agent = entity.Read<AgentBehaviour>().Value;
            if (agent == null || !agent.isOnNavMesh)
            {
                ActionFailed(entity);
                return;
            }
            agent.SetDestination(worldPos);
        }

        protected override void OnUpdate(Game.Entity entity)
        {
            var agent = entity.Read<AgentBehaviour>().Value;
            if (agent == null || !agent.isOnNavMesh)
            {
                ActionFailed(entity);
                return;
            }

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                agent.ResetPath();
                entity.Set<IsAtCover>();
                ActionSucceeded(entity);
            }
        }

        protected override void OnFinish(Game.Entity entity)
        {
            if (entity.Has<IsMovingToCover>())
                entity.Delete<IsMovingToCover>();

            var agent = entity.Read<AgentBehaviour>().Value;
            if (agent != null && agent.isOnNavMesh && agent.hasPath)
                agent.ResetPath();
        }
    }
}
