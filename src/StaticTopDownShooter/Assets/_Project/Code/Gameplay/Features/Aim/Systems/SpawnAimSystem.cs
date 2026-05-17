namespace Shooter;

public struct SpawnAimSystem : ISystem
{
    public void Init()
    {
        if (Game.Query<All<IsPlayer, AimPrefab>>().One(out var entity))
        {
            GameObject aim = Object.Instantiate(entity.Ref<AimPrefab>().Value);
            aim.name = "Aim";
            entity.Add<AimTransform>().Value = aim.transform;
        }
        else
        {
            throw new Exception("No player entity with AimPrefab found when trying to spawn aim.");
        }
    }
}