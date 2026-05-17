namespace Shooter;

public struct EmitPlayerMousePositionSystem : ISystem
{
    public void Update()
    {
        Game.Query<All<IsPlayer, MousePosition>>().For(static (ref MousePosition input) =>
        {
            input.Value = Game.GetResource<PlayerInputMap>().Value.Locomotion.MousePos.ReadValue<Vector2>();
        });
    }
}