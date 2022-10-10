#if UNITY_2019_3_OR_NEWER
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.NodeBased;
using UnityUtilityEditor.Window.NodeBased.Stuff;

namespace UnityUtilityEditor.Window.NodeBased
{
    internal class GraphEditorWindow : EditorWindow
    {
        private readonly Color SELECTION_COLOR = Colours.Black.AlterA(0.25f);

        private SerializedGraph _serializedGraph;
        private GraphEditorSettings _settings;
        private HashSet<string> _nodeIgnoredFields;
        private GraphCamera _camera;
        private GraphSidePanel _sidePanel;
        private GraphGrid _grid;
        private GraphToolbar _toolbar;
        private List<NodeViewer> _nodeViewers;

        private PortViewer _selectedPort;
        private bool _selectionRectOn;
        private Vector2 _downPoint;
        private Vector2 _mapSize;
        private Vector2 _winSize;
        private int _mapSizeVersion;
        private int _winSizeVersion;
        private int _rootNodeVersion;

        private int _onGuiCounter;
        private int _rootNodeId;

        public SerializedGraph SerializedGraph => _serializedGraph;
        public IReadOnlyList<NodeViewer> NodeViewers => _nodeViewers;
        public int OnGuiCounter => _onGuiCounter;
        public GraphCamera Camera => _camera;
        public bool GridSnapping => _toolbar.GridToggle;
        public bool HideTransitions => _toolbar.HideTransitions;
        public TransitionViewType TransitionView => _toolbar.TransitionView;
        public HashSet<string> NodeIgnoredFields => _nodeIgnoredFields;
        public GraphEditorSettings Settings => _settings;

        public Vector2 WinSize
        {
            get
            {
                if (_winSizeVersion != _onGuiCounter)
                {
                    _winSize.x = position.width;
                    _winSize.y = position.height;
                    _winSizeVersion = _onGuiCounter;
                }

                return _winSize;
            }
        }

        public Vector2 MapSize
        {
            get
            {
                if (_mapSizeVersion != _onGuiCounter)
                {
                    _mapSize = WinSize;
                    if (_toolbar.PropertiesToggle)
                        _mapSize.x -= SerializedGraph.PANEL_WIDTH;
                    _mapSize.y -= GraphToolbar.HEIGHT;
                    _mapSizeVersion = _onGuiCounter;
                }

                return _mapSize;
            }
        }

        public int RootNodeId
        {
            get
            {
                if (_rootNodeVersion != _onGuiCounter)
                {
                    _rootNodeId = _serializedGraph.RootNodeProperty.intValue;
                    _rootNodeVersion = _onGuiCounter;
                }

                return _rootNodeId;
            }
        }

        private void OnEnable()
        {
            minSize = new Vector2(800f, 600f);
            _nodeIgnoredFields = new HashSet<string>
            {
                RawNode.NameFieldName,
                RawNode.ArrayFieldName,
                RawNode.GraphFieldName,
                RawNode.IdFieldName,
                RawNode.PositionFieldName,
            };
        }

        private void OnGUI()
        {
            if (_serializedGraph.GraphAsset == null)
            {
                Close();
                return;
            }

            _serializedGraph.SerializedObject.Update();

            _onGuiCounter++;

            Event e = Event.current;

            _toolbar.Draw();

            Vector2 mapScreenPos = new Vector2(_toolbar.PropertiesToggle ? SerializedGraph.PANEL_WIDTH : 0f, 0f);
            Rect mapRect = new Rect(mapScreenPos, MapSize);

            if (_toolbar.PropertiesToggle)
                _sidePanel.Draw(new Rect(0f, 0f, SerializedGraph.PANEL_WIDTH, mapRect.height));

            GUI.BeginGroup(mapRect);

            _camera.ProcessEvents(e);
            if (_camera.IsDragging)
                EditorGUIUtility.AddCursorRect(mapRect, MouseCursor.Pan);

            _grid.Draw();

            DrawConnectionLine(e);
            DrawNodes();
            DrawSelectionRect(e);

            ProcessNodeEvents(e);
            ProcessEvents(e);

            GUI.EndGroup();

            _serializedGraph.SerializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private void OnDestroy()
        {
            if (_serializedGraph.GraphAsset == null)
                return;

            Save();
        }

        public static void Open(RawGraph graphAsset)
        {
            GetWindow<GraphEditorWindow>(true, "Graph Editor").SetUp(graphAsset);
        }

        public void SetAsRoot(NodeViewer node)
        {
            _serializedGraph.SerializedObject.Update();
            _serializedGraph.SetAsRoot(node);
            _serializedGraph.SerializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        public void DeleteNode(NodeViewer node)
        {
            _serializedGraph.SerializedObject.Update();

            _nodeViewers.Remove(node);
            _nodeViewers.ForEach(item => item.RemoveTransition(node));
            _serializedGraph.RemoveNode(node);

            foreach (SerializedProperty nodeProp in _serializedGraph.NodesProperty.EnumerateArrayElements())
            {
                int id = nodeProp.FindPropertyRelative(RawNode.IdFieldName).intValue;
                _nodeViewers.Find(item => item.Id == id).SetSerializedProperty(nodeProp);
            }

            if (_nodeViewers.Count == 0)
                _camera.Position = default;

            _serializedGraph.SerializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        public void OnClickOnPort(PortViewer targetPort)
        {
            if (_selectedPort == targetPort)
                return;

            if (_selectedPort == null || _selectedPort.Type == targetPort.Type || _selectedPort.Node == targetPort.Node)
            {
                _selectedPort = targetPort;
                return;
            }

            if (_selectedPort.Node.Type == NodeType.Hub && targetPort.Node.Type == NodeType.Hub)
            {
                _selectedPort = targetPort;
                return;
            }

            PortViewer sourse = targetPort.Type == PortType.Out ? targetPort : _selectedPort;
            PortViewer dest = targetPort.Type == PortType.In ? targetPort : _selectedPort;

            sourse.Node.CreateTransition(dest);

            _selectedPort = null;
        }

        //public void CopySelectedNode()
        //{
        //    _graphAssetEditor.SerializedObject.Update();

        //    List<NodeViewer> newNodes = new List<NodeViewer>();

        //    foreach (NodeViewer item in _nodeViewers.Where(item => item.IsSelected))
        //    {
        //        if (item.Type.ServiceNode())
        //            continue;

        //        Rect rect = item.WorldRect;
        //        SerializedProperty nodeProp = _graphAssetEditor.CloneNode(rect.position + Vector2.up * (rect.height + 30f), item.Id);
        //        newNodes.Place(new NodeViewer(nodeProp, this))
        //                .Select(true);
        //        item.Select(false);
        //    }

        //    _nodeViewers.AddRange(newNodes);

        //    _graphAssetEditor.SerializedObject.ApplyModifiedPropertiesWithoutUndo();
        //}

        private void SetUp(RawGraph graphAsset)
        {
            if (_serializedGraph == null)
            {
                _settings = GraphEditorSettings.Load(graphAsset);
                _serializedGraph = new SerializedGraph(graphAsset);
                _sidePanel = new GraphSidePanel(this);
                _grid = new GraphGrid(this);
                _toolbar = new GraphToolbar(this);
                _camera = new GraphCamera(this);
                _nodeViewers = new List<NodeViewer>();
            }
            else if (_serializedGraph.GraphAsset != graphAsset)
            {
                Save();

                _settings = GraphEditorSettings.Load(graphAsset);
                _serializedGraph.InitAssetReference(graphAsset);
                _camera.Position = _settings.CameraPosition;
                _nodeViewers.Clear();
            }
            else
            {
                return;
            }

            foreach (SerializedProperty nodeProp in _serializedGraph.NodesProperty.EnumerateArrayElements())
            {
                _nodeViewers.Add(new NodeViewer(nodeProp, this));
            }

            if (_serializedGraph.CommonNodeProperty.HasManagedReferenceValue())
            {
                _nodeViewers.Add(new NodeViewer(_serializedGraph.CommonNodeProperty, this));
            }

            _nodeViewers.ForEach(item => item.CreateConnections());
        }

        private void CreateNode(Vector2 mousePosition, Type type)
        {
            _serializedGraph.SerializedObject.Update();

            Vector2 position = _camera.ScreenToWorld(mousePosition);
            SerializedProperty nodeProp = _serializedGraph.CreateNode(position, type);
            _nodeViewers.Add(new NodeViewer(nodeProp, this));

            _serializedGraph.SerializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private void DrawNodes()
        {
            for (int i = 0; i < _nodeViewers.Count; i++)
            {
                _nodeViewers[i].DrawTransitions();
            }

            for (int i = 0; i < _nodeViewers.Count; i++)
            {
                _nodeViewers[i].Draw();
            }
        }

        private void DrawSelectionRect(Event e)
        {
            if (_selectionRectOn)
            {
                Rect selectionRect = new Rect(_downPoint, e.mousePosition - _downPoint);

                Handles.DrawSolidRectangleWithOutline(selectionRect, SELECTION_COLOR, GraphEditorStyles.GetLineColor());

                for (int i = 0; i < _nodeViewers.Count; i++)
                {
                    _nodeViewers[i].Select(selectionRect.Overlaps(_nodeViewers[i].ScreenRect, true));
                }

                GUI.changed = true;
            }
        }

        private void DrawConnectionLine(Event e)
        {
            if (_selectedPort == null)
                return;

            switch (_toolbar.TransitionView)
            {
                case TransitionViewType.Spline:
                    Vector2 startTangentDir = _selectedPort.Type == PortType.In ? Vector2.left : Vector2.right;
                    Vector2 endTangentDir = _selectedPort.Type == PortType.In ? Vector2.right : Vector2.left;
                    TransitionViewer.DrawSpline(_selectedPort.ScreenRect.center, e.mousePosition, startTangentDir, endTangentDir);
                    break;

                case TransitionViewType.Direction:
                    TransitionViewer.DrawDirection(_selectedPort.Node.ScreenRect.center, e.mousePosition);
                    break;

                default:
                    throw new UnsupportedValueException(_toolbar.TransitionView);
            }

            GUI.changed = true;
        }

        private void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    mouseDown();
                    break;

                case EventType.MouseUp:
                    mouseUp();
                    break;

                    //case EventType.KeyDown:
                    //    keyDown();
                    //    break;
            }

            void mouseDown()
            {
                if (e.button == 0)
                {
                    if (!_selectionRectOn)
                    {
                        _selectionRectOn = true;
                        _downPoint = e.mousePosition;
                        _selectedPort = null;
                    }
                }
                else if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
            }

            void mouseUp()
            {
                if (e.button == 0)
                {
                    _selectionRectOn = false;
                }
            }

            //void keyDown()
            //{
            //    if (e.keyCode == KeyCode.D)
            //    {
            //        CopySelectedNode();
            //        GUI.changed = true;
            //    }
            //}
        }

        private void ProcessNodeEvents(Event e)
        {
            if (_selectionRectOn)
                return;

            bool needLockEvent = false;

            for (int i = _nodeViewers.Count - 1; i >= 0; i--)
            {
                if (_nodeViewers[i].ProcessEvents(e))
                    needLockEvent = true;
            }

            if (needLockEvent)
                e.Use();
        }

        private void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu menu = new GenericMenu();

            addServiceMenuItem(typeof(CommonNode), "Common", _serializedGraph.CommonNodeProperty.HasManagedReferenceValue());
            addServiceMenuItem(typeof(HubNode), "Hub", false);
            addServiceMenuItem(typeof(ExitNode), "Exit", _nodeViewers.Contains(item => item.Type == NodeType.Exit));

            menu.AddSeparator(string.Empty);

            addNodeMenuItem(_serializedGraph.GraphAsset.GetNodeType());

            foreach (Type type in TypeCache.GetTypesDerivedFrom(_serializedGraph.GraphAsset.GetNodeType()))
            {
                addNodeMenuItem(type);
            }

            menu.ShowAsContext();

            void addServiceMenuItem(Type type, string name, bool disabled)
            {
                if (disabled)
                    menu.AddDisabledItem(new GUIContent(name));
                else
                    menu.AddItem(new GUIContent(name), false, () => CreateNode(mousePosition, type));
            }

            void addNodeMenuItem(Type type)
            {
                if (type.IsAbstract)
                    menu.AddDisabledItem(new GUIContent($"Abstract {type.Name} ({type.Namespace})"));
                else
                    menu.AddItem(new GUIContent($"{type.Name} ({type.Namespace})"), false, () => CreateNode(mousePosition, type));
            }
        }

        private void Save()
        {
            _nodeViewers.ForEach(item => item.Save());
            _serializedGraph.Save();
            _toolbar.Save();
            _serializedGraph.SerializedObject.ApplyModifiedPropertiesWithoutUndo();
            _settings.CameraPosition = _camera.Position;
            GraphEditorSettings.Save(_serializedGraph.GraphAsset, _settings);
            EditorUtilityExt.SaveProject();
        }
    }
}
#endif
