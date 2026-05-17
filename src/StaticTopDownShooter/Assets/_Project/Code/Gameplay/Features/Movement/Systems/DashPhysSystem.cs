namespace Shooter;

public struct DashPhysSystem : ISystem
{
    private EventReceiver<GameWT, DodgeActionPerformed> _dashInputReceiver;

    public void Init()
    {
        _dashInputReceiver = Game.RegisterEventReceiver<DodgeActionPerformed>();
    }

    public void Update()
    {
        foreach (var dodgeEvent in _dashInputReceiver)
        foreach (var entity in Game.Query<All<DashImpulse, CanDash, MoveInput>>().Entities())
        {
            if (entity.GID != dodgeEvent.Value.InputOwner)
                continue;

            Vector2 moveInput = entity.Read<MoveInput>().Value;
            Vector2 dashDirection = moveInput.sqrMagnitude < 0.01f
                ? Vector2.up
                : moveInput.normalized;

            entity.Add<MoveImpulse>().Value = dashDirection * entity.Read<DashImpulse>().Value;
        }
    }

    public void Destroy()
    {
        Game.DeleteEventReceiver(ref _dashInputReceiver);
    }
}
