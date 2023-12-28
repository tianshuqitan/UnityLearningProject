using System.Collections.Generic;
using System.Linq;
using Test.GraphicView.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Test.GraphicView.Editor
{
    public class GraphSaveUtility
    {
        private DialogGraphView m_TargetGraphView;
        private DialogContainer m_ContainerCache;
        private SerializedObject m_SerializedObject;

        private List<Edge> Edges => m_TargetGraphView.edges.ToList();
        private List<DialogNode> Nodes => m_TargetGraphView.nodes.ToList().Cast<DialogNode>().ToList();

        public static GraphSaveUtility GetInstance(DialogGraphView targetGraphView)
        {
            return new GraphSaveUtility()
            {
                m_TargetGraphView = targetGraphView
            };
        }

        public void SaveGraph(string fileName)
        {
            if (!Edges.Any())
            {
                return;
            }

            var container = ScriptableObject.CreateInstance<DialogContainer>();

            var connectPorts = Edges.Where(edge => edge.input.node != null).ToArray();
            foreach (var edge in connectPorts)
            {
                var outputNode = edge.output.node as DialogNode;
                var inputNode = edge.input.node as DialogNode;

                container.linkDatas.Add(new NodeLinkData
                {
                    baseNodeGuid = outputNode.Guid,
                    portName = edge.output.portName,
                    targetNodeGuid = inputNode.Guid,
                });
            }

            foreach (var node in Nodes.Where(node => !node.EntryPoint))
            {
                container.nodeDatas.Add(new DialogNodeData()
                {
                    guid = node.Guid,
                    dialogText = node.DialogText,
                    position = node.GetPosition().position
                });
            }

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            AssetDatabase.CreateAsset(container, $"Assets/Resources/{fileName}.asset");
            AssetDatabase.SaveAssets();
        }

        public void LoadGraph(string fileName)
        {
            m_ContainerCache = Resources.Load<DialogContainer>(fileName);
            if (m_ContainerCache == null)
            {
                EditorUtility.DisplayDialog("File Not Found", "Target dialog graph file does not exists!", "OK");
                return;
            }
            
            m_SerializedObject = new SerializedObject(m_ContainerCache);

            ClearGraph();
            CreateNodes();
            ConnectNodes();
        }

        private void ConnectNodes()
        {
            for (var i = 0; i < Nodes.Count; i++)
            {
                var connections = m_ContainerCache.linkDatas.Where(x => x.baseNodeGuid == Nodes[i].Guid).ToList();
                for (var j = 0; j < connections.Count(); j++)
                {
                    var targetNodeGuid = connections[j].targetNodeGuid;
                    var targetNode = Nodes.First(x => x.Guid == targetNodeGuid);
                    LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);

                    targetNode.SetPosition(new Rect(
                        m_ContainerCache.nodeDatas.First(x => x.guid == targetNodeGuid).position,
                        m_TargetGraphView.DefaultNodeSize));
                }
            }
        }

        private void LinkNodes(Port output, Port input)
        {
            var tmpEdge = new Edge
            {
                output = output,
                input = input,
            };
            
            tmpEdge.input.Connect(tmpEdge);
            tmpEdge.output.Connect(tmpEdge);
            m_TargetGraphView.Add(tmpEdge);
        }

        private void CreateNodes()
        {
            var nodeDatas = m_ContainerCache.nodeDatas;
            var nodeDatasFindProperty = m_SerializedObject.FindProperty("nodeDatas");
            for (var i = 0; i < nodeDatas.Count; i++)
            {
                var tmpNode = m_TargetGraphView.CreateDialogNode(nodeDatasFindProperty.GetArrayElementAtIndex(i), m_SerializedObject);
                tmpNode.Guid = nodeDatas[i].guid;
                m_TargetGraphView.AddElement(tmpNode);
                
                var nodePorts = m_ContainerCache.linkDatas.Where(x => x.baseNodeGuid == nodeDatas[i].guid).ToList();
                nodePorts.ForEach(x => m_TargetGraphView.AddChoicePort(tmpNode, x.portName));
            }
        }

        private void ClearGraph()
        {
            Nodes.Find(x => x.EntryPoint).Guid = m_ContainerCache.linkDatas[0].baseNodeGuid;

            foreach (var node in Nodes)
            {
                if (node.EntryPoint)
                {
                    continue;
                }

                // remove edges that connected to this node
                Edges.Where(x => x.input.node == node).ToList()
                    .ForEach(edge => m_TargetGraphView.RemoveElement(edge));

                // remove node
                m_TargetGraphView.RemoveElement(node);
            }
        }
    }
}