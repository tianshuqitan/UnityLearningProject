using UnityEditor;
using UnityEngine;

namespace Tools.Excel.Editor
{
    public static class ExcelMenu
    {
        [MenuItem("Tools/Excel/OpenToolsWindow")]
        public static void OpenToolsWindow()
        {
            var window = EditorWindow.GetWindow<ExcelToolsWindow>();
            window.titleContent = new GUIContent("Excel Tools Window");
            window.Show();
        }
    }
}
