namespace Shooter;

public struct UpdateCameraFollowPosition : ISystem
{
    public void Update()
    {
        Game.Query<All<IsPlayer>>().For(static
        (
            ref CameraFollowTransform cameraFollowTransform,
            in TransformComponent playerTransform,
            in MousePosition mousePos
        ) =>
        {
            ref readonly var settings = ref Game.GetResource<CameraFollowSettings>();
            var mainCamera = Game.GetResource<MainCamera>().Value;

            Vector3 playerPos = playerTransform.Value.position;

            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(mousePos.Value);
            mouseWorld.z = playerPos.z;

            Vector3 toAim = mouseWorld - playerPos;
            Vector3 clamped = Vector3.ClampMagnitude(toAim, settings.MaxLookOffset);
            Vector3 desired = playerPos + clamped * settings.AimWeight;
            desired.z = playerPos.z;

            cameraFollowTransform.Value.position = Vector3.Lerp(
                cameraFollowTransform.Value.position, desired, settings.FollowSpeed * Time.deltaTime);
        });
    }
}
