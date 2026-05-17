using UnityEngine.AI;

namespace Shooter;

[Serializable] public struct IsNeedRethink : ITag {}

[Serializable]
public struct AgentBehaviour : IComponent
{
    public NavMeshAgent Value;

    public void OnAdd<TWorld>(World<TWorld>.Entity self) where TWorld : struct, IWorldType
    {
        if (Value == null) return;
        Value.updatePosition = false;
        Value.updateRotation = false;
        Value.updateUpAxis  = false;
        Value.autoBraking   = false;
    }
}
