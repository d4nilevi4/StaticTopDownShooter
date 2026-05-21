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
        foreach (World<GameWT>.Event<DeadEvent> deadEvent in _deadEventReceiver.LastOnly())
        {
            var deadEventData = deadEvent.Value;

            if (deadEventData.Target.TryUnpack<GameWT>(out var target) && target.IsMatch<All<TransformComponent>>())
            {
                Object.Destroy(target.Mut<TransformComponent>().Value.gameObject);
                target.Destroy();
            }            
        }
    }

    public void Destroy()
    {
        Game.DeleteEventReceiver(ref _deadEventReceiver);
    }
}
