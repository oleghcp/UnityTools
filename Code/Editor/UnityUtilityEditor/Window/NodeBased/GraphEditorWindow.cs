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
        private int _rootNodeId;

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

        public int RootNodeId
        {
            get
            {
                if (_rootNodeVersion != _onGuiCounter)
                {
                    _rootNodeId = _graphAssetEditor.GraphAsset.RootNode.Id;
                    _rootNodeVersion = _onGuiCounter;
                }

                return _rootNodeId;
            }
        }

        private void OnEnable()
        {
            minSize = new Vector2(800f, 600f);
        }

        private void OnGUI()
        {
            if (_graphAssetEditor.GraphAsset == null)
            {
                Close();
                return;
            }

            _graphAssetEditor.SerializedObject.Update();

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

            DrawConnectionLine(e);
            DrawNodes();
            DrawSelectionRect(e);

            ProcessNodeEvents(e);
            ProcessEvents(e);

            GUI.EndGroup();

            _graphAssetEditor.SerializedObject.ApplyModifiedProperties();
        }

        private void OnDestroy()
        {
            if (_graphAssetEditor.GraphAsset == null)
                return;

            Save();
        }

        //private void OnLostFocus()
        //{
        //    Save();
        //}

        public void SetUp(RawGraph graphAsset)
        {
            if (_graphAssetEditor?.GraphAsset == graphAsset)
                return;

            if (_graphAssetEditor != null)
            {
                _nodeViewers.Clear();
            }
            else
            {
                _grid = new GraphGrid(this);
                _toolbar = new GraphToolbar(this);
                _nodeViewers = new List<NodeViewer>();
                _camera = new GraphCamera(this);
            }

            _settings = LoadSettings(graphAsset);
            _graphAssetEditor = new GraphAssetEditor(graphAsset);
            _camera.Position = _settings.CameraPosition;

            foreach (SerializedProperty nodeProp in _graphAssetEditor.NodesProperty.EnumerateArrayElements())
            {
                NodeViewer viewer = _nodeViewers.Place(new NodeViewer(nodeProp, this));
                if (viewer.Type == NodeType.Exit)
                    _hasExitNode = true;
            }

            _nodeViewers.ForEach(item => item.CreateConnections());
        }

        public void SetAsRoot(NodeViewer node)
        {
            _graphAssetEditor.SerializedObject.Update();
            _graphAssetEditor.SetAsRoot(node);
            _graphAssetEditor.SerializedObject.ApplyModifiedProperties();
        }

        public void DeleteNode(NodeViewer node)
        {
            _graphAssetEditor.SerializedObject.Update();

            _nodeViewers.Remove(node);
            _nodeViewers.ForEach(item => item.RemoveTransition(node));
            _graphAssetEditor.DestroyNode(node.Id);

            foreach (SerializedProperty nodeProp in _graphAssetEditor.NodesProperty.EnumerateArrayElements())
            {
                int id = nodeProp.FindPropertyRelative(RawNode.IdFieldName).intValue;
                _nodeViewers.Find(item => item.Id == id).ReinitSerializedProperty(nodeProp);
            }

            if (node.Type == NodeType.Exit)
                _hasExitNode = false;

            if (_nodeViewers.Count == 0)
                _camera.Position = default;

            _graphAssetEditor.SerializedObject.ApplyModifiedProperties();
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

        public void CopySelectedNode()
        {
            _graphAssetEditor.SerializedObject.Update();

            List<NodeViewer> newNodes = new List<NodeViewer>();

            foreach (NodeViewer item in _nodeViewers.Where(item => item.IsSelected))
            {
                if (item.Type.ServiceNode())
                    continue;

                Rect rect = item.WorldRect;
                SerializedProperty nodeProp = _graphAssetEditor.CloneNode(rect.position + Vector2.up * (rect.height + 30f), item.Id);
                NodeViewer newNodeEditor = newNodes.Place(new NodeViewer(nodeProp, this));
                item.Select(false);
                newNodeEditor.Select(true);
            }

            _nodeViewers.AddRange(newNodes);

            _graphAssetEditor.SerializedObject.ApplyModifiedProperties();
        }

        private void CreateNode(Vector2 mousePosition, Type type)
        {
            _graphAssetEditor.SerializedObject.Update();

            Vector2 position = _camera.ScreenToWorld(mousePosition);
            SerializedProperty nodeProp = _graphAssetEditor.CreateNode(position, type);
            NodeViewer viewer = _nodeViewers.Place(new NodeViewer(nodeProp, this));
            if (viewer.Type == NodeType.Exit)
                _hasExitNode = true;

            _graphAssetEditor.SerializedObject.ApplyModifiedProperties();
        }

        private void DrawNodes()
        {
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
            _graphAssetEditor.Save();
            _toolbar.Save();
            _graphAssetEditor.SerializedObject.ApplyModifiedPropertiesWithoutUndo();
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
