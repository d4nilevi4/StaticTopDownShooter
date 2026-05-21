namespace Shooter;

public struct PeriodicRethinkSystem : ISystem
{
    private float _accumulator;

    public void Init()
    {
        Game.SetResource(new RethinkInterval { Value = 0.2f });
    }

    public void Update()
    {
        var interval = Game.GetResource<RethinkInterval>().Value;
        _accumulator += Game.GetResource<TimeData>().DeltaTime;
        if (_accumulator < interval) return;
        _accumulator -= interval;

        Game.Query<All<IsEnemy, BrainComponent>>().For(static (entity) =>
        {
            entity.Set<IsNeedRethink>();
        });
    }
}