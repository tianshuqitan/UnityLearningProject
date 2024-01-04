using UnityEngine.UIElements;

namespace NewGraph
{
    [Node("#00000001")]
    public class CommentNode : INode, IUtilityNode
    {
        private NodeView nodeView;

        public bool CreateInspectorUI() => false;

        public bool CreateNameUI() => true;

        public void Initialize(NodeController nodeController)
        {
            nodeView = nodeController.nodeView;
            // we need auto resizing, so the comment node expands to the full width of the provided text...
            nodeView.style.width = StyleKeyword.Auto;
            nodeView.AddToClassList(nameof(CommentNode));
            
            nodeView.Clear();
        }

        public bool ShouldColorizeBackground()
        {
            return true;
        }
    }
}