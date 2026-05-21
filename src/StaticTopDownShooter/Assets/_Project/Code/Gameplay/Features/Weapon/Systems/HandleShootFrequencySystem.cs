namespace Shooter;

public struct HandleShootFrequencySystem : ISystem
{
    public void Update()
    {
        Game.Query<All<ShootCooldownTimer, ShootAvailable>>().For(static (Game.Entity entity) =>
        {
            entity.Delete<ShootAvailable>();
        });

        TimeData timeData = Game.GetResource<TimeData>();

        Game.Query().For(timeData, static (ref TimeData time, Game.Entity entity, ref ShootCooldownTimer timer) =>
        {
            timer.Value -= time.DeltaTime;

            if (timer.Value <= 0)
            {
                entity.Delete<ShootCooldownTimer>();
                entity.Set<ShootAvailable>();
            }
        });
    }
}