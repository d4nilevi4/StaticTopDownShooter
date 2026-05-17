namespace Shooter;

[Serializable] public struct TransformComponent : IComponent { public Transform Value; }

[Serializable] public struct OwnerGID : IComponent { public EntityGID Value; }