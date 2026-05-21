namespace Shooter;

public struct SyncAgentSimulationPositionSystem : ISystem
{
    public void Update()
    {
        Game.Query()
            .For(static (in AgentBehaviour agent, in TransformComponent transform) =>
            {
                if (agent.Value == null) return;
                agent.Value.nextPosition = transform.Value.position;
            });
    }
}
