using System;
using System.Collections.Generic;
using System.IO;
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

        private GraphEditorSettings _settings;
        private GraphAssetEditor _graphAssetEditor;
        private GraphCamera _camera;
        private GraphGrid _grid;
        private GraphToolbar _toolbar;
        private List<NodeViewer> _nodeViewers;
        private List<TransitionViewer> _transitionViewers;

        private PortViewer _selectedPort;
        private bool _selectionRectOn;
        private Vector2 _downPoint;
        private Vector2 _mapSize;
        private Vector2 _winSize;
        private int _mapSizeVersion;
        private int _winSizeVersion;
        private int _rootNodeVersion;

        private int _onGuiCounter;
        private bool _hasExitNode;
        private RawNode _rootNode;

        public GraphAssetEditor GraphAssetEditor => _graphAssetEditor;
        public IReadOnlyList<NodeViewer> NodeViewers => _nodeViewers;
        public int OnGuiCounter => _onGuiCounter;
        public GraphCamera Camera => _camera;
        public bool GridSnapping => _toolbar.GridToggle;

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
                        _mapSize.x -= GraphAssetEditor.PANEL_WIDTH;
                    _mapSize.y -= GraphToolbar.HEIGHT;
                    _mapSizeVersion = _onGuiCounter;
                }

                return _mapSize;
            }
        }

        public RawNode RootNode
        {
            get
            {
                if (_rootNodeVersion != _onGuiCounter)
                {
                    _rootNode = _graphAssetEditor.GraphAsset.RootNode;
                    _rootNodeVersion = _onGuiCounter;
                }

                return _rootNode;
            }
        }

        private void OnEnable()
        {
            minSize = new Vector2(800f, 600f);
        }

        public static void OpenWindow(RawGraph graphAsset)
        {
            GraphEditorWindow window = GetWindow<GraphEditorWindow>(true, "Graph Editor");
            window.SetUp(graphAsset);
        }

        private void SetUp(RawGraph graphAsset)
        {
            if (_graphAssetEditor?.GraphAsset == graphAsset)
                return;

            if (_graphAssetEditor != null)
            {
                _nodeViewers.Clear();
                _transitionViewers.Clear();
            }
            else
            {
                _grid = new GraphGrid(this);
                _toolbar = new GraphToolbar(this);
                _nodeViewers = new List<NodeViewer>();
                _transitionViewers = new List<TransitionViewer>();
            }

            _settings = LoadSettings(graphAsset);
            _graphAssetEditor = new GraphAssetEditor(graphAsset);
            _camera = new GraphCamera(this, _settings.CameraPosition);

            foreach (RawNode item in _graphAssetEditor.ParseList())
            {
                _nodeViewers.Add(new NodeViewer(item, this));
                if (item is ExitNode)
                    _hasExitNode = true;
            }

            for (int i = 0; i < _nodeViewers.Count; i++)
            {
                CreateTransitionsForNode(_nodeViewers[i]);
            }
        }

        private void OnGUI()
        {
            if (_graphAssetEditor.GraphAsset == null)
            {
                Close();
                return;
            }

            _onGuiCounter++;

            Event e = Event.current;

            _toolbar.Draw();

            Vector2 mapScreenPos = new Vector2(_toolbar.PropertiesToggle ? GraphAssetEditor.PANEL_WIDTH : 0f, 0f);
            Rect mapRect = new Rect(mapScreenPos, MapSize);

            if (_toolbar.PropertiesToggle)
                _graphAssetEditor.Draw(new Rect(0f, 0f, GraphAssetEditor.PANEL_WIDTH, mapRect.height));

            GUI.BeginGroup(mapRect);

            _camera.ProcessEvents(e);
            if (_camera.IsDragging)
                EditorGUIUtility.AddCursorRect(mapRect, MouseCursor.Pan);

            _grid.Draw();

            DrawConnections();
            DrawConnectionLine(e);
            DrawNodes();
            DrawSelectionRect(e);

            ProcessNodeEvents(e);
            ProcessTransitionsEvents(e);
            ProcessEvents(e);

            GUI.EndGroup();
        }

        private void OnDestroy()
        {
            if (_graphAssetEditor.GraphAsset == null)
                return;

            Save();
        }

        private void OnLostFocus()
        {
            Save();
        }

        public void SetAsRoot(NodeViewer node)
        {
            _graphAssetEditor.SetAsRoot(node.NodeAsset);
        }

        public void DeleteNode(NodeViewer node)
        {
            _transitionViewers.RemoveAll(item => item.Source == node.In || item.Destination == node.Out);
            _nodeViewers.Remove(node);
            _nodeViewers.ForEach(item => item.RemoveReference(node.NodeAsset));
            _graphAssetEditor.DestroyNode(node.NodeAsset);

            if (_nodeViewers.Count == 0)
                _camera.Position = default;

            if (node.NodeAsset is ExitNode)
                _hasExitNode = false;
        }

        public void OnClickOnPort(PortViewer newPort)
        {
            if (_selectedPort == newPort)
                return;

            if (_selectedPort == null || _selectedPort.Type == newPort.Type || _selectedPort.Node == newPort.Node)
            {
                _selectedPort = newPort;
                return;
            }

            if (_selectedPort.Node.NodeAsset is HubNode && newPort.Node.NodeAsset is HubNode)
            {
                _selectedPort = newPort;
                return;
            }

            PortViewer outPort = newPort.Type == PortType.Out ? newPort : _selectedPort;
            PortViewer inPort = newPort.Type == PortType.In ? newPort : _selectedPort;

            if (!_transitionViewers.Any(item => item.Source == inPort && item.Destination == outPort))
            {
                outPort.Node.AddTransition(inPort.Node.NodeAsset);
                _transitionViewers.Add(new TransitionViewer(outPort, inPort, this));
            }

            _selectedPort = null;
        }

        public void DeleteTransition(TransitionViewer transition)
        {
            _transitionViewers.Remove(transition);
            transition.Destination.Node.RemoveReference(transition.Source.Node.NodeAsset);
        }

        public void CopySelectedNode()
        {
            List<NodeViewer> newNodes = new List<NodeViewer>();

            foreach (NodeViewer item in _nodeViewers.Where(item => item.IsSelected))
            {
                if (item.NodeAsset.ServiceNode())
                    continue;

                Rect rect = item.WorldRect;
                RawNode newNode = _graphAssetEditor.CreateNode(rect.position + Vector2.up * (rect.height + 30f), item.NodeAsset);
                NodeViewer newNodeEditor = newNodes.Place(new NodeViewer(newNode, this));
                CreateTransitionsForNode(newNodeEditor);
                item.Select(false);
                newNodeEditor.Select(true);
            }

            _nodeViewers.AddRange(newNodes);
        }

        private void CreateNode(Vector2 mousePosition, Type type)
        {
            Vector2 position = _camera.ScreenToWorld(mousePosition);
            RawNode nodeAsset = _graphAssetEditor.CreateNode(position, type);
            _nodeViewers.Add(new NodeViewer(nodeAsset, this));
            if (nodeAsset is ExitNode)
                _hasExitNode = true;
        }

        private void CreateTransitionsForNode(NodeViewer nodeViewer)
        {
            foreach (RawNode connectedNode in nodeViewer.ParseTransitionsList())
            {
                NodeViewer connectedNodeViewer = _nodeViewers.Find(itm => itm.NodeAsset == connectedNode);
                _transitionViewers.Add(new TransitionViewer(nodeViewer.Out, connectedNodeViewer.In, this));
            }
        }

        private void DrawNodes()
        {
            for (int i = 0; i < _nodeViewers.Count; i++)
            {
                _nodeViewers[i].Draw();
            }
        }

        private void DrawConnections()
        {
            for (int i = 0; i < _transitionViewers.Count; i++)
            {
                _transitionViewers[i].Draw();
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

            switch (_selectedPort.Type)
            {
                case PortType.In:
                    TransitionViewer.DrawLine(_selectedPort.ScreenRect.center, e.mousePosition, Vector2.left, Vector2.right);
                    break;

                case PortType.Out:
                    TransitionViewer.DrawLine(_selectedPort.ScreenRect.center, e.mousePosition, Vector2.right, Vector2.left);
                    break;
            }

            GUI.changed = true;
        }

        private void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
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
                    break;

                case EventType.MouseUp:
                    _selectionRectOn = false;
                    break;

                case EventType.KeyDown:
                    if (e.keyCode == KeyCode.D)
                    {
                        CopySelectedNode();
                        GUI.changed = true;
                    }
                    break;
            }
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

        private void ProcessTransitionsEvents(Event e)
        {
            if (_selectionRectOn)
                return;

            bool needLockEvent = false;

            for (int i = 0; i < _transitionViewers.Count; i++)
            {
                if (_transitionViewers[i].ProcessEvents(e))
                    needLockEvent = true;
            }

            if (needLockEvent)
                e.Use();
        }

        private void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu menu = new GenericMenu();

            addNodeMenuItem(_graphAssetEditor.NodeType);

            foreach (Type type in TypeCache.GetTypesDerivedFrom(_graphAssetEditor.NodeType))
            {
                addNodeMenuItem(type);
            }

            menu.AddSeparator(string.Empty);

            addServiceMenuItem(typeof(HubNode), false);
            addServiceMenuItem(typeof(ExitNode), _hasExitNode);

            menu.ShowAsContext();

            void addServiceMenuItem(Type type, bool disabled)
            {
                if (disabled)
                    menu.AddDisabledItem(new GUIContent(type.Name));
                else
                    menu.AddItem(new GUIContent(type.Name), false, () => CreateNode(mousePosition, type));
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
            _transitionViewers.ForEach(item => item.Save());
            _graphAssetEditor.Save();
            _toolbar.Save();
            _settings.CameraPosition = _camera.Position;
            SaveSettings(_graphAssetEditor.GraphAsset, _settings);
            EditorUtilityExt.SaveProject();
        }

        private static GraphEditorSettings LoadSettings(RawGraph asset)
        {
            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string guid, out long _))
            {
                string floderPath = Path.Combine(Application.persistentDataPath, "GraphSettings");
                Directory.CreateDirectory(floderPath);
                string filePath = Path.Combine(floderPath, guid);

                if (!File.Exists(filePath))
                    return new GraphEditorSettings();

                GraphEditorSettings graphSettings = BinaryFileUtility.Load<GraphEditorSettings>(filePath);
                return graphSettings ?? new GraphEditorSettings();
            }

            return new GraphEditorSettings();
        }

        private static void SaveSettings(RawGraph asset, GraphEditorSettings settings)
        {
            if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string guid, out long _))
            {
                Debug.LogWarning("Cannot get asset guid.");
                return;
            }

            string path = Path.Combine(Application.persistentDataPath, "GraphSettings");
            Directory.CreateDirectory(path);
            BinaryFileUtility.Save(Path.Combine(path, guid), settings);
        }
    }
}
