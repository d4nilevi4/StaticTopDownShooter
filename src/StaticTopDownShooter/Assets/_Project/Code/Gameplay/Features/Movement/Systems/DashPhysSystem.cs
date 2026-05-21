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
        {
            var dodgeEventData = dodgeEvent.Value;

            if (dodgeEventData.InputOwner.TryUnpack<GameWT>(out var target) &&
                target.IsMatch<All<DashImpulse, CanDash, MoveInput>>())
            {
                Vector2 moveInput = target.Read<MoveInput>().Value;
                Vector2 dashDirection = moveInput.sqrMagnitude < 0.01f
                    ? Vector2.up
                    : moveInput.normalized;

                target.Add<MoveImpulse>().Value = dashDirection * target.Read<DashImpulse>().Value;
            }
        }
    }

    public void Destroy()
    {
        Game.DeleteEventReceiver(ref _dashInputReceiver);
    }
}
