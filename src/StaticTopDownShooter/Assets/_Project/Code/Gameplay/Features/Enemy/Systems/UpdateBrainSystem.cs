namespace Shooter;

public struct UpdateBrainSystem : ISystem
{
    public void Update()
    {
        Game.Query<All<BrainComponent>>().For(static (Game.Entity entity, in BrainComponent brain) =>
        {
            brain.Value.UpdateBrain(entity);
        });
    }
}
