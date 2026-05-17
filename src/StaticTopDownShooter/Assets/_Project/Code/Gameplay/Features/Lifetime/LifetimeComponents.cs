namespace Shooter;

[Serializable]
public struct Hp : IComponent
{
    public float Current;
    public float Max;
}

[Serializable]
public struct DamageEvent : IEvent
{
    public EntityGID Source;
    public EntityGID Target;
    public float Value;
}

[Serializable]
public struct DeadEvent : IEvent
{
    public EntityGID Target;
}
