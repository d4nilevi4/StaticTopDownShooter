namespace Shooter;

[Serializable]
public struct PlayerInputMap : IResource { public PlayerInputActions Value; }

[Serializable]
public struct MoveInput : IComponent { public Vector2 Value; }

[Serializable]
public struct DodgeActionPerformed : IEvent
{
    public DodgeActionPerformed(EntityGID inputOwner)
    {
        InputOwner = inputOwner;
    }

    public EntityGID InputOwner;
}

[Serializable]
public struct AttackActionPerformed : IEvent
{
    public AttackActionPerformed(EntityGID inputOwner)
    {
        InputOwner = inputOwner;
    }

    public EntityGID InputOwner;
}

[Serializable]
public struct MousePosition : IComponent { public Vector2 Value; }