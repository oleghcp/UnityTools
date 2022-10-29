#if UNITY_2019_3_OR_NEWER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.NodeBased;
using UnityUtilityEditor.Window.NodeBased.Stuff.NodeDrawing;

namespace UnityUtilityEditor.Window.NodeBased.Stuff
{
    internal class GraphMap
    {
        private readonly Color _selectionColor = Colours.Black.AlterA(0.25f);
        public readonly Vector2 UiOffset = new Vector2(10f, 10f);
        public readonly Vector2 UiShrink = new Vector2(20f, 22f);
        public readonly float NodeHeaderHeight;

        private GraphEditorWindow _window;
        private GraphGrid _grid;
        private List<NodeViewer> _nodeViewers;
        private HashSet<NodeViewer> _selectedNodes = new HashSet<NodeViewer>();
        private HashSet<string> _nodeIgnoredFields;

        private bool _selectionRectOn;
        private Vector2 _downPoint;
        private PortViewer _selectedPort;
        private Dictionary<Type, NodeDrawer> _nodeDrawers;
        private NodeDrawer _regularNodeDrawer;

        public GraphEditorWindow Window => _window;
        public IReadOnlyList<NodeViewer> NodeViewers => _nodeViewers;
        public int SelectionCount => _selectedNodes.Count;

        public GraphMap(GraphEditorWindow window)
        {
            NodeHeaderHeight = EditorGUIUtility.singleLineHeight + UiShrink.y;
            _window = window;
            _grid = new GraphGrid(window);

            _nodeIgnoredFields = new HashSet<string>
            {
                RawNode.NameFieldName,
                RawNode.ArrayFieldName,
                RawNode.GraphFieldName,
                RawNode.IdFieldName,
                RawNode.PositionFieldName,
            };

            _regularNodeDrawer = new NodeDrawer() { Map = this };
            _nodeDrawers = new Dictionary<Type, NodeDrawer>()
            {
                { typeof(HubNode), new ServiceNodeDrawer("► ► ►", Colours.Silver) },
                { typeof(CommonNode), new ServiceNodeDrawer("[ . . . ]", Colours.Yellow) },
                { typeof(ExitNode), new ServiceNodeDrawer("→ █", Colours.Red) },
            };
            _nodeViewers = new List<NodeViewer>();
        }

        public void Clear()
        {
            _nodeViewers.Clear();
            _selectedNodes.Clear();
        }

        public void Fill()
        {
            foreach (SerializedProperty nodeProp in _window.SerializedGraph.NodesProperty.EnumerateArrayElements())
            {
                _nodeViewers.Add(new NodeViewer(nodeProp, this));
            }

            if (_window.SerializedGraph.CommonNodeProperty.HasManagedReferenceValue())
            {
                _nodeViewers.Add(new NodeViewer(_window.SerializedGraph.CommonNodeProperty, this));
            }

            _nodeViewers.ForEach(item => item.CreateConnections());
        }

        public bool IsServiceField(SerializedProperty property)
        {
            return _nodeIgnoredFields.Contains(property.name);
        }

        public void Draw(Event e)
        {
            _grid.Draw();

            DrawConnectionLine(e);
            DrawNodes();
            DrawSelectionRect(e);

            ProcessNodeEvents(e);
            ProcessEvents(e);
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

        public void DeleteNode(NodeViewer node)
        {
            SerializedGraph serializedGraph = _window.SerializedGraph;

            serializedGraph.SerializedObject.Update();

            _nodeViewers.Remove(node);
            _nodeViewers.ForEach(item => item.RemoveTransition(node));
            serializedGraph.RemoveNode(node);

            foreach (SerializedProperty nodeProp in serializedGraph.NodesProperty.EnumerateArrayElements())
            {
                int id = nodeProp.FindPropertyRelative(RawNode.IdFieldName).intValue;
                _nodeViewers.Find(item => item.Id == id).SetSerializedProperty(nodeProp);
            }

            if (_nodeViewers.Count == 0)
                _window.Camera.Position = default;

            serializedGraph.SerializedObject.ApplyModifiedPropertiesWithoutUndo();
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

        public NodeDrawer GetDrawer(Type nodeType)
        {
            if (_nodeDrawers.TryGetValue(nodeType, out NodeDrawer drawer))
                return drawer;

            Type drawerType = TypeCache.GetTypesDerivedFrom(typeof(NodeDrawer))
                                       .Where(item => !item.IsAbstract && item.IsDefined(typeof(CustomNodeDrawerAttribute), true))
                                       .FirstOrDefault(item => item.GetCustomAttribute<CustomNodeDrawerAttribute>().NodeType == nodeType);
            if (drawerType == null)
                return _nodeDrawers.Place(nodeType, _regularNodeDrawer);

            return _nodeDrawers.Place(nodeType, (NodeDrawer)Activator.CreateInstance(drawerType));
        }

        public NodeViewer GetSelectedNode()
        {
            var iterator = _selectedNodes.GetEnumerator();
            iterator.MoveNext();
            return iterator.Current;
        }

        public void Save()
        {
            _nodeViewers.ForEach(item => item.Save());
        }

        public void OnNodeSelectionChanched(NodeViewer node, bool on)
        {
            if (on)
                _selectedNodes.Add(node);
            else
                _selectedNodes.Remove(node);
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

                Handles.DrawSolidRectangleWithOutline(selectionRect, _selectionColor, GraphEditorStyles.GetLineColor());

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

            switch (_window.TransitionView)
            {
                case TransitionViewType.Splines:
                    Vector2 startTangentDir = _selectedPort.Type == PortType.In ? Vector2.left : Vector2.right;
                    Vector2 endTangentDir = _selectedPort.Type == PortType.In ? Vector2.right : Vector2.left;
                    TransitionViewer.DrawSpline(_selectedPort.ScreenRect.center, e.mousePosition, startTangentDir, endTangentDir);
                    break;

                case TransitionViewType.Arrows:
                    TransitionViewer.DrawDirection(_selectedPort.Node.ScreenRect.center, e.mousePosition);
                    break;

                default:
                    throw new UnsupportedValueException(_window.TransitionView);
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
            SerializedGraph serializedGraph = _window.SerializedGraph;

            GenericMenu menu = new GenericMenu();

            addServiceMenuItem(typeof(CommonNode), "Common", serializedGraph.CommonNodeProperty.HasManagedReferenceValue());
            addServiceMenuItem(typeof(HubNode), "Hub", false);
            addServiceMenuItem(typeof(ExitNode), "Exit", _nodeViewers.Contains(item => item.Type == NodeType.Exit));

            menu.AddSeparator(string.Empty);

            addNodeMenuItem(serializedGraph.GraphAsset.GetNodeType());

            foreach (Type type in TypeCache.GetTypesDerivedFrom(serializedGraph.GraphAsset.GetNodeType()))
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

        private void CreateNode(Vector2 mousePosition, Type type)
        {
            _window.SerializedGraph.SerializedObject.Update();

            Vector2 position = _window.Camera.ScreenToWorld(mousePosition);
            SerializedProperty nodeProp = _window.SerializedGraph.CreateNode(position, type);
            _nodeViewers.Add(new NodeViewer(nodeProp, this));

            _window.SerializedGraph.SerializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
#endif
