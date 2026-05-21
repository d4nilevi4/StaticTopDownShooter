using NoOpArmy.WiseFeline;
using UnityEngine.AI;

namespace Shooter
{
    public sealed class AttackFromCoverAction : ActionBase
    {
        protected override void OnStart(Game.Entity entity)
        {
            entity.Set<IsAttacking>();

            NavMeshAgent agent = entity.Read<AgentBehaviour>().Value;
            if (agent != null && agent.isOnNavMesh && agent.hasPath)
                agent.ResetPath();
        }

        protected override void OnUpdate(Game.Entity entity)
        {
            if (!Game.Query<All<IsPlayer, TransformComponent>>().One(out var player))
            {
                ActionFailed(entity);
            }
            

            Transform targetTransform = player.Read<TransformComponent>().Value;
            entity.Mut<AttackTargetPosition>().Value = targetTransform.position;

            if (entity.Has<ShootAvailable>() && entity.Read<AmmoCount>().Value > 0)
            {
                Game.SendEvent(new AttackActionPerformed(entity.GID));
            }
        }

        protected override void OnFinish(Game.Entity entity)
        {
            if (entity.Has<IsAttacking>())
                entity.Delete<IsAttacking>();
        }
    }
}
