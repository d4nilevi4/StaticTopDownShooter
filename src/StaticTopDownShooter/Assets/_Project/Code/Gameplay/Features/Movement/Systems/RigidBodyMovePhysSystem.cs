namespace Shooter;

public struct RigidBodyMovePhysSystem : ISystem
{
    public void Update()
    {
        Game
            .Query<All<MoveDirection, Speed, RB>>()
            .For(static (in MoveDirection input, in Speed speed, in RB rb) =>
            {
                rb.Value.linearVelocity = input.Value * speed.Value;
            });
    }
}
