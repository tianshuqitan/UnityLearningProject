using System;
using UnityEngine;

namespace NewGraph
{
    [Serializable]
    public class SearchTreeGroupEntry : SearchTreeEntry
    {
        internal int selectedIndex;
        internal Vector2 scroll;

        public SearchTreeGroupEntry(string content, Func<bool> enabledCheck, int level = 0) : base(content,
            enabledCheck)
        {
            this.level = level;
        }
    }
}