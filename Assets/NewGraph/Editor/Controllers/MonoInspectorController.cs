using UnityEngine.UIElements;

namespace NewGraph
{
    public class MonoInspectorController : InspectorControllerBase
    {
        public MonoInspectorController(VisualElement parent) : base(parent)
        {
        }

        public override void CreateRenameGraphUI(IGraphModelData graph)
        {
            var graphName = inspectorHeader.Q<Label>();
            graphName.text = (graph as MonoGraphModel)?.name;
        }

        protected override void SetupCreateButton(Button createButton)
        {
            createButton.style.display = DisplayStyle.None;
        }

        protected override void SetupLoadButton(Button loadButton)
        {
            loadButton.style.display = DisplayStyle.None;
        }
    }
}