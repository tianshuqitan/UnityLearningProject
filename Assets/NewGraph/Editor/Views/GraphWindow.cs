using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NewGraph
{
    using static GraphSettingsSingleton;
    using static GraphSettings;

    public class GraphWindow : EditorWindow
    {
        private static readonly Dictionary<Type, Type> inspectorControllerLookup = new()
        {
            { typeof(ScriptableGraphModel), typeof(ScriptableInspectorController) },
            { typeof(MonoGraphModel), typeof(MonoInspectorController) },
        };

        private static readonly Dictionary<Type, Func<string, IGraphModelData>> lastGraphCreationStrategies =
            new()
            {
                { typeof(ScriptableGraphModel), ScriptableGraphModel.GetGraphData },
                { typeof(MonoGraphModel), MonoGraphModel.GetGraphData },
            };

        private KeyCode lastKeyCode;
        private EventModifiers lastModifiers;
        private EventType eventType;
        public GraphController graphController;

        public static Type currentWindowType = null;
        private static string currentWindowTypeKey = nameof(NewGraph) + "." + nameof(currentWindowType);

        private static Type CurrentWindowType
        {
            get
            {
                if (currentWindowType == null)
                {
                    string savedType = EditorPrefs.GetString(currentWindowTypeKey, null);
                    if (savedType != null)
                    {
                        currentWindowType = Type.GetType(savedType);
                    }
                }

                return currentWindowType;
            }
            set
            {
                currentWindowType = value;
                EditorPrefs.SetString(currentWindowTypeKey, currentWindowType.AssemblyQualifiedName);
            }
        }

        public static event Action<Event> OnGlobalKeyDown;

        [NonSerialized] private static GraphWindow window = null;
        [NonSerialized] private static bool loadRequested = false;

        public static void AddWindowType(Type windowType, Type inspectorControllerType,
            Func<string, IGraphModelData> lastGraphCreationStrategy)
        {
            if (!inspectorControllerLookup.ContainsKey(windowType))
            {
                inspectorControllerLookup.Add(windowType, inspectorControllerType);
                lastGraphCreationStrategies.Add(windowType, lastGraphCreationStrategy);
            }
        }

        public static void InitializeWindowBase(Type windowType)
        {
            if (window != null && CurrentWindowType != windowType)
            {
                window.Close();
            }

            if (window == null)
            {
                CurrentWindowType = windowType;
                window = GetWindow<GraphWindow>(Settings.windowName);
                window.wantsMouseMove = true;
                window.Show();
            }
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= LogPlayModeState;
            EditorApplication.playModeStateChanged += LogPlayModeState;
            GlobalKeyEventHandler.OnKeyEvent -= HandleGlobalKeyPressEvents;
            GlobalKeyEventHandler.OnKeyEvent += HandleGlobalKeyPressEvents;
        }
        
        private void HandleGlobalKeyPressEvents(Event evt)
        {
            if (!evt.isKey || mouseOverWindow != this || !hasFocus)
            {
                return;
            }

            if (lastKeyCode != evt.keyCode || lastModifiers != evt.modifiers)
            {
                lastModifiers = evt.modifiers;
                lastKeyCode = evt.keyCode;
                eventType = evt.type;
                OnGlobalKeyDown?.Invoke(evt);
            }

            if (evt.type == EventType.KeyUp)
            {
                lastKeyCode = KeyCode.None;
                lastModifiers = EventModifiers.None;
            }
        }

        public void LogPlayModeState(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.ExitingPlayMode:
                    graphController?.EnsureSerialization();
                    break;
                case PlayModeStateChange.EnteredEditMode:
                    LoadGraph();
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    graphController?.Reload();
                    break;
            }
        }

        private void OnGUI()
        {
            graphController?.Draw();
        }

        private void OnDisable()
        {
            GlobalKeyEventHandler.OnKeyEvent -= HandleGlobalKeyPressEvents;
            EditorApplication.playModeStateChanged -= LogPlayModeState;
            loadRequested = false;
        }

        public static void LoadGraph(IGraphModelData graph = null)
        {
            if (graph != null)
            {
                // SetLastOpenedGraphData(graph);
            }
            else
            {
                // var lastGraphInfo = LastOpenedGraphInfo;
                // if (lastGraphInfo != null && lastGraphInfo.graphType != null && lastGraphInfo.GUID != null)
                // {
                //     graph = lastGraphCreationStrategies[lastGraphInfo.graphType](lastGraphInfo.GUID);
                // }
            }

            if (graph != null)
            {
                if (graph.SerializedGraphData == null)
                {
                    graph.CreateSerializedObject();
                }

                var windowType = graph.BaseObject.GetType();
                InitializeWindowBase(windowType);
                window.graphController.OpenGraphExternal(graph);
            }
            else
            {
                LastOpenedGraphInfo = null;
            }

            loadRequested = true;
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;

            VisualElement docRoot = graphDocument.CloneTree();
            root.Add(docRoot);
            docRoot.StretchToParentSize();

            var graphRoot = docRoot.Q<VisualElement>("graphViewRoot");
            graphController =
                new GraphController(docRoot, root, inspectorControllerLookup[CurrentWindowType]);

            var graphView = graphController.graphView;
            graphView.StretchToParentSize();
            graphRoot.Add(graphView);
            
            root.styleSheets.Add(graphStylesheetVariables);
            root.styleSheets.Add(graphStylesheet);

            if (Settings.customStylesheet != null)
            {
                root.styleSheets.Add(Settings.customStylesheet);
            }

            root.schedule.Execute(() =>
            {
                if (!loadRequested)
                {
                    LoadGraph();
                }

                loadRequested = false;
            });
        }
    }
}