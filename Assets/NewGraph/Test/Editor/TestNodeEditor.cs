using UnityEngine.UIElements;

namespace NewGraph
{
    [CustomNodeEditor(typeof(TestNode))]
    public class TestNodeEditor : NodeEditor
    {
        public override void Initialize(NodeController nodeController)
        {
            nodeController.nodeView.titleContainer.Add(new Label("Modded label"));
        }
    }
}