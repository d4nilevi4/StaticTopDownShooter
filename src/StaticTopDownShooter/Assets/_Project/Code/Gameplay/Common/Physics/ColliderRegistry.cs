using System.Collections.Generic;

namespace Shooter;

public static class ColliderRegistry
{
    public static Dictionary<int, EntityGID> EntityColliders;

    public static void Initialize()
    {
        EntityColliders = new();
    }

    public static void RegisterColliderId(int colliderId, EntityGID entityGid)
    {
        EntityColliders[colliderId] = entityGid;
    }

    public static EntityGID GetEntityGid(int colliderId)
    {
        return EntityColliders.TryGetValue(colliderId, out EntityGID gid)
            ? gid
            : new EntityGID();
    }
}