using NewGraph;
using UnityEngine;

namespace Test
{
    /// <summary>
    /// Example to create a custom edge drop menu.
    /// Inherit from EdgeDropMenu and add the [CustomEdgeDropMenu].
    /// Please note: There can only be one custom edge drop menu for the whole graph.
    /// </summary>
    [CustomEdgeDropMenu]
    public class CustomEdgeDropMenu : EdgeDropMenu
    {
        /// <summary>
        /// Customize the main label for this menu.
        /// </summary>
        /// <returns></returns>
        protected override string GetHeader()
        {
            return "My Custom Edge Drop";
        }

        /// <summary>
        /// Modify the main node entries.
        /// </summary>
        protected override void AddNodeEntries()
        {
            base.AddNodeEntries();
            AddNodeEntry("Custom/Menu/EdgedropItem", (obj) => { Debug.Log("custom edge drop item was clicked!"); });
        }
    }
}