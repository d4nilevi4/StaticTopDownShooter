using FFS.Libraries.StaticEcs.Unity.Editor;
using UnityEditor;
using Shooter;

namespace Shooter.Editor {
    [CustomEditor(typeof(GameEventProvider)), CanEditMultipleObjects]
    public class GameEventProviderEditor : StaticEcsEvenTEntityProviderEditor<GameWT, GameEventProvider> { }
}
