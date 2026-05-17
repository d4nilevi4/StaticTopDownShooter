using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace NoOpArmy.WiseFeline
{
    /// <summary>
    /// This search provider is used for creating the list of action/consideration types to show
    /// </summary>
    public class AITypesSearchProvider : ScriptableObject, ISearchWindowProvider
    {

        public System.Type baseType;

        public Action<System.Type> OnTypeSelected;
        private Texture2D icon = null;

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            if (icon == null)
                icon = Resources.Load<Texture2D>("wficon");
            List<SearchTreeEntry> list = new List<SearchTreeEntry>();

            var title = new SearchTreeGroupEntry(new GUIContent("Types"));
            title.level = 0;
            title.userData = null;
            list.Add(title);

            IOrderedEnumerable<Type> types = ReflectionUtilities.GetAllDerivedTypes(baseType).OrderBy(_ => _.Namespace).ThenBy(_ => _.Name);
            foreach (var type in types)
            {
                var e = new SearchTreeEntry(new GUIContent(type.Name, icon, $"{type.FullName}"));
                e.level = 1;
                e.userData = type;
                list.Add(e);
            }
            return list;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            OnTypeSelected?.Invoke((System.Type)SearchTreeEntry.userData);
            return true;
        }
    }
}
