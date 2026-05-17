namespace Shooter
{
    [RequireComponent(typeof(Camera))]
    public sealed class MainCameraResourceRegistrar : MonoBehaviour, IResourcesRegistrar
    {
        public void RegisterResources()
        {
            Game.SetResource(new MainCamera() { Value = GetComponent<Camera>() });
        }
    }
}
