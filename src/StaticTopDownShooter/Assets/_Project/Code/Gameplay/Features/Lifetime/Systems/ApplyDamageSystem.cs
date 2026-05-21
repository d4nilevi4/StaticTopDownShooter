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
            DamageEvent eventData = damageEvent.Value;

            if (eventData.Target.TryUnpack<GameWT>(out var target) && target.IsMatch<All<Hp>>())
            {
                ref Hp health = ref target.Mut<Hp>();

                health.Current -= eventData.Value;

                if (health.Current <= 0)
                {
                    Game.SendEvent(new DeadEvent()
                    {
                        Target = target.GID
                    });
                }
            }
        }
    }

    public void Destroy()
    {
        Game.DeleteEventReceiver(ref _damageEventReceiver);
    }
}