using UnityEngine.AI;

namespace Shooter
{
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class AgentBehaviourRegistrar : GameEntityComponentsRegistrar
    {
        [SerializeField] private NavMeshAgent _agent;

        private void Reset() => _agent = GetComponent<NavMeshAgent>();

        public override void RegisterComponents(World<GameWT>.Entity entity)
        {
            var agent = _agent != null ? _agent : GetComponent<NavMeshAgent>();
            entity.Set(new AgentBehaviour { Value = agent });
        }
    }
}
