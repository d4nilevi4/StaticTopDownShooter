namespace Shooter;

public struct SyncTimeResourceSystem : ISystem
{
    public void Init()
    {
        Game.SetResource(new TimeData());
    }

    public void Update()
    {
        ref var time = ref Game.GetResource<TimeData>();

        time.Time = Time.time;
        time.DeltaTime = Time.deltaTime;
        time.FixedDeltaTime = Time.fixedDeltaTime;
    }
}