namespace Shooter;

public struct UpdateMoveDirectionByMoveInputSystem : ISystem
{
    public void Update()
    {
        Game.Query().For(static (ref MoveDirection direction, in MoveInput input) =>
        {
            direction.Value = input.Value.normalized;
        });
    }
}