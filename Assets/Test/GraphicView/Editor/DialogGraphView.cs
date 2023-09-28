using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Test.GraphicView.Editor
{
    public class DialogGraphView : GraphView
    {
        public readonly Vector2 DefaultNodeSize = new Vector2(150, 200);

        public DialogGraphView()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            grid.StretchToParentSize();
            grid.styleSheets.Add(Resources.Load<StyleSheet>("DialogGraph"));
            Insert(0, grid);

            AddElement(GenerateEntryPointNode());
        }

        private DialogNode GenerateEntryPointNode()
        {
            var node = new DialogNode
            {
                title = "Start",
                Guid = Guid.NewGuid().ToString(),
                DialogText = "EntryPoint",
                EntryPoint = true,
            };

            var generatePort = GeneratePort(node, Direction.Output);
            generatePort.portName = "Next";
            node.outputContainer.Add(generatePort);
            
            // 节点不可删除, 不可移动
            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;
            
            node.RefreshExpandedState();
            node.RefreshPorts();

            node.SetPosition(new Rect(100, 200, 100, 150));

            return node;
        }

        private Port GeneratePort(DialogNode node, Direction portDirection,
            Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
        }

        public DialogNode CreateDialogNode(string nodeName)
        {
            var node = new DialogNode
            {
                title = nodeName,
                DialogText = nodeName,
                Guid = Guid.NewGuid().ToString(),
            };

            // 添加输入节点
            var inputPort = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            node.inputContainer.Add(inputPort);

            // 添加节点标题颜色
            node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            
            // 添加新增输出端口
            var button = new Button(() => AddChoicePort(node)) { text = "New Choice" };
            node.titleContainer.Add(button);

            // 添加 Node 标题输入框
            var textField = new TextField(string.Empty);
            textField.RegisterValueChangedCallback(evt =>
            {
                node.DialogText = evt.newValue;
                node.title = evt.newValue;
            });
            textField.SetValueWithoutNotify(node.title);
            node.mainContainer.Add(textField);

            node.RefreshExpandedState();
            node.RefreshPorts();
            node.SetPosition(new Rect(Vector2.zero, DefaultNodeSize));

            return node;
        }

        public void AddChoicePort(DialogNode node, string portName = "")
        {
            var generatePort = GeneratePort(node, Direction.Output);

            var oldLabel = generatePort.contentContainer.Q<Label>("type");
            generatePort.contentContainer.Remove(oldLabel);

            var outputPortCount = node.outputContainer.Query("connector").ToList().Count;
            var choicePortName = string.IsNullOrEmpty(portName) ? $"Choice {outputPortCount}" : portName;

            var textField = new TextField
            {
                name = string.Empty,
                value = choicePortName,
            };
            textField.RegisterValueChangedCallback(evt => generatePort.portName = evt.newValue);
            
            generatePort.contentContainer.Add(new Label("  "));
            generatePort.contentContainer.Add(textField);

            var deleteButton = new Button(() => RemovePort(node, generatePort)) { text = "X" };
            generatePort.contentContainer.Add(deleteButton);

            generatePort.portName = choicePortName;

            node.outputContainer.Add(generatePort);
            node.RefreshExpandedState();
            node.RefreshPorts();
        }

        private void RemovePort(DialogNode node, Port generatePort)
        {
            var targetEdge = edges.ToList().Where(x =>
                x.output.portName == generatePort.portName && x.output.node == generatePort.node);

            if (!targetEdge.Any())
            {
                return;
            }

            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());

            node.outputContainer.Remove(generatePort);
            node.RefreshPorts();
            node.RefreshExpandedState();
        }

        public void CreateNode(string nodeName)
        {
            AddElement(CreateDialogNode(nodeName));
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.Where(port => startPort != port && startPort.node != port.node).ToList();
        }
    }
}