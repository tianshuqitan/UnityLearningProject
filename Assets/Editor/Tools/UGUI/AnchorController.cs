using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Editor.Tools.UGUI
{
    public static class AnchorController
    {
        [MenuItem("Tools/UGUI/Anchors To Corners")]
        private static void AnchorToCorners()
        {
            foreach (var transform in Selection.transforms)
            {
                var t = transform as RectTransform;
                var p = Selection.activeTransform.parent as RectTransform;

                if (t == null || p == null)
                {
                    continue;
                }

                var rect = p.rect;
                
                var minX = t.anchorMin.x + t.offsetMin.x / rect.width;
                var minY = t.anchorMin.y + t.offsetMin.y / rect.height;

                var maxX = t.anchorMax.x + t.offsetMax.x / rect.width;
                var maxY = t.anchorMax.y + t.offsetMax.y / rect.height;

                t.anchorMin = new Vector2(minX, minY);
                t.anchorMax = new Vector2(maxX, maxY);
                t.offsetMin = t.offsetMax = Vector2.zero;
            }
        }
    }
}
