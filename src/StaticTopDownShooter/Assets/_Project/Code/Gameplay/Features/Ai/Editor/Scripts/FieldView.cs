using System;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NoOpArmy.WiseFeline
{
    public class FieldView : VisualElement
    {
#pragma warning disable CS0618 // Type or member is obsolete
        public new class UxmlFactory : UxmlFactory<FieldView, UxmlTraits> { }
#pragma warning restore CS0618 // Type or member is obsolete

        public FieldView()
        {
            RegisterCallback<AttachToPanelEvent>(OnAttachToParent);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromParent);
        }

        public AIObject AIObject;
        public System.Type ObjectType;
        public BehaviorView BehaviorView;

        private Button _fButton;
        private Label _fText;
        private Label _fScore;
        private TextField _fTextField;

        private static string clipboardBehaviorAsset;
        private static int clipboardSetIndex;
        private static int clipboardActionIndex;
        private static int clipboardConsiderationIndex;
        private static int clipboardCopyType;//0 = set, 1 = action and 2 = consideration

        public void Init(AIObject aiObject, System.Type type, BehaviorView behaviorView)
        {
            AIObject = aiObject;
            ObjectType = type;
            BehaviorView = behaviorView;

            _fText = this.Q<Label>("label-name");
            _fScore = this.Q<Label>("label-score");
            _fTextField = this.Q<TextField>();
            _fText.text = aiObject.Name;
            _fTextField.SetValueWithoutNotify(aiObject.Name);

            SerializedObject so = new SerializedObject(aiObject);
            SerializedProperty sp = so.FindProperty("Name");
            _fText.BindProperty(sp);

            var clickable1 = new Clickable(OnSingleClick);
            clickable1.activators.Clear();
            clickable1.activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse, clickCount = 1 });

            /*var clickable2 = new Clickable(OnDoubleClick);
            clickable2.activators.Clear();
            clickable2.activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse, clickCount = 2 });*/

            var clickable3 = new Clickable(OnRightClick);
            clickable3.activators.Clear();
            clickable3.activators.Add(new ManipulatorActivationFilter { button = MouseButton.RightMouse, clickCount = 1 });

            _fButton = this.Q<Button>();
            _fButton.clickable = clickable1;

            if (BehaviorView.PlayMode)
            {
                if (type != typeof(ActionSet))
                {
                    _fScore.style.display = DisplayStyle.Flex;
                    SetScore(0f);
                }
            }
            else
            {
                //_fButtton.AddManipulator(clickable2);
                _fButton.AddManipulator(clickable3);
                _fButton.AddManipulator(new DragAndDropManipulator(_fButton, this));
            }

            UpdateClassList(type);
        }

        private void OnAttachToParent(AttachToPanelEvent evt)
        {
            /*if (!BehaviorView.PlayMode)
            {
                //this.AddManipulator(new DragAndDropManipulator(this));
            }*/

        }

        private void OnDetachFromParent(DetachFromPanelEvent evt)
        {

        }

        private void OnSingleClick(EventBase evt) { BehaviorView.FieldSelected(this); }
        private void OnDoubleClick(EventBase evt) { RenameField(); }
        private void OnRightClick(EventBase evt) { ShowContextMenu(evt); }

        /// <summary>
        /// Updates the USS classes of the button based on what type it is being used for.
        /// </summary>
        /// <param name="type"></param>
        private void UpdateClassList(System.Type type)
        {
            if (type == typeof(ActionSet))
                _fButton.AddToClassList("actionset");
            else if (type.IsSubclassOf(typeof(ActionBase)))
                _fButton.AddToClassList("action");
            else if (type.IsSubclassOf(typeof(ConsiderationBase)))
                _fButton.AddToClassList("consideration");
        }

        private void RenameField()
        {
            _fTextField.style.display = DisplayStyle.Flex;
            _fTextField.RegisterValueChangedCallback(x => FieldNewName(x.newValue));
            _fTextField.Q(TextField.textInputUssName).RegisterCallback<KeyDownEvent>(e =>
            {
                if (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter)
                    FinishRename();
            });
        }

        private void FieldNewName(string newName)
        {
            if (!string.IsNullOrEmpty(newName))
            {
                _fText.text = newName;
                AIObject.Name = newName;
            }
        }

        public void FinishRename()
        {
            _fTextField.style.display = DisplayStyle.None;
        }

        private void ShowContextMenu(EventBase evt)
        {
            var menu = new GenericMenu();

            bool isSelected = false;
            menu.AddItem(new GUIContent("Duplicate"), isSelected, () => DuplicateField());
            menu.AddItem(new GUIContent("Copy"), isSelected, () => CopyField());
            menu.AddItem(new GUIContent("Paste"), isSelected, () => PasteField());
            menu.AddItem(new GUIContent("Delete"), isSelected, () => DeleteField());


            // Get position of menu on top of target element.
            var menuPosition = evt.originalMousePosition;
            var menuRect = new Rect(menuPosition, Vector2.zero);

            menu.DropDown(menuRect);
        }

        private void DeleteField()
        {
            BehaviorView.DeleteField(this);
        }

        private void DuplicateField()
        {
            BehaviorView.DuplicateField(this.AIObject);
        }

        private void CopyField()
        {
            BehaviorView.FieldSelected(this);
            AgentBehavior bhv = BehaviorView.GetBehavior();
            clipboardBehaviorAsset = AssetDatabase.GetAssetPath(bhv);
            (int setIndex, int actionIndex, int considerationIndex) indices = BehaviorView.GetSelectedIndices(this.AIObject);
            clipboardSetIndex = indices.setIndex;
            clipboardActionIndex = indices.actionIndex;
            clipboardConsiderationIndex = indices.considerationIndex;
            clipboardCopyType = GetCopyType();
        }

        private int GetCopyType()
        {
            if (AIObject is ActionSet)
                return 0;
            if (AIObject is ActionBase)
                return 1;
            if (AIObject is ConsiderationBase)
                return 2;
            throw new InvalidOperationException($"The AIObject's type {AIObject.GetType().Name} is not valid");
        }

        private void PasteField()
        {
            if (!string.IsNullOrEmpty(clipboardBehaviorAsset))
            {
                AgentBehavior bhv = AssetDatabase.LoadAssetAtPath<AgentBehavior>(clipboardBehaviorAsset);
                if (bhv != null)
                {
                    if (clipboardCopyType == 0)
                        BehaviorView.DuplicateField(bhv.ActionSets[clipboardSetIndex]);
                    else if (clipboardCopyType == 1)
                        BehaviorView.DuplicateField(bhv.ActionSets[clipboardSetIndex].Actions[clipboardActionIndex]);
                    else if (clipboardCopyType == 2)
                        BehaviorView.DuplicateField(bhv.ActionSets[clipboardSetIndex].Actions[clipboardActionIndex].Considerations[clipboardConsiderationIndex]);
                }
            }
        }

        public void UpdateScore(ActionData actionData)
        {
            if (AIObject is ConsiderationBase)
            {
                var cons = AIObject as ConsiderationBase;
                if (cons != null)
                    SetScore(cons.Score);
            }
            else if (AIObject is ActionBase)
            {
                var action = AIObject as ActionBase;
                if (action != null)
                    SetScore(action.Score);

                if (actionData.Action == action)
                    _fButton.AddToClassList("running");
                else
                    _fButton.RemoveFromClassList("running");
            }
            else if (AIObject is ActionSet)
            {
                if (actionData.ActionSet == AIObject)
                    _fButton.AddToClassList("running");
                else
                    _fButton.RemoveFromClassList("running");
            }
        }

        private void SetScore(float score)
        {
            _fScore.text = score.ToString();
        }
    }
}
