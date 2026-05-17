using System.Collections.Generic;

namespace Shooter
{
    public sealed class RootGameComponentsRegistrar : MonoBehaviour
    {
        public GameEntityProvider EntityProvider;
        public List<GameEntityComponentsRegistrar> ComponentsRegistrars;

        public void RegisterComponents()
        {
            foreach (GameEntityComponentsRegistrar registrar in ComponentsRegistrars)
            {
                registrar.RegisterComponents(EntityProvider.Entity);
            }
        }
    }
}