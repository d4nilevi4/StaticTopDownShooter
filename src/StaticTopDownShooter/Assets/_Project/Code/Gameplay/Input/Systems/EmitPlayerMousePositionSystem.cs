namespace Shooter;

public struct EmitPlayerMousePositionSystem : ISystem
{
    public void Update()
    {
        Game.Query<All<IsPlayer>>().For(static (ref MousePosition input) =>
        {
            input.Value = Game.GetResource<PlayerInputMap>().Value.Locomotion.MousePos.ReadValue<Vector2>();
        });
    }
}