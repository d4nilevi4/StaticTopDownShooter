using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;

namespace NoOpArmy.WiseFeline
{
    public class UtilityAIEditor : EditorWindow
    {
        public static string AssetPath = "Assets/NoOpArmy.WiseFeline/UtilityAI/Editor/UI/";

        AgentBehavior _agentBehavior;
        BehaviorView _behaviorView;
        InspectorView inspectorView;

        [MenuItem("Window/NoOpArmy/Wise Feline")]
        public static void OpenWindow()
        {
            EnsureAssetPathResolved();
            UtilityAIEditor wnd = GetWindow<UtilityAIEditor>();
            wnd.autoRepaintOnSceneChange = true;

            wnd.titleContent = new GUIContent("Wise Feline");
            wnd.minSize = new Vector2(840, 400);
            wnd.Show();
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instancId, int line)
        {
            if (Selection.activeObject is AgentBehavior)
            {
                OpenWindow();
                return true;
            }
            return false;
        }

        private static void EnsureAssetPathResolved()
        {
            string[] guids = AssetDatabase.FindAssets("UtilityAIEditor t:script", null);
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                path = Path.GetDirectoryName(path);
                path = Path.GetDirectoryName(path);
                // ReSharper disable once AssignNullToNotNullAttribute
                AssetPath = Path.Combine(path, "UI/");
            }
        }

        public void CreateGUI()
        {
            EnsureAssetPathResolved();
            var root = rootVisualElement;
            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetPath + "UtilityAIEditor.uxml");
            visualTree.CloneTree(root);

            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetPath + "UtilityAIEditor.uss"));

            _behaviorView = root.Q<BehaviorView>();
            _behaviorView.Init(this);
            inspectorView = root.Q<InspectorView>("inspector-view");

            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;

            OnSelectionChange();
        }

        private void OnPlayModeChanged(PlayModeStateChange playMode)
        {
            switch (playMode)
            {
                case PlayModeStateChange.EnteredEditMode:
                    _behaviorView.ClearView();
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    _behaviorView.ClearView();
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    _behaviorView.ClearView();
                    OnSelectionChange();
                    break;
            }
        }

        private void OnSelectionChange()
        {
            AgentBehavior behavior = null;
            if (Application.isPlaying)
            {
                if (Selection.activeGameObject)
                {
                    if (Selection.activeGameObject.TryGetComponent(out Brain brain))
                        behavior = brain.Behavior;
                    if (behavior != null)
                    {
                        _behaviorView.PlayMode = true;
                        _behaviorView.PopulateActionSets(behavior, brain);
                    }
                }
            }
            else
            {
                behavior = Selection.activeObject as AgentBehavior;
                if (behavior != null)
                {
                    if (AssetDatabase.CanOpenForEdit(behavior, StatusQueryOptions.UseCachedIfPossible))
                    {
                        _behaviorView.PlayMode = false;
                        _behaviorView.PopulateActionSets(behavior);
                    }
                }
                else
                {
                    if (Selection.activeGameObject)
                    {
                        if (Selection.activeGameObject.TryGetComponent(out Brain brain))
                            behavior = brain.Behavior;
                        if (behavior != null)
                        {
                            _behaviorView.PlayMode = false;
                            _behaviorView.PopulateActionSets(behavior);
                        }
                    }
                }
            }

            if (behavior == null)
            {
                _behaviorView.ClearView();
            }
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        }
    }
}
