namespace Shooter;

public struct ApplyMoveImpulseSystem : ISystem
{
    public void Update()
    {
        Game.Query().For(static (Game.Entity entity, in MoveImpulse impulse, in RB rb) =>
        {
            rb.Value.AddForce(impulse.Value, ForceMode2D.Impulse);
            entity.Delete<MoveImpulse>();
        });
    }
}