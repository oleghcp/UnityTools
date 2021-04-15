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

        private GraphAssetEditor _graphAssetEditor;
        private GraphCamera _camera;
        private GraphGrid _grid;
        private GraphToolbar _toolbar;
        private List<NodeViewer> _nodeViewers;
        private List<TransitionViewer> _transitionViewers;

        private PortViewer _selectedPort;
        private bool _selectionRectOn;
        private Vector2 _downPoint;

        public static bool GridSnapping;

        public GraphAssetEditor GraphAssetEditor => _graphAssetEditor;
        public IReadOnlyList<NodeViewer> NodeViewers => _nodeViewers;
        public GraphCamera Camera => _camera;

        public static void OpenWindow(Graph graphAsset)
        {
            GraphEditorWindow window = GetWindow<GraphEditorWindow>(true, "Graph Editor");
            window.minSize = new Vector2(800f, 600f);
            window.SetUp(graphAsset);
        }

        private void SetUp(Graph graphAsset)
        {
            if (_graphAssetEditor?.GraphAsset == graphAsset)
                return;

            if (_graphAssetEditor != null)
            {
                _nodeViewers.Clear();
                _transitionViewers.Clear();
            }

            _graphAssetEditor = new GraphAssetEditor(graphAsset);
            _camera = new GraphCamera(this, _graphAssetEditor.CameraPosition);

            foreach (RawNode item in _graphAssetEditor.ParseList())
            {
                _nodeViewers.Add(new NodeViewer(item, this));
            }

            for (int i = 0; i < _nodeViewers.Count; i++)
            {
                CreateTransitionsForNode(_nodeViewers[i]);
            }
        }

        private void OnEnable()
        {
            _grid = new GraphGrid(this);
            _toolbar = new GraphToolbar(this);
            _nodeViewers = new List<NodeViewer>();
            _transitionViewers = new List<TransitionViewer>();
        }

        private void OnGUI()
        {
            if (_graphAssetEditor.GraphAsset == null)
            {
                Close();
                return;
            }

            Event e = Event.current;

            ProcessCamera(e);
            _grid.Draw();

            DrawConnections();
            DrawConnectionLine(e);
            DrawNodes();
            DrawSelectionRect(e);
            GUILayout.Label(_graphAssetEditor.GraphAsset.name, EditorStyles.boldLabel);
            _toolbar.Draw();

            ProcessNodeEvents(e);
            ProcessTransitionsEvents(e);
            ProcessEvents(e);
        }

        private void OnDestroy()
        {
            if (_graphAssetEditor.GraphAsset == null)
                return;

            Save();
            EditorUtilityExt.SaveProject();
        }

        private void OnLostFocus()
        {
            Save();
            EditorUtilityExt.SaveProject();
        }

        public bool IsRootNode(RawNode node)
        {
            return _graphAssetEditor.RootNode == node;
        }

        public void SetAsRoot(NodeViewer node)
        {
            _graphAssetEditor.SetAsRoot(node.NodeAsset);
        }

        public void DeleteNode(NodeViewer node)
        {
            _transitionViewers.RemoveAll(item => item.In == node.In || item.Out == node.Out);
            _nodeViewers.Remove(node);
            _nodeViewers.ForEach(item => item.RemoveReference(node.NodeAsset));
            _graphAssetEditor.DestroyNode(node.NodeAsset);

            if (_nodeViewers.Count == 0)
                _camera.Position = default;
        }

        public void OnClickOnPort(PortViewer newPort)
        {
            if (_selectedPort == newPort)
                return;

            if (_selectedPort == null)
            {
                _selectedPort = newPort;
                return;
            }

            if (_selectedPort.Type != newPort.Type && _selectedPort.Node != newPort.Node)
            {
                PortViewer outPort = newPort.Type == PortType.Out ? newPort : _selectedPort;
                PortViewer inPort = newPort.Type == PortType.In ? newPort : _selectedPort;

                if (!_transitionViewers.Any(item => item.In == inPort && item.Out == outPort))
                {
                    SerializedProperty property = outPort.Node.AddTransition(inPort.Node.NodeAsset);
                    _transitionViewers.Add(new TransitionViewer(outPort, inPort, property, this));
                }
            }

            _selectedPort = null;
        }

        public void DeleteTransition(TransitionViewer transition)
        {
            _transitionViewers.Remove(transition);
            transition.Out.Node.RemoveReference(transition.In.Node.NodeAsset);
        }

        public void CopySelectedNode()
        {
            List<NodeViewer> newNodes = new List<NodeViewer>();

            foreach (NodeViewer item in _nodeViewers.Where(item => item.IsSelected))
            {
                Rect rect = item.GetRectInWorld();
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
        }

        private void CreateTransitionsForNode(NodeViewer nodeViewer)
        {
            foreach (var (transitionProp, connectedNode) in nodeViewer.ParseTransitionsList())
            {
                NodeViewer connectedNodeViewer = _nodeViewers.Find(itm => itm.NodeAsset == connectedNode);
                _transitionViewers.Add(new TransitionViewer(nodeViewer.Out, connectedNodeViewer.In, transitionProp, this));
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

                _nodeViewers.ForEach(item => item.Select(false));

                for (int i = 0; i < _nodeViewers.Count; i++)
                {
                    if (selectionRect.Overlaps(_nodeViewers[i].GetRectInScreen(), true))
                        _nodeViewers[i].Select(true);
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

        public void ProcessCamera(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDrag:
                    if (e.button == 2)
                    {
                        _camera.Drag(e.delta);
                        GUI.changed = true;
                    }
                    break;

                case EventType.ScrollWheel:
                    if (e.delta.y > 0f)
                        _camera.Size = 2f;
                    else
                        _camera.Size = 1f;
                    GUI.changed = true;
                    break;
            }
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

            addMenuItem(_graphAssetEditor.NodeType);

            foreach (Type type in TypeCache.GetTypesDerivedFrom(_graphAssetEditor.NodeType))
            {
                addMenuItem(type);
            }

            menu.ShowAsContext();

            void addMenuItem(Type type)
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
            _graphAssetEditor.Save(_camera.Position);
        }
    }
}
