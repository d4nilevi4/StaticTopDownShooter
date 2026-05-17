using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;
using System;

namespace NoOpArmy.WiseFeline
{
    public class AboutEditor : EditorWindow
    {
        public static void OpenWindow()
        {
            AboutEditor wnd = GetWindow<AboutEditor>();
            wnd.titleContent = new GUIContent("About Wise Feline");
            wnd.Show();
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UtilityAIEditor.AssetPath + "AboutView.uxml");
            visualTree.CloneTree(root);
            
            Label url = root.Q<Label>("url");
            url.RegisterCallback<MouseDownEvent>(OpenURL);
            root.Q<Button>("ok").clicked += CloseWindow;
        }

        private void OpenURL(MouseDownEvent evt)
        {
            Application.OpenURL("https://www.nooparmygames.com/");
        }

        private void CloseWindow()
        {
            this.Close();
        }

        private void OnDisable()
        {

        }
    }
}
