#define WF_LITE
using System.Collections.Generic;
using Shooter;
using Unity.Profiling;
using Unity.VectorGraphics;
using UnityEngine.LowLevelPhysics2D;

namespace NoOpArmy.WiseFeline
{
    /// <summary>
    /// You should attach this component to any GameObject which wishes to be an AI agent controlled by a set of actions
    /// </summary>
    public class Brain : MonoBehaviour
    {
        /// <summary>
        /// Different action selection modes
        /// </summary>
        public enum ActionSelectionAlgorithm : byte
        {
            /// <summary>
            /// Choose the action with the highest score
            /// </summary>
            HighestScore,
            /// <summary>
            /// Randomly choose from the top N without considering priority
            /// </summary>
            RandomConsideringPriority,
            /// <summary>
            /// Randomly choose from the top N without considering priority so only actions with priorities as high as the top one will be considered
            /// </summary>
            RandomWithoutConsideringPriority,
            /// <summary>
            /// Weighted Randomly choise from the top N without considering priority
            /// </summary>
            WeightedRandomConsideringPriority,
            /// <summary>
            /// Weighted Randomly choose from the top N without considering priority so only actions with priorities as high as the top one will be considered
            /// </summary>
            WeightedRandomWithoutConsideringPriority
        }

#if WF_LITE
        [Header("Selection Mode, Priority and score delta are Ultimate features")]
#endif
        /// <summary>
        /// The algorithm the brain uses for selecting the best action
        /// </summary>
        [Tooltip("The algorithm the brain uses for selecting the best action")]
        public ActionSelectionAlgorithm actionSelectionAlgorithm;

        /// <summary>
        /// The top N items are considered for random action selection and this value is what N is.
        /// </summary>
        [Tooltip("The top N items are considered for random action selection and this value is what N is.")]
        public int itemCountForRandomActionSelection = 5;

        /// <summary>
        /// If the score difference between the chosen action and the currently executing one is higher than this, then the actions changes to
        /// the chosen one
        /// </summary>
        [Tooltip("If the score difference between the chosen action and the currently executing one is higher than this, then the actions changes to the chosen one")]
        public float scoreDeltaForMidExecutionActionChange = 0;

        /// <summary>
        /// Fires when the list of behaviors available to this brain is modified by calling AddBehavior() or RemoveBehavior()
        /// </summary>
        public event Action OnBehaviorListModified;

        /// <summary>
        /// Fires when the executing action changes.
        /// The first parameter is the previous action and the second one is the action we are changing too
        /// If the first parameter is null then either the previous action is fully finished successfully or with a failure
        /// or this is the first action to execute
        /// </summary>
        public event Action<ActionBase, ActionBase> OnCurrentActionChanged;

        /// <summary>
        /// This event gets fired when an action succeeds
        /// </summary>
        public event Action<ActionBase> OnActionSucceeded;

        /// <summary>
        /// This event gets fired when an action fails
        /// </summary>
        public event Action<ActionBase> OnActionFailed;

        /// <summary>
        /// The sets of behavior for an agent
        /// </summary>
        [SerializeField]
        private AgentBehavior _behavior;

        /// <summary>
        /// Gets the currently executing action class
        /// </summary>
        public ActionBase currentAction { get { return _currentAction; } }

        public AgentBehavior Behavior { get { if (_clonedBehavior == null) return _behavior; else return _clonedBehavior; } }
        private AgentBehavior _clonedBehavior;
        private List<ActionData> _actionDataList;

        /// <summary>
        /// Should this brain component automatically think when time passes by based on ThinkDuration and UpdateTargetsDuration
        /// </summary>
        [SerializeField]
        private bool _pauseAction = false;
        private ActionBase _currentAction;
        public int dataRecorderRecordIndex { get; private set; }


        //profiler markers
        private static readonly ProfilerMarker thinkMarker = new(ProfilerCategory.Ai, "WiseFelineThink");
        private static readonly ProfilerMarker getTargetsMarker = new(ProfilerCategory.Ai, "WiseFelineGetTargets");

        public void InitializeBrain(Game.Entity entity)
        {
            if (_behavior == null)
            {
                _behavior = AgentBehavior.GetEmpty();
            }
            _clonedBehavior = _behavior.Clone();
            _actionDataList = new List<ActionData>();

            int count = 0;
            for (int i = 0; i < Behavior.ActionSets.Count; i++)
            {
                count += Behavior.ActionSets[i].Actions.Count;
                for (int j = 0; j < Behavior.ActionSets[i].Actions.Count; j++)
                {
                    ActionBase action = Behavior.ActionSets[i].Actions[j];
                    if (action != null)
                    {
                        action.Initialize(this, entity);
                    }
                }
            }
            _actionDataList = new List<ActionData>(count);
            // RebuildMainList(entity);
            // UpdateActionsTargets(entity);
            // Think(entity);

            _actionDataList.Clear();
            int k = 0;
            for (int i = 0; i < Behavior.ActionSets.Count; i++)
            {
                ActionSet set = Behavior.ActionSets[i];
                for (int j = 0; j < set.Actions.Count; j++, k++)
                {
                    _actionDataList.Add(new ActionData());
                }
            }
        }

        private void RebuildMainList(Game.Entity entity)
        {
            _actionDataList.Clear();
            int k = 0;
            for (int i = 0; i < Behavior.ActionSets.Count; i++)
            {
                ActionSet set = Behavior.ActionSets[i];
                for (int j = 0; j < set.Actions.Count; j++, k++)
                {
                    ActionBase action = set.Actions[j];
                    _actionDataList.Add(new ActionData(set, action, action.GetScore(dataRecorderRecordIndex, entity), action._priority, k));
                }
            }
        }

        private List<ActionScoringData> UpdateActionScores(Game.Entity entity)
        {
            List<ActionScoringData> pad = new(_actionDataList.Count);
            _actionDataList.Clear();
            int k = 0;
            for (int i = 0; i < Behavior.ActionSets.Count; i++)
            {
                ActionSet set = Behavior.ActionSets[i];
                for (int j = 0; j < set.Actions.Count; j++, k++)
                {
                    ActionBase action = set.Actions[j];
                    _actionDataList.Add(new ActionData(set, action, action.GetScore(dataRecorderRecordIndex, entity), action._priority, k));
                    pad.Add(new ActionScoringData(action.Score, action._priority, k));
                }
            }
            return pad;
        }

        public void UpdateBrain(Game.Entity entity)
        {
            if (Behavior == null || _actionDataList.Count == 0)
                return;

            if (!_pauseAction)
            {
                if (_currentAction) _currentAction.UpdateAction(entity);
            }
        }

        /// <summary>
        /// Pauses/Resumes executing the current action.
        /// </summary>
        /// <param name="pause">pauses ecution if true and resumes it if false</param>
        public void PauseExecutingActions(bool pause)
        {
            _pauseAction = pause;
        }

        /// <summary>
        /// Updates the target list of all actions so the updated targets can be used while thinking
        /// </summary>
        public void UpdateActionsTargets(Game.Entity entity)
        {
            getTargetsMarker.Begin();

            for (int i = 0; i < Behavior.ActionSets.Count; i++)
            {
                ActionSet set = Behavior.ActionSets[i];
                for (int j = 0; j < set.Actions.Count; j++)
                {
                    ActionBase action = set.Actions[j];
                    action.UpdateTargetsList(entity);
                }
            }

            getTargetsMarker.End();
        }

        /// <summary>
        /// Calculates all action scores and selects the next action to execute. This might or might not change the currently executing action
        /// </summary>
        public void Think(Game.Entity entity)
        {
            thinkMarker.Begin();
            //if there are no actions
            if (_actionDataList.Count == 0)
                return;

            if (_currentAction != null && !_currentAction._isInterruptable)
            {
                Behavior.OnThinkDone?.Invoke(default);
                return;
            }

            //Update all action scores by evaluating all actions and their considerations
            List<ActionScoringData> scratchPad = UpdateActionScores(entity);
#if WF_LITE
            int maxIndex = GetMaxScoreIndexLite(scratchPad);
#else
            int maxIndex = GetMaxScoreIndex(scratchPad);
            float currentActionScore = (_currentAction != null) ? _currentAction.Score : 0;
            int currentActionPriority = (_currentAction != null) ? _currentAction._priority : 0;
            //check priority and score delta
            maxIndex =
                ((_actionDataList[maxIndex].Priority > currentActionPriority && _actionDataList[maxIndex].Score > 0)
                || (_actionDataList[maxIndex].Priority == currentActionPriority && Mathf.Abs(_actionDataList[maxIndex].Score - currentActionScore) > scoreDeltaForMidExecutionActionChange))
                ? maxIndex : -1;
#endif
            if (maxIndex >= 0)
            {
                if (_currentAction != _actionDataList[maxIndex].Action)//a new action is chosen
                {
                    var prev = _currentAction;
                    if (_currentAction != null)
                        _currentAction.FinishAction(entity, rethink : false);

                    _currentAction = _actionDataList[maxIndex].Action;
                    OnCurrentActionChanged?.Invoke(prev, _currentAction);
                    _currentAction.StartAction(entity);
                }
                Behavior.OnThinkDone?.Invoke(_actionDataList[maxIndex]);
            }
            else
            {
                Behavior.OnThinkDone?.Invoke(default);
            }
            thinkMarker.End();
        }

        private int GetMaxScoreIndexLite(List<ActionScoringData> pad)
        {
            pad.Sort((x, y) =>
            {
                if (x.Score > y.Score)
                    return -1;
                if (y.Score > x.Score)
                    return 1;
                return 0;
            });

            return pad[0].Index;
        }


        internal void ActionSucceeded(ActionBase action, Game.Entity entity)
        {
            OnActionSucceeded?.Invoke(action);
            if (action) action.FinishAction(entity);
            if (_currentAction == action)
            {
                _currentAction = null;
                OnCurrentActionChanged?.Invoke(action, null);
            }
        }

        internal void ActionFailed(ActionBase action, Game.Entity entity)
        {
            OnActionFailed?.Invoke(action);
            if (action) action.FinishAction(entity);
            if (_currentAction == action)
            {
                _currentAction = null;
                OnCurrentActionChanged?.Invoke(action, null);
            }
        }

        /// <summary>
        /// Adds actions inside a behavior agent asset to the actions of this brain
        /// </summary>
        /// <param name="agentBehavior">The agent behavior asset to add</param>
        /// <remarks>
        /// Duplicate actions which already exist in the brain will be added again too.
        /// You should design your behaviors so they don't have duplicate actions
        /// You usually do not need multiple instances of an action in the same brain
        /// </remarks>
        public void AddBehavior(AgentBehavior agentBehavior, Game.Entity entity)
        {
            if (!Application.isPlaying) return;
            if (agentBehavior == null) return;
            if (_clonedBehavior == null)
                throw new Exception(String.Format("Can not combine behavior in {0} because cloned behavior is null.", gameObject.name));
            if (agentBehavior.ActionSets.Count == 0) return;
            int count = 0;
            for (int i = 0; i < agentBehavior.ActionSets.Count; i++)
            {
                count += _clonedBehavior.AddRuntimeActionSet(agentBehavior.ActionSets[i], this, entity);
            }
            RebuildMainList(entity);
            OnBehaviorListModified?.Invoke();
        }

        /// <summary>
        /// Adds an action set to the set of behaviors this brain has available to score and execute
        /// </summary>
        /// <param name="set">The action set to add</param>
        /// <remarks>
        /// Duplicat actions which already exist in the brain will be added again too so you should design your action sets acordingly.
        /// You usually do not need multiple instances of an action in the same brain
        /// </remarks>
        public void AddActionSet(ActionSet set, Game.Entity entity)
        {
            if (!Application.isPlaying) return;
            if (set == null) return;

            var count = _clonedBehavior.AddRuntimeActionSet(set, this, entity);
            RebuildMainList(entity);
            OnBehaviorListModified?.Invoke();
        }

        /// <summary>
        /// Removes actions of a behavior from this brain
        /// </summary>
        /// <param name="agentBehavior"></param>
        public void RemoveBehavior(AgentBehavior agentBehavior, Game.Entity entity)
        {
            if (!Application.isPlaying) return;
            if (agentBehavior == null) return;
            if (_clonedBehavior == null)
                throw new Exception(String.Format("Can not combine behavior in {0} because cloned behavior is null.", gameObject.name));
            if (agentBehavior.ActionSets.Count == 0) return;
            int count = 0;
            foreach (var set in agentBehavior.ActionSets)
            {
                count += _clonedBehavior.RemoveRuntimeActionSet(set);
            }
            RebuildMainList(entity);
            OnBehaviorListModified?.Invoke();
        }

        /// <summary>
        /// Removes a specific action set from the list of behaviors of this brain
        /// </summary>
        /// <param name="set"></param>
        public void RemoveActionSet(ActionSet set, Game.Entity entity)
        {
            if (!Application.isPlaying) return;
            if (set == null) return;

            var count = _clonedBehavior.RemoveRuntimeActionSet(set);
            RebuildMainList(entity);
            OnBehaviorListModified?.Invoke();
        }

        private void OnDestroy()
        {
            // Destroy(Behavior);
        }
    }

    /// <summary>
    /// This struct represents the runtime info for a an action and its final score
    /// </summary>
    public struct ActionData
    {
        /// <summary>
        /// The action set this instance refers to.
        /// </summary>
        public ActionSet ActionSet;

        /// <summary>
        /// Which action this instance refers to.
        /// </summary>
        public ActionBase Action;

        /// <summary>
        /// The score of the action in the last think operation.
        /// </summary>
        public float Score;

        /// <summary>
        /// Priority of the action
        /// </summary>
        public int Priority;

        /// <summary>
        /// index of the action in the main list
        /// </summary>
        public int Index;

        public ActionData(ActionSet set, ActionBase action, float score, int priority, int index)
        {
            ActionSet = set;
            Action = action;
            Score = score;
            Priority = priority;
            Index = index;
        }
    }

    public struct ActionScoringData
    {
        /// <summary>
        /// The score of the action in the last think operation.
        /// </summary>
        public float Score;

        /// <summary>
        /// Priority of the action
        /// </summary>
        public int Priority;

        /// <summary>
        /// index of the action in the main list
        /// </summary>
        public int Index;

        public ActionScoringData(float score, int priority, int index)
        {
            Score = score;
            Priority = priority;
            Index = index;
        }
    }
}
