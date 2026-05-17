namespace Shooter;

public struct UpdateMoveDirectionByMoveInputSystem : ISystem
{
    public void Update()
    {
        Game.Query<All<MoveInput, MoveDirection>>()
            .For(static (ref MoveDirection direction, in MoveInput input) =>
            {
                direction.Value = input.Value.normalized;
            });
    }
}
