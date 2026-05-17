namespace Shooter;

[Serializable]
public struct MainCamera : IResource { public Camera Value; }

[Serializable] public struct CameraFollowTransform : IComponent { public Transform Value; }
