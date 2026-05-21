namespace Shooter;

public struct AnimateMovementSystem : ISystem
{
    public void Update()
    {
        Game.Query().For(static (in MoveInput input, in SpriteRenderer renderer) =>
        {
            if (input.Value.x > 0)
            {
                renderer.Value.flipX = false;
            }
            else if (input.Value.x < 0)
            {
                renderer.Value.flipX = true;
            }
        });
    }
}