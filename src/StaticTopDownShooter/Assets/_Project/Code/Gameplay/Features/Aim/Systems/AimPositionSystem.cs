namespace Shooter;

public struct AimPositionSystem : ISystem
{
    public void Update()
    {
        Game.Query<All<AimTransform, MousePosition>>().For(static (ref AimTransform aim, in MousePosition mousePos) =>
        {
            Vector3 worldPos = Game.GetResource<MainCamera>().Value.ScreenToWorldPoint(mousePos.Value);
            worldPos.z = 0f;
            aim.Value.position = worldPos;
        });
    }
}