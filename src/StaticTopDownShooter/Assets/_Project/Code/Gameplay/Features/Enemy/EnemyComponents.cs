using NoOpArmy.WiseFeline;

namespace Shooter;

[Serializable] public struct IsEnemy : ITag {}
[Serializable] public struct IsIdling : ITag {}
[Serializable] public struct IsPatrolling : ITag {}
[Serializable] public struct IsMovingToCover : ITag {}
[Serializable] public struct IsAtCover : ITag {}
[Serializable] public struct IsAttacking : ITag {}

[Serializable] public struct IdleTime : IComponent { public float Value; }
[Serializable] public struct IdleTimer : IComponent { public float Value; }
[Serializable] public struct AggressionRange : IComponent { public float Value; }
[Serializable] public struct WalkSpeed : IComponent { public float Value; }
[Serializable] public struct RunSpeed : IComponent { public float Value; }

[Serializable] public struct PatrolPoints : IComponent { public Transform[] Value; }
[Serializable] public struct PatrolPointIndex : IComponent { public int Value; }

[Serializable] public struct CoverPoints : IComponent { public CoverObstacle[] Value; }
[Serializable] public struct CurrentCoverIndex : IComponent { public int Value; }
[Serializable] public struct CurrentCoverWorldPos : IComponent { public Vector2 Value; }

[Serializable] public struct BrainComponent : IComponent { public Brain Value; }

[Serializable] public struct RethinkInterval : IResource { public float Value; }
