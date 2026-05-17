using FFS.Libraries.StaticEcs.Unity.Editor;
using UnityEditor;
using Shooter;

namespace Shooter.Editor {
    public class GameEcsView : StaticEcsView<GameWT, GameEntityProvider, GameEventProvider> {
        [MenuItem("Window/Game ECS")]
        public static void OpenWindow() {
            var window = GetWindow<GameEcsView>();
            window.Show();
            window.Focus();
        }
    }
}
