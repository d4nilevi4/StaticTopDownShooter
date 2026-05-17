namespace Shooter;

public struct ApplyDamageSystem : ISystem
{
    private EventReceiver<GameWT, DamageEvent> _damageEventReceiver;

    public void Init()
    {
        _damageEventReceiver = Game.RegisterEventReceiver<DamageEvent>();
    }

    public void Update()
    {
        foreach (World<GameWT>.Event<DamageEvent> damageEvent in _damageEventReceiver)
        {
            Game.Query<All<Hp>>().For(
                damageEvent.Value,
                static
            (
                ref DamageEvent damageEvent,
                Game.Entity entity,
                ref Hp health
            ) =>
            {
                if (damageEvent.Target != entity.GID)
                    return;

                health.Current -= damageEvent.Value;

                if (health.Current <= 0)
                {
                    Game.SendEvent(new DeadEvent()
                    {
                        Target = entity.GID
                    });
                }
            });
        }
    }

    public void Destroy()
    {
        Game.DeleteEventReceiver(ref _damageEventReceiver);
    }
}
