namespace Shooter;

public struct HandleBulletCollisionInputSystem : ISystem
{
    private EventReceiver<GameWT, TriggerEnterEvent> _triggerEnterReceiver;

    public void Init()
    {
        _triggerEnterReceiver = Game.RegisterEventReceiver<TriggerEnterEvent>();
    }

    public void Update()
    {
        foreach (var triggerEnterEvent in _triggerEnterReceiver)
        {
            if (!triggerEnterEvent.Value.Source.TryUnpack<GameWT>(out var entity))
                continue;

            if(!entity.IsMatch<All<OwnerGID, IsBullet>>())
                continue;

            EntityGID other = ColliderRegistry.GetEntityGid(triggerEnterEvent.Value.OtherColliderId);
            var ownerGid = entity.Read<OwnerGID>();

            if (other == ownerGid.Value)
                continue;

            if (other.Raw == 0)
                continue;

            Game.SendEvent(new DamageEvent()
            {
                Source = ownerGid.Value,
                Target = other,
                Value = 100
            });

            Game.SendEvent(new DeadEvent()
            {
                Target = entity.GID,
            });
        }
    }

    public void Destroy()
    {
        Game.DeleteEventReceiver(ref _triggerEnterReceiver);
    }
}
