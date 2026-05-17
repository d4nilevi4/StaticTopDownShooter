namespace Shooter
{
    public sealed class CameraFollowSettingsResourceRegistrar : MonoBehaviour, IResourcesRegistrar
    {
        public CameraFollowSettings CameraFollowSettings;

        public void RegisterResources()
        {
            Game.SetResource(CameraFollowSettings);
        }
    }
}
