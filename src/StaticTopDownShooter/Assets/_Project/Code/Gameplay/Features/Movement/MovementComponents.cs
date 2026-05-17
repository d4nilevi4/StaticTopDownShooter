namespace Shooter;

[Serializable] public struct Speed : IComponent { public float Value; }
[Serializable] public struct RB : IComponent { public Rigidbody2D Value; }

[Serializable] public struct CanDash : ITag { }
[Serializable] public struct DashImpulse : IComponent { public float Value; }

[Serializable] public struct MoveDirection : IComponent { public Vector2 Value; }
[Serializable] public struct MoveImpulse : IComponent { public Vector2 Value; }
