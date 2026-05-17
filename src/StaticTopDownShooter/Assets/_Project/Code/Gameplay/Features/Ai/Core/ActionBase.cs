using System.Collections.Generic;
using Shooter;

namespace NoOpArmy.WiseFeline
{
    /// <summary>
    /// AI actions should derive from this class to define a custom action which can be added to the set of actions for an agent
    /// </summary>
    public abstract class ActionBase : AIObject
    {
        /// <summary>
        /// This is the priority of the action. Actions will be chosen for execution if their score is non-zero even if actions with lower priorities have higher scores
        /// </summary>
        public int _priority = 0;

        /// <summary>
        /// Is this action interruptable mid-execution by another action which has a higher score
        /// </summary>
        public bool _isInterruptable = true;

        /// <summary>
        /// The action's score will be multiplied by this value so you can increase/decrease the score of an action by moving this value away from 1.
        /// </summary>
        [Header("Generic")]
        [SerializeField, Range(0, 10)]
        [Tooltip(
            "The action's score will be multiplied by this value so you can increase/decrease the score of an action by moving this value away from 1.")]
        private float _weight = 1f;

        /// <summary>
        /// Maximum number of targets which this action should consider
        /// </summary>
        [SerializeField] protected int _maxTargetCount = 5;

        /// <summary>
        /// Should the action add a 25% score bonus to the current target to not change the target multiple times too quickly when scores are too close.
        /// </summary>
        [SerializeField] protected bool _useMomentumOnTarget;

        /// <summary>
        /// List of the considerations for the action.
        /// The score of the action is the result of multiplication of the scores of all of these considerations
        /// </summary>
        public List<ConsiderationBase> Considerations
        {
            get
            {
                _considerations ??= new List<ConsiderationBase>();
                return _considerations;
            }
        }

        /// <summary>
        /// List of all considerations
        /// </summary>
        [SerializeField, HideInInspector] private List<ConsiderationBase> _considerations;

        /// <summary>
        /// List of the considerations which work on the action's target
        /// </summary>
        private ConsiderationBase[] _targetedConsiderations;

        /// <summary>
        /// List of the considerations which work on the action's agent itself.
        /// </summary>
        private ConsiderationBase[] _selfConsiderations;

        /// <summary>
        /// The brain component which the action is executing for.
        /// </summary>
        protected Brain Brain { get; private set; }

        /// <summary>
        /// Is the action initialized?
        /// </summary>
        public bool IsInitialized { get; protected set; }

        /// <summary>
        /// The last calculated score of the action in the last think operation.
        /// </summary>
        public float Score
        {
            get { return _score; }
        }

        /// <summary>
        /// The wait of the action which is multiplied by the score to allow you to prioritize some actions
        /// </summary>
        public float Weight
        {
            get { return _weight; }
        }

        /// <summary>
        /// tracks all targets and their scores with the list below it
        /// </summary>
        private List<Component> TargetsScoresList1;

        /// <summary>
        /// tracks all targets and their scores with the list above it
        /// </summary>
        private List<float> TargetsScoresList2;

        /// <summary>
        /// The target with the best score.
        /// The type of this component depends on the type that you yourself store in the list in the gets callback.
        /// If the action doesn't have any targets and target based considerations then this field doesn't matter and it value should be considered undefined.
        /// </summary>
        public Component ChosenTarget;

        private float _score;
        private float _compensationFactor;
        private int _considerationCount;
        private readonly float _momentum = 0.25f;

        /// <summary>
        /// Adds a component to the targets list of the action
        /// </summary>
        /// <param name="t"></param>
        protected void AddTarget(Component t)
        {
            TargetsScoresList1.Add(t);
            TargetsScoresList2.Add(1);
        }


        /// <summary>
        /// Adds an array of targets to the list of targets for the action
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targets"></param>
        protected void AddTargets<T>(T[] targets) where T : Component
        {
            TargetsScoresList1.AddRange(targets);
            for (int i = 0; i < targets.Length; i++)
            {
                TargetsScoresList2.Add(1);
            }
        }

        /// <summary>
        /// Adds an array of targets to the list of targets for the action
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targets"></param>
        protected void AddTargets<T>(List<T> targets) where T : Component
        {
            TargetsScoresList1.AddRange(targets);
            for (int i = 0; i < targets.Count; i++)
                TargetsScoresList2.Add(1);
        }

        /// <summary>
        /// Clears the list of targets for the action
        /// </summary>
        protected void ClearTargets()
        {
            TargetsScoresList1.Clear();
            TargetsScoresList2.Clear();
        }

        /// <summary>
        /// Removes a component from the list of targets for the action
        /// </summary>
        /// <param name="t"></param>
        protected void RemoveTarget(Component t)
        {
            int index = TargetsScoresList1.IndexOf(t);
            if (index >= 0)
            {
                TargetsScoresList1.RemoveAt(index);
                TargetsScoresList2.RemoveAt(index);
            }
        }


        /// <summary>
        /// Clones the action for execution at runtime.
        /// </summary>
        /// <param name="set"></param>
        /// <param name="wiseAIFactory"></param>
        /// <returns></returns>
        public ActionBase Clone(ActionSet set)
        {
            ActionBase action = Object.Instantiate(this);
            for (int i = 0; i < action.Considerations.Count; i++)
            {
                action.Considerations[i] = action.Considerations[i].Clone();
            }

            return action;
        }

        /// <summary>
        /// Initializes the action.
        /// You don't need to call this. The Brain component does this automatically
        /// </summary>
        /// <param name="brain"></param>
        internal void Initialize(Brain brain, Game.Entity entity)
        {
            Brain = brain;
            IsInitialized = true;

            TargetsScoresList1 = new List<Component>();
            TargetsScoresList2 = new List<float>();

            InitializeConsiderations(entity);
            OnInitialized(entity);
        }

        /// <summary>
        /// Initializes the considerations of the action
        /// </summary>
        private void InitializeConsiderations(Game.Entity entity)
        {
            List<ConsiderationBase> targetedCons = new();
            List<ConsiderationBase> selfCons = new();
            for (int i = 0; i < Considerations.Count; i++)
            {
                if (_considerations[i] != null)
                {
                    if (_considerations[i].NeedTarget)
                        targetedCons.Add(_considerations[i]);
                    else
                        selfCons.Add(_considerations[i]);
                }
            }

            _targetedConsiderations = targetedCons.ToArray();
            _selfConsiderations = selfCons.ToArray();


            for (int i = 0; i < _selfConsiderations.Length; i++)
            {
                _selfConsiderations[i].Initialize(Brain, entity);
            }

            for (int i = 0; i < _targetedConsiderations.Length; i++)
            {
                _targetedConsiderations[i].Initialize(Brain, entity);
            }
        }

        /// <summary>
        /// Called when the action is initialized
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void OnInitialized(Game.Entity entity)
        {
        }


        internal void StartAction(Game.Entity entity)
        {
            OnStart(entity);
        }

        /// <summary>
        /// Should be used like MonoBehaviour's Start for the action
        /// </summary>
        protected virtual void OnStart(Game.Entity entity)
        {
        }

        internal void UpdateAction(Game.Entity entity)
        {
            OnUpdate(entity);
        }

        /// <summary>
        /// Should be used like MonoBehaviour's Update for the action
        /// </summary>
        protected virtual void OnUpdate(Game.Entity entity)
        {
        }

        internal void LateUpdateAction(Game.Entity entity)
        {
            OnLateUpdate(entity);
        }

        /// <summary>
        /// Should be used like MonoBehaviour's LateUpdate for the action
        /// </summary>
        protected virtual void OnLateUpdate(Game.Entity entity)
        {
        }

        internal void FixedUpdateAction(Game.Entity entity)
        {
            OnFixedUpdate(entity);
        }

        /// <summary>
        /// Should be used like MonoBehaviour's FixedUpdate for the action
        /// </summary>
        protected virtual void OnFixedUpdate(Game.Entity entity)
        {
        }

        internal void FinishAction(Game.Entity entity, bool rethink = true)
        {
            OnFinish(entity);
            if (rethink)
            {
                entity.Set<IsNeedRethink>();
            }
        }

        /// <summary>
        /// Called when the action is finished, either by failing or succeeding in achieving a desired behaviour
        /// </summary>
        protected virtual void OnFinish(Game.Entity entity)
        {
        }

        [Obsolete("Use ActionSucceeded")]
        protected void ActionSucceed(Game.Entity entity)
        {
            Brain.ActionSucceeded(this, entity);
        }

        public void ActionSucceeded(Game.Entity entity)
        {
            Brain.ActionSucceeded(this, entity);
        }

        public void ActionFailed(Game.Entity entity)
        {
            Brain.ActionFailed(this, entity);
        }

        internal void UpdateTargetsList(Game.Entity entity)
        {
            UpdateTargets(entity);
        }

        /// <summary>
        /// Fires when you need to update the list of targets
        /// </summary>
        protected virtual void UpdateTargets(Game.Entity entity)
        {
        }

        /// <summary>
        /// Calculates the score of the action
        /// </summary>
        /// <returns></returns>
        internal float GetScore(int recordIndex, Game.Entity entity)
        {
            _considerationCount = _selfConsiderations.Length + _targetedConsiderations.Length;

            if (_considerationCount == 0)
            {
                _score = 0;
                return 0;
            }

            _compensationFactor = 1f - (1f / _considerationCount);
            _score = 1;

            float selfScore = GetSelfScore(recordIndex, entity);

            if (selfScore == 0)
            {
#if UNITY_EDITOR
                for (int i = 0; i < _targetedConsiderations.Length; i++)
                {
                    _targetedConsiderations[i].SetScoreToZero();
                }
#endif
                _score = 0;
                return 0f;
            }

            for (int i = TargetsScoresList1.Count - 1; i >= 0; --i)
            {
                if (TargetsScoresList1[i] == null)
                {
                    TargetsScoresList1.RemoveAt(i);
                    TargetsScoresList2.RemoveAt(i);
                }
            }

            if (TargetsScoresList1.Count > 0)
            {
                for (int i = 0; i < TargetsScoresList2.Count; i++)
                {
                    TargetsScoresList2[i] = 1;
                }
                for (int i = 0; i < _targetedConsiderations.Length; i++)
                {
                    MultiplyConsiderationScoreForAllTargets(_targetedConsiderations[i], entity);
                }
                int index = GetBestTarget();
                ChosenTarget = TargetsScoresList1[index];
#if UNITY_EDITOR
                if (debugTargetedScores.Count > 0)
                {
                    Dictionary<ConsiderationBase, float> scores = debugTargetedScores[ChosenTarget];
                    for (int i = 0; i < _targetedConsiderations.Length; i++)
                    {
                        _targetedConsiderations[i].SetScore(scores[_targetedConsiderations[i]]);
                    }
                }
                else
                {
                    UnityEngine.Debug.LogWarning(
                        $"Action {Name} has targets but there are no considerations with NeedTarget = true");
                }
#endif
                _score = _score * selfScore * TargetsScoresList2[index];
            }
            else if
                (_targetedConsiderations.Length >
                 0) //if we have targeted considerations but no targets then we should not do the action
                _score = 0;
            else
                _score *= selfScore;

            _score *= _weight;
            return _score;
        }

        /// <summary>
        /// Gets the score for self considerations
        /// </summary>
        /// <returns></returns>
        private float GetSelfScore(int recordIndex, Game.Entity entity)
        {
            float totalScore = 1;
            for (int i = 0; i < _selfConsiderations.Length; i++)
            {
                float score = _selfConsiderations[i].GetScore(Brain, entity);
                score = ComputeCompensatedScore(score);
                totalScore *= score;
                //if consideration is close to 0 then multiplying things by it would only result in 0 so there is no point in multiplying the rest
#if !UNITY_EDITOR
                if (totalScore < Mathf.Epsilon)
                {

                    return 0;

            }
#endif
            }

            return totalScore;
        }

        //This is only used in the editor for the debugger
        private readonly Dictionary<Component, Dictionary<ConsiderationBase, float>>
            debugTargetedScores = new();

        private void MultiplyConsiderationScoreForAllTargets(ConsiderationBase consideration, Game.Entity entity)
        {
            for (int i = 0; i < TargetsScoresList1.Count; i++)
            {
#if !UNITY_EDITOR
                //TODO improve this. In editor for ease of debugging we calculate all targeted considerations until we remove this without messing the debugger
                if (TargetsScoresList2[i] <= Mathf.Epsilon)//if the score of a target is already 0, we don't need to go through the rest of the considerations
                    continue;
#endif
                float score = consideration.GetScore(TargetsScoresList1[i], entity);
#if UNITY_EDITOR
                if (!debugTargetedScores.ContainsKey(TargetsScoresList1[i]))
                    debugTargetedScores[TargetsScoresList1[i]] =
                        new Dictionary<ConsiderationBase, float>();
                debugTargetedScores[TargetsScoresList1[i]][consideration] = consideration.Score;
#endif
                score = ComputeCompensatedScore(score);
                TargetsScoresList2[i] *= score;
            }
        }

        private float ComputeCompensatedScore(float score)
        {
            float modification = (1f - score) * _compensationFactor;
            return score + (modification * score);
        }

        private int GetBestTarget()
        {
            int lastTargetIndex = -1;

            float maxScore = -1;
            int index = -1;
            for (int i = 0; i < TargetsScoresList2.Count; ++i)
            {
                if (TargetsScoresList1[i] == ChosenTarget)
                {
                    lastTargetIndex = i;
                    TargetsScoresList2[i] += _momentum;
                }

                if (TargetsScoresList2[i] >= maxScore)
                {
                    maxScore = TargetsScoresList2[i];
                    index = i;
                }
            }

            if (lastTargetIndex >= 0)
                TargetsScoresList2[lastTargetIndex] -=
                    _momentum; //so other action score considerations don't get affected by this
            return index;
        }
    }
}
