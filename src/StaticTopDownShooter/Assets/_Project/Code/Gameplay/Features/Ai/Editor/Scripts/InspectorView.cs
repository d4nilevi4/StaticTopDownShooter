using UnityEngine.UIElements;
using UnityEditor;

namespace NoOpArmy.WiseFeline
{
    public class InspectorView : VisualElement
    {
#pragma warning disable CS0618 // Type or member is obsolete
        public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }
#pragma warning restore CS0618 // Type or member is obsolete
        Editor editor;

        public InspectorView()
        {

        }

        public void UpdateSelection(UnityEngine.Object uObject)
        {
            Clear();

            UnityEngine.Object.DestroyImmediate(editor);
            editor = Editor.CreateEditor(uObject);
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (editor.target)
                    editor.OnInspectorGUI();
            });

            Add(container);
        }
    }
}
