namespace Shooter;

public struct IsAim : ITag {}
[Serializable]
public struct AimTransform : IComponent { public Transform Value; }
[Serializable]
public struct AimPrefab : IComponent { public GameObject Value; }