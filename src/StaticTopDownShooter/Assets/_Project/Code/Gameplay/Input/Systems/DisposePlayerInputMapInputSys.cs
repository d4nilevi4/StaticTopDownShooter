namespace Shooter;

public struct DisposePlayerInputMapInputSys : ISystem
{
    public void Destroy()
    {
        Game.GetResource<PlayerInputMap>().Value.Disable();
        Game.GetResource<PlayerInputMap>().Value.Dispose();
    }
}