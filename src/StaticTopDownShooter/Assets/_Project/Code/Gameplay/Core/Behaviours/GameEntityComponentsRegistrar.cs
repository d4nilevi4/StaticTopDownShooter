namespace Shooter
{
    public abstract class GameEntityComponentsRegistrar : MonoBehaviour, IComponentsRegistrar<GameWT>
    {
        public abstract void RegisterComponents(World<GameWT>.Entity entity);
    }
}