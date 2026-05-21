namespace Shooter;

public struct RethinkBrainSystem : ISystem
{
    public void Update()
    {
        Game.Query<All<IsNeedRethink>>().For(static
            (Game.Entity entity, ref BrainComponent brain) =>
        {
            entity.Delete<IsNeedRethink>();
            brain.Value.UpdateActionsTargets(entity);
            brain.Value.Think(entity);
        });
    }
}