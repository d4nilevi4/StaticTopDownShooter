namespace Shooter;

public struct RotateBulletTowardsMoveDirectionSystem : ISystem
{
    public void Update()
    {
        Game.Query<All<IsBullet>>().For(static
        (
            in RB rb,
            in MoveDirection moveDirection
        ) =>
        {
            float angle = Mathf.Atan2(moveDirection.Value.y, moveDirection.Value.x) * Mathf.Rad2Deg;
            rb.Value.MoveRotation(angle);
        });
    }
}