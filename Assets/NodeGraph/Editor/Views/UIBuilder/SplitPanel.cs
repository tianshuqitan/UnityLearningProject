using UnityEngine.UIElements;

namespace NodeGraph.Editor
{
    public class SplitPanel : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitPanel, UxmlTraits>
        {
        }
    }
}