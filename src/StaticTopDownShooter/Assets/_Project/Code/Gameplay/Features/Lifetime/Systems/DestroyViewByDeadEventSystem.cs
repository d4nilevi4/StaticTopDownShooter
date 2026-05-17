namespace Shooter;

public struct DestroyViewByDeadEventSystem : ISystem
{
    private EventReceiver<GameWT, DeadEvent> _deadEventReceiver;

    public void Init()
    {
        _deadEventReceiver = Game.RegisterEventReceiver<DeadEvent>();
    }

    public void Update()
    {
        foreach (World<GameWT>.Event<DeadEvent> deadEvent in _deadEventReceiver)
        {
            // if (!deadEvent.IsLastReading())
                // continue;

            Game.Query<All<TransformComponent>>().For(
                deadEvent.Value,
                static
                (
                    ref DeadEvent deadEvent,
                    Game.Entity entity,
                    ref TransformComponent component
                ) =>
                {
                    if (deadEvent.Target != entity.GID)
                        return;

                    Object.Destroy(component.Value.gameObject);
                    entity.Destroy();
                });
        }
    }

    public void Destroy()
    {
        Game.DeleteEventReceiver(ref _deadEventReceiver);
    }
}
