namespace Shooter;

[Serializable] public struct AmmoCount : IComponent { public int Value; }

[Serializable] public struct IsBullet : ITag {}
[Serializable] public struct BulletPrefab : IComponent { public GameEntityProvider Value; }
[Serializable] public struct WeaponRecoilImpulse : IComponent { public float Value; }
[Serializable] public struct AttackTargetPosition : IComponent { public Vector2 Value; }

[Serializable] public struct ShootPerSec :  IComponent { public float Value; }
[Serializable] public struct ShootAvailable :  ITag { }
[Serializable] public struct ShootCooldownTimer :  IComponent { public float Value; }
