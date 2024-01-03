using NodeGraph.Editor;
using NodeGraph.Runtime;
using UnityEditor;

namespace NodeGraphTest.Editor
{
    public static class NodeGraphTestMenu
    {
        [MenuItem("Tools/" + nameof(NodeGraphWindow))]
        static void OpenNodeGraphWindow()
        {
            var window = EditorWindow.GetWindow<NodeGraphWindow>("NodeGraphWindow");
            window.Show();
        }
    }
}