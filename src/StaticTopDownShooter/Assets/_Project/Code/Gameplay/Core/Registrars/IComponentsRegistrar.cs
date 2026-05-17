namespace Shooter;

public interface IComponentsRegistrar<TWorld> where TWorld : struct, IWorldType
{
    void RegisterComponents(World<TWorld>.Entity entity);
}