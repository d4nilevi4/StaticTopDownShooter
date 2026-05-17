namespace Shooter;

public struct SyncAttackTargetPositionAndAimSystem : ISystem
{
    public void Update()
    {
        Game.Query<All<AttackTargetPosition, AimTransform>>()
            .For(static (ref AttackTargetPosition targetPosition, in AimTransform aim) =>
            {
                targetPosition.Value = aim.Value.position.ToVector2();
            });
    }
}
