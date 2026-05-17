using System;
using UnityEngine;

namespace NoOpArmy.WiseFeline
{
    /// <summary>
    /// This is the base class of Wise Feline AI assets. 
    /// You never need to use this directly.
    /// </summary>
    public class AIObject : ScriptableObject
    {
        /// <summary>
        /// The unique id of the asset.
        /// </summary>
        [HideInInspector]
        public string guid;

        /// <summary>
        /// The name of the asset
        /// </summary>
        public string Name;

        /// <summary>
        /// Sets the Guid of the object.
        /// We do not ensure the IDs are unique and assume the application developer generates IDs with the guarantees he/she needs.
        /// </summary>
        /// <param name="newId">The ID to assign</param>
        /// <param name="canReplace">Can the ID be replaced if it is alredy set, defaults to false</param>
        /// <remarks>This method should onlybe called once per object</remarks>
        public void SetGuid(string newId,bool canReplace = false)
        {
            if (string.IsNullOrEmpty(guid) || canReplace)
            {
                guid = newId;
            }
            else
            {
                throw new InvalidOperationException("Id of an AI Object cannot be set multiple times");
            }
        }
    }
}
