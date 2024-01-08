using UnityEngine;
using NewGraph;
using UnityEditor.Experimental.GraphView;

namespace Test
{
    /// <summary>
    /// Example to create a custom context menu.
    /// Inherit from NewGraph.ContextMenu and add the [CustomContextMenu].
    /// Please note: There can only be one custom context menu for the whole graph.
    /// </summary>
    [CustomContextMenu]
    public class CustomMenu : NewGraph.ContextMenu
    {
        /// <summary>
        /// Customize the main label for this menu.
        /// </summary>
        /// <returns></returns>
        protected override string GetHeader()
        {
            return "My Custom Header";
        }

        /// <summary>
        /// Modify the main node entries.
        /// </summary>
        protected override void AddNodeEntries()
        {
            base.AddNodeEntries();
            AddNodeEntry("Custom/Menu/Item", (obj) => { Debug.Log("custom menu item was clicked!"); });
            AddNodeEntry("Custom/Group", (obj) => graphController.graphView.AddElement(new Group()));
        }
    }
}