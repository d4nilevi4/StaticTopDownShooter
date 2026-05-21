namespace Shooter;

public struct SyncAttackTargetPositionAndAimSystem : ISystem
{
    public void Update()
    {
        Game.Query().For(static (ref AttackTargetPosition targetPosition, in AimTransform aim) =>
        {
            targetPosition.Value = aim.Value.position.ToVector2();
        });
    }
}