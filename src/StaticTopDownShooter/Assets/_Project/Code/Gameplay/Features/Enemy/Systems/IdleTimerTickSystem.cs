namespace Shooter;

public struct IdleTimerTickSystem : ISystem
{
    public void Update()
    {
        var timeData = Game.GetResource<TimeData>();
        Game.Query<All<IsEnemy, IsIdling>>().For(timeData, static (ref TimeData time, ref IdleTimer timer) =>
        {
            timer.Value -= time.DeltaTime;
        });
    }
}