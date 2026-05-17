namespace Shooter;

[Serializable]
public struct CollisionEnterEvent : IEvent
{
    public EntityGID Source;
    public Collision2D Collision;
}

[Serializable]
public struct CollisionStayEvent : IEvent
{
    public EntityGID Source;
    public Collision2D Collision;
}

[Serializable]
public struct CollisionExitEvent : IEvent
{
    public EntityGID Source;
    public Collision2D Collision;
}

[Serializable]
public struct TriggerEnterEvent : IEvent
{
    public EntityGID Source;
    public int OtherColliderId;
}

[Serializable]
public struct TriggerStayEvent : IEvent
{
    public EntityGID Source;
    public int OtherColliderId;
}

[Serializable]
public struct TriggerExitEvent : IEvent
{
    public EntityGID Source;
    public int OtherColliderId;
}