using System;
using UnityEngine;

namespace NewGraph
{
    [Serializable]
    public class SearchTreeEntry : IComparable<SearchTreeEntry>
    {
        public int level;
        public GUIContent content;

        public Action<object> actionToExecute;
        public Func<bool> enabledCheck;
        public bool IsEnabled;
        public object userData;

        public SearchTreeEntry(string content, Func<bool> enabledCheck, Action<object> actionToExecute = null)
        {
            this.content = new GUIContent(content);
            this.enabledCheck = enabledCheck;
            this.actionToExecute = actionToExecute;
        }

        public static bool AlwaysEnabled()
        {
            return true;
        }

        public string name => content.text;

        public int CompareTo(SearchTreeEntry o)
        {
            return name.CompareTo(o.name);
        }
    }
}