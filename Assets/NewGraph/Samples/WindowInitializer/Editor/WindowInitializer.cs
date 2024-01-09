using NewGraph;
using UnityEditor;

namespace WindowInitializer
{
    public class WindowInitializer
    {
        [MenuItem("Tools/" + nameof(GraphWindow))]
        static void OpenWindow()
        {
            GraphWindow.InitializeWindowBase(typeof(ScriptableGraphModel));
        }
    }
}