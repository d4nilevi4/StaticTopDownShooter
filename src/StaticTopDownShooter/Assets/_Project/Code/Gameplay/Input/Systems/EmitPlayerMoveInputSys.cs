namespace Shooter;

public struct EmitPlayerMoveInputSys : ISystem
{
    public void Update()
    {
        Game.Query<All<IsPlayer>>().For(static (ref MoveInput input) =>
        {
            input.Value = Game.GetResource<PlayerInputMap>().Value.Locomotion.Move.ReadValue<Vector2>();
        });
    }
}