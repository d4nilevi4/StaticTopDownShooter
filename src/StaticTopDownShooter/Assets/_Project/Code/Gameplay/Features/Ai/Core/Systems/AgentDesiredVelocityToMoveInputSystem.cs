namespace Shooter;

public struct AgentDesiredVelocityToMoveInputSystem : ISystem
{
    public void Update()
    {
        Game.Query()
            .For(static (ref MoveInput input, in AgentBehaviour agent) =>
            {
                var nav = agent.Value;
                if (nav == null || !nav.isOnNavMesh)
                {
                    input.Value = Vector2.zero;
                    return;
                }

                var v = nav.desiredVelocity;
                if (v.sqrMagnitude < 0.0001f)
                {
                    input.Value = Vector2.zero;
                    return;
                }

                Vector2 dir = new Vector2(v.x, v.y);
                input.Value = dir.normalized;
            });
    }
}
