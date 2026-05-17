namespace Shooter;

public struct InitializePlayerInputMapInputSys : ISystem
{
    public void Init()
    {
        Game.SetResource<PlayerInputMap>(new PlayerInputMap()
        {
            Value = new PlayerInputActions()
        });
        Game.GetResource<PlayerInputMap>().Value.Enable();
    }
}