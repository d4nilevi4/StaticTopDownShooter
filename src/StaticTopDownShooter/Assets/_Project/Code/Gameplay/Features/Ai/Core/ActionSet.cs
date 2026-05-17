using System.Collections.Generic;
using UnityEngine;

namespace NoOpArmy.WiseFeline
{
    /// <summary>
    /// Action sets are like directories which group a set of actions together but you don't need to deal with them in code unless you are making debugging/editor tools or highly advanced systems
    /// An Agent Behavior asset is made of a set of ActionSet objects which each of them contain a set of actions.
    /// </summary>
    public class ActionSet : AIObject
    {
        /// <summary>
        /// The agent behavior that this instance belongs too.
        /// </summary>
        [HideInInspector]
        public AgentBehavior Behavior;

        /// <summary>
        /// The list of actions which are contained in this action set.
        /// </summary>
        [SerializeField, HideInInspector]
        private List<ActionBase> _actions;

        /// <summary>
        /// The list of actions which are contained in this action set.
        /// </summary>
        public List<ActionBase> Actions {
            get
            {
                if (_actions == null)
                    _actions = new List<ActionBase>();
                return _actions;
            }
        }

        /// <summary>
        /// Clones the action set and all of its actions to be used at runtime for an agent.
        /// </summary>
        /// <param name="behavior"></param>
        /// <param name="wiseAIFactory"></param>
        /// <returns></returns>
        public ActionSet Clone(AgentBehavior behavior)
        {
            ActionSet set = Object.Instantiate(this);
            set.Behavior = behavior;

            for (int i = 0; i < set.Actions.Count; i++)
            {
                set.Actions[i] = set.Actions[i].Clone(set);
            }
            return set;
        }
    }
}
