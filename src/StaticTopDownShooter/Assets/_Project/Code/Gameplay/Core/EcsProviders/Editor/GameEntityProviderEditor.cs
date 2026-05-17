using FFS.Libraries.StaticEcs.Unity.Editor;
using UnityEditor;
using Shooter;

namespace Shooter.Editor {
    [CustomEditor(typeof(GameEntityProvider)), CanEditMultipleObjects]
    public class GameEntityProviderEditor : StaticEcsEntityProviderEditor<GameWT, GameEntityProvider> { }
}
