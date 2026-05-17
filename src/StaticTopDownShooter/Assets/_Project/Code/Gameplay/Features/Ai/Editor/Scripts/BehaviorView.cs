using UnityEditor.Experimental.GraphView;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NoOpArmy.WiseFeline
{
    public class BehaviorView : VisualElement
    {
#pragma warning disable CS0618 // Type or member is obsolete
        public new class UxmlFactory : UxmlFactory<BehaviorView, UxmlTraits> { }
#pragma warning restore CS0618 // Type or member is obsolete
        public BehaviorView()
        {
        }

        private const int FieldHeight = 24;

        public UtilityAIEditor window;

        private AgentBehavior _behavior;
        private Brain _brain;

        private Label _title;

        private ScrollView _actionSetList;
        private ScrollView _actionList;
        private ScrollView _considerationList;

        private Button _actionSetButton;
        private Button _actionButton;
        private Button _considerationButton;

        private InspectorView _inspector;
        private ToolbarToggle _actionSortToggle;

        private List<FieldView> _actionSetViews;
        private List<FieldView> _actionViews;
        private List<FieldView> _considerationViews;
        private List<FieldView> _sortedActions;

        private ActionSet _currentActionSet;
        private ActionBase _currentAction;
        private ConsiderationBase _currentConsideration;

        private string selectedActionSetBeforePopulate;
        private string selectedActionBeforePopulate;
        private string selectedConsiderationBeforePopulate;
        public bool PlayMode
        {
            get { return _playMode; }
            internal set
            {
                if (_playMode != value)
                {
                    _playMode = value;
                    _inspector.SetEnabled(!value);
                }
            }
        }
        private bool _playMode;

        public void Init(UtilityAIEditor window)
        {
            this.window = window;

            _title = this.Q<Label>("title");
            _actionSetList = this.Q<ScrollView>("actionset-container");
            _actionList = this.Q<ScrollView>("actions-container");
            _considerationList = this.Q<ScrollView>("consideration-container");

            _actionSetButton = this.Q<Button>("add-actionset");
            _actionButton = this.Q<Button>("add-action");
            _considerationButton = this.Q<Button>("add-consideration");

            _inspector = this.Q<InspectorView>();
            _actionSortToggle = this.Q<ToolbarToggle>("sort-toggle");
            this.Q<ToolbarButton>("about").clicked += ShowAbout;
            this.Q<ToolbarButton>("documentation").clicked += OpenDocumentation;
            this.Q<ToolbarButton>("discord").clicked += OpenDiscord;

            _actionSetButton.clicked -= CreateActionSet;
            _actionSetButton.clicked += CreateActionSet;

            _actionButton.clicked -= ShowActionList;
            _actionButton.clicked += ShowActionList;

            _considerationButton.clicked -= ShowConsiderationList;
            _considerationButton.clicked += ShowConsiderationList;

            Undo.undoRedoPerformed += OnUndoRedo;
            RegisterCallback<DetachFromPanelEvent>(c => { Undo.undoRedoPerformed -= OnUndoRedo; });

            _actionSetViews = new List<FieldView>();
            _actionViews = new List<FieldView>();
            _considerationViews = new List<FieldView>();
            _sortedActions = new List<FieldView>();
        }

        private void ShowAbout()
        {
            AboutEditor.OpenWindow();
        }

        private void OpenDocumentation()
        {
            Application.OpenURL("https://www.nooparmygames.com/WF-UtilityAI-Unity/");
        }

        private void OpenDiscord()
        {
            Application.OpenURL("https://discord.gg/FA8R7APZWR");
        }

        private void OnUndoRedo()
        {
            if (_behavior != null)
            {
                PopulateActionSets(_behavior);
                AssetDatabase.SaveAssets();
            }

        }

        public void PopulateActionSets(AgentBehavior behavior, Brain brain = null)
        {
            if (_currentActionSet != null)
                selectedActionSetBeforePopulate = _currentActionSet.Name;
            if (_currentAction != null)
                selectedActionBeforePopulate = _currentAction.Name;
            if (_currentConsideration != null)
                selectedConsiderationBeforePopulate = _currentConsideration.Name;

            ClearView();
            _behavior = behavior;
            _behavior.DeleteNulls();

            if (brain != null)
            {
                _brain = brain;
                _brain.OnBehaviorListModified -= OnBehaviorModified;
                _brain.OnBehaviorListModified += OnBehaviorModified;
            }

            _title.text = _behavior.name;
            _actionSetViews = new List<FieldView>();
            behavior.ActionSets.ForEach(a =>
            {
                if (a != null)
                    _actionSetViews.Add(CreateFieldView(a, _actionSetList));
            });
            _actionSetList.style.height = _actionSetViews.Count * FieldHeight;
            UpdateSelectedActionSet();

            UpdateButtonsState();
            //UpdateSelected();

            if (PlayMode)
            {
                _behavior.OnThinkDone -= UpdateScores;
                _behavior.OnThinkDone += UpdateScores;
            }

            /*EditorUtility.SetDirty(behavior);
            AssetDatabase.SaveAssets();*/
        }

        private void UpdateSelectedActionSet()
        {
            if (!string.IsNullOrEmpty(selectedActionSetBeforePopulate))
            {
                foreach (var actionSet in _actionSetViews)
                {
                    if (actionSet.AIObject.Name == selectedActionSetBeforePopulate)
                    {
                        UpdateViewBasedOnSelectedField(actionSet);
                        return;
                    }
                }
            }

            if (_actionSetViews.Count != 0)
                UpdateViewBasedOnSelectedField(_actionSetViews[0]);

        }

        private void UpdateSelectedAction()
        {
            if (!string.IsNullOrEmpty(selectedActionBeforePopulate))
            {
                foreach (var action in _actionViews)
                {
                    if (action.AIObject.Name == selectedActionBeforePopulate)
                    {
                        UpdateViewBasedOnSelectedField(action);
                        return;
                    }
                }
            }

            if (_actionViews.Count != 0)
                UpdateViewBasedOnSelectedField(_actionViews[0]);

        }

        private void UpdateSelectedConsideration()
        {
            if (!string.IsNullOrEmpty(selectedConsiderationBeforePopulate))
            {
                foreach (var con in _considerationViews)
                {
                    if (con.AIObject.Name == selectedConsiderationBeforePopulate)
                    {
                        UpdateViewBasedOnSelectedField(con);
                        return;
                    }
                }

                if (_considerationViews.Count != 0)
                    UpdateViewBasedOnSelectedField(_considerationViews[0]);
            }
        }

        private void PopulateActions()
        {
            _actionList.Clear();
            _actionViews = new List<FieldView>();
            if (_currentActionSet != null)
            {
                _currentActionSet.Actions.ForEach(a =>
                {
                    if (a != null)
                        _actionViews.Add(CreateFieldView(a, _actionList));
                });
                _actionList.style.height = _actionViews.Count * FieldHeight;
                if (_actionViews.Count != 0)
                    UpdateSelectedAction();
                else
                {
                    _considerationList.Clear();
                    _currentAction = null;
                    _currentConsideration = null;
                }
            }
            UpdateButtonsState();
        }

        private void PopulateConsiderations()
        {
            _considerationList.Clear();
            _considerationViews = new List<FieldView>();
            if (_currentAction != null)
            {
                _currentAction.Considerations.ForEach(c =>
                {
                    if (c != null)
                        _considerationViews.Add(CreateFieldView(c, _considerationList));
                });
                _considerationList.style.height = _considerationViews.Count * FieldHeight;
                if (_considerationViews.Count != 0)
                    UpdateSelectedConsideration();
            }
        }

        public void ClearView()
        {
            if (_behavior != null)
                _behavior.OnThinkDone -= UpdateScores;
            _behavior = null;
            if (_brain != null)
                _brain.OnBehaviorListModified -= OnBehaviorModified;
            _brain = null;
            _title.text = "None";
            _actionSetList.Clear();
            _actionList.Clear();
            _considerationList.Clear();
            _currentActionSet = null;
            _currentAction = null;
            _currentConsideration = null;

            UpdateButtonsState();
        }

        public void OnBehaviorModified()
        {
            PopulateActionSets(_behavior, _brain);
        }

        private void CreateActionSet()
        {
            ActionSet set = _behavior.AddActionSet("New Action Set");
            FieldView setView = CreateFieldView(set, _actionSetList);
            _actionSetViews.Add(setView);
            _actionSetList.style.height = _actionSetViews.Count * FieldHeight;
            selectedConsiderationBeforePopulate = "";
            selectedActionBeforePopulate = "";
            UpdateViewBasedOnSelectedField(setView);
            UpdateButtonsState();
        }

        private void CopyActionSet(ActionSet oldset)
        {
            ActionSet set = oldset.Clone(_behavior);
            _behavior.AddCopiedActionSet(set);
            FieldView setView = CreateFieldView(set, _actionSetList);
            _actionSetViews.Add(setView);
            _actionSetList.style.height = _actionSetViews.Count * FieldHeight;
            selectedConsiderationBeforePopulate = "";
            selectedActionBeforePopulate = "";
            UpdateViewBasedOnSelectedField(setView);
            UpdateButtonsState();
        }

        private void ShowActionList()
        {
            CreateSearchableWindow(typeof(ActionBase), CreateAction, 300, 200);
        }

        private void CreateAction(object menuItem)
        {
            ActionBase action = _behavior.AddAction(menuItem as Type, _currentActionSet);
            FieldView actionView = CreateFieldView(action, _actionList);
            _actionViews.Add(actionView);
            _actionList.style.height = _actionViews.Count * FieldHeight;
            selectedConsiderationBeforePopulate = "";
            UpdateViewBasedOnSelectedField(actionView);
            UpdateButtonsState();
        }

        private void CopyAction(ActionBase oldAction)
        {
            ActionBase action = oldAction.Clone(_currentActionSet);
            _behavior.AddCopiedAction(action, _currentActionSet);
            FieldView actionView = CreateFieldView(action, _actionList);
            _actionViews.Add(actionView);
            _actionList.style.height = _actionViews.Count * FieldHeight;
            selectedConsiderationBeforePopulate = "";
            UpdateViewBasedOnSelectedField(actionView);
            UpdateButtonsState();
        }

        private void ShowConsiderationList()
        {
            CreateSearchableWindow(typeof(ConsiderationBase), CreateConsideration, 300, 200);
        }

        private void CreateSearchableWindow(Type baseType, Action<Type> callback, int width, int height)
        {
            var prov = ScriptableObject.CreateInstance<AITypesSearchProvider>();
            prov.baseType = baseType;
            prov.OnTypeSelected = callback;
            SearchWindow.Open<AITypesSearchProvider>(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition), width, height), prov);
        }

        private void CreateConsideration(object menuItem)
        {
            ConsiderationBase consideration = _behavior.AddConsideration(menuItem as Type, _currentAction);
            FieldView consView = CreateFieldView(consideration, _considerationList);
            _considerationViews.Add(consView);
            _considerationList.style.height = _considerationViews.Count * FieldHeight;
            UpdateViewBasedOnSelectedField(consView);
        }

        private void CopyConsideration(ConsiderationBase con)
        {
            var clone = con.Clone();
            _behavior.AddCopiedConsideration(clone, _currentAction);
            ConsiderationBase consideration = clone;
            FieldView consView = CreateFieldView(consideration, _considerationList);
            _considerationViews.Add(consView);
            _considerationList.style.height = _considerationViews.Count * FieldHeight;
            UpdateViewBasedOnSelectedField(consView);
        }

        private FieldView CreateFieldView(AIObject aiObject, ScrollView parent)
        {
            if (aiObject == null)
                return null;

            var socket = new VisualElement();
            socket.AddToClassList("socket");
            socket.style.height = FieldHeight;

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UtilityAIEditor.AssetPath + "FieldView.uxml");
            visualTree.CloneTree(socket);
            var fieldView = socket.Q<FieldView>();
            parent.Add(socket);
            fieldView.Init(aiObject, aiObject.GetType(), this);
            return fieldView;
        }

        public void FieldSelected(FieldView fieldView)
        {
            if (fieldView.ObjectType.IsSubclassOf(typeof(ActionSet)))
            {
                selectedActionBeforePopulate = "";
                selectedConsiderationBeforePopulate = "";
            }
            else if (fieldView.ObjectType.IsSubclassOf(typeof(ActionBase)))
            {
                selectedConsiderationBeforePopulate = "";
            }

            UpdateViewBasedOnSelectedField(fieldView);
        }

        public void UpdateViewBasedOnSelectedField(FieldView fieldView)
        {
            UpdateInspector(fieldView);
            UpdateViews(fieldView);
            //FinishRenames(fieldView);
        }

        public void DeleteField(FieldView fieldView)
        {
            _behavior.DeleteAsset(fieldView.AIObject);
            PopulateActionSets(_behavior);
        }

        public AgentBehavior GetBehavior()
        {
            return _behavior;
        }

        public (int setIndex,int actionIndex,int considerationIndex) GetSelectedIndices(AIObject currentObject)
        {
            int setIndex = -1;
            int actionIndex = -1;
            int considerationIndex = -1;

            for (int i = 0; i < _actionSetViews.Count; ++i)
            {
                if (_actionSetViews[i].AIObject == _currentActionSet)
                {
                    setIndex = i;
                    break;
                }
            }
            for (int i = 0; i < _actionViews.Count; ++i)
            {
                if (_actionViews[i].AIObject == _currentAction)
                {
                    actionIndex = i;
                    break;
                }
            }
            for (int i = 0; i < _considerationViews.Count; ++i)
            {
                if (_considerationViews[i].AIObject == _currentConsideration)
                {
                    considerationIndex = i;
                    break;
                }
            }
            return (setIndex, actionIndex, considerationIndex);
        }

        public void DuplicateField(AIObject o)
        {
            if (o is ActionSet)
            {
                CopyActionSet(o as ActionSet);
            }
            else if (o.GetType().IsSubclassOf(typeof(ActionBase)))
            {
                CopyAction(o as ActionBase);
            }
            else if (o.GetType().IsSubclassOf(typeof(ConsiderationBase)))
            {
                CopyConsideration(o as ConsiderationBase);
            }
            PopulateActionSets(_behavior);
        }

        internal void SwapObjects(AIObject obj1, AIObject obj2)
        {
            Undo.IncrementCurrentGroup();
            if (obj1 is ConsiderationBase)
            {
                var action = _behavior.GetAction(obj1 as ConsiderationBase);
                Undo.RecordObject(action, "Swap Considerations");
                action.Considerations.Swap(obj1 as ConsiderationBase, obj2 as ConsiderationBase);
            }
            else if (obj1 is ActionBase)
            {
                var set = _behavior.GetActionSet(obj1 as ActionBase);
                Undo.RecordObject(set, "Swap Actions");
                set.Actions.Swap(obj1 as ActionBase, obj2 as ActionBase);
            }
            else
            {
                Undo.RecordObject(_behavior, "Swap ActionSets");
                _behavior.ActionSets.Swap(obj1 as ActionSet, obj2 as ActionSet);
            }
            AssetDatabase.SaveAssets();
        }

        private void UpdateViews(FieldView fieldView)
        {
            if (fieldView.ObjectType == typeof(ActionSet))
            {
                if (_currentActionSet == fieldView.AIObject as ActionSet)
                    return;

                _currentActionSet = fieldView.AIObject as ActionSet;
                _actionSetViews.ForEach(a =>
                {
                    if (a == fieldView)
                        a.AddToClassList("selected");
                    else
                        a.RemoveFromClassList("selected");
                });

                PopulateActions();
            }
            else if (fieldView.ObjectType.IsSubclassOf(typeof(ActionBase)))
            {
                if (_currentAction == fieldView.AIObject as ActionBase)
                    return;

                _currentAction = fieldView.AIObject as ActionBase;
                _actionViews.ForEach(a =>
                {
                    if (a == fieldView)
                        a.AddToClassList("selected");
                    else
                        a.RemoveFromClassList("selected");
                });
                PopulateConsiderations();
            }
            else if (fieldView.ObjectType.IsSubclassOf(typeof(ConsiderationBase)))
            {
                if (_currentConsideration == fieldView.AIObject as ConsiderationBase)
                    return;

                _currentConsideration = fieldView.AIObject as ConsiderationBase;
                _considerationViews.ForEach(c =>
                {
                    if (c == fieldView)
                        c.AddToClassList("selected");
                    else
                        c.RemoveFromClassList("selected");
                });
            }

            UpdateScores(lastActionData);
        }

        private void UpdateInspector(FieldView fieldView)
        {
            if (fieldView != null)
            {
                if (fieldView.AIObject != null)
                    _inspector.UpdateSelection(fieldView.AIObject);
            }
        }

        private void FinishRenames(FieldView fieldView)
        {
            foreach (var field in _actionSetViews)
            {
                if (field != fieldView)
                    field.FinishRename();
            }

            foreach (var field in _actionViews)
            {
                if (field != fieldView)
                    field.FinishRename();
            }

            foreach (var field in _considerationViews)
            {
                if (field != fieldView)
                    field.FinishRename();
            }
        }

        private void UpdateButtonsState()
        {
            if (_behavior == null || PlayMode)
            {
                _actionSetButton.SetEnabled(false);
                _actionButton.SetEnabled(false);
                _considerationButton.SetEnabled(false);
                return;
            }

            if (!_actionSetButton.enabledSelf)
                _actionSetButton.SetEnabled(true);

            if (_currentActionSet == null)
            {
                _actionButton.SetEnabled(false);
                _considerationButton.SetEnabled(false);
                return;
            }
            else if (!_actionButton.enabledSelf)
                _actionButton.SetEnabled(true);

            if (_currentAction == null)
                _considerationButton.SetEnabled(false);
            else if (!_considerationButton.enabledSelf)
                _considerationButton.SetEnabled(true);
        }
        private ActionData lastActionData;
        internal void UpdateScores(ActionData actionData)
        {
            lastActionData = actionData;
            foreach (var set in _actionSetViews)
            {
                set.UpdateScore(actionData);
            }
            foreach (var action in _actionViews)
            {
                action.UpdateScore(actionData);
            }
            foreach (var cons in _considerationViews)
            {
                cons.UpdateScore(actionData);
            }

            if (_actionSortToggle.value)
                ReorderActions();
        }

        internal void ReorderActions()
        {
            _sortedActions = _actionViews.OrderByDescending(a => (a.AIObject as ActionBase).Score).ThenBy(a => (a.AIObject as ActionBase).name).ToList();
            for (int i = 0; i < _sortedActions.Count; i++)
            {
                if (i == 0)
                    _sortedActions[i].parent.SendToBack();
                else if (i == _sortedActions.Count - 1)
                    _sortedActions[i].parent.BringToFront();
                else
                {
                    _sortedActions[i].parent.PlaceInFront(_sortedActions[i - 1].parent);
                }
            }
        }
    }
}
