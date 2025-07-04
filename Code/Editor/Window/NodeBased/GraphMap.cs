﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OlegHcp;
using OlegHcp.CSharp;
using OlegHcp.CSharp.Collections;
using OlegHcp.Engine;
using OlegHcp.NodeBased;
using OlegHcp.NodeBased.Service;
using OlegHcpEditor.Engine;
using OlegHcpEditor.NodeBased;
using OlegHcpEditor.Window.NodeBased.NodeDrawing;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Window.NodeBased
{
    internal class GraphMap
    {
        private readonly Color _selectionColor = Colours.Black.AlterA(0.25f);
        public readonly Vector2 UiOffset = new Vector2(10f, 10f);
        public readonly Vector2 UiShrink = new Vector2(20f, 22f);
        public readonly float NodeHeaderHeight;

        private GraphEditorWindow _window;
        private List<NodeViewer> _nodeViewers;
        private HashSet<NodeViewer> _selectedNodes = new HashSet<NodeViewer>();
        private HashSet<string> _nodeIgnoredFields;

        private bool _selectionRectOn;
        private bool _mouseDown;
        private Vector2 _downPoint;
        private PortViewer _selectedPort;
        private Dictionary<Type, NodeDrawer> _nodeDrawers;
        private NodeDrawer _regularNodeDrawer;
        private int _lastSelectionVersion;

        public IReadOnlyList<NodeViewer> NodeViewers => _nodeViewers;
        public int SelectionCount => _selectedNodes.Count;
        public bool CreatingTransition => _selectedPort != null;

        public GraphMap(GraphEditorWindow window)
        {
            NodeHeaderHeight = EditorGUIUtility.singleLineHeight + UiShrink.y;
            _window = window;

            _nodeIgnoredFields = new HashSet<string>
            {
                RawNode.NameFieldName,
                RawNode.ArrayFieldName,
                RawNode.GraphFieldName,
                RawNode.IdFieldName,
                RawNode.PositionFieldName,
            };

            _regularNodeDrawer = new NodeDrawer();
            _regularNodeDrawer.SetUp(_nodeIgnoredFields, _window.SidePanel);

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
                _nodeViewers.Add(new NodeViewer(nodeProp, this, _window));
            }

            if (_window.SerializedGraph.CommonNodeProperty.HasManagedReferenceValue())
            {
                _nodeViewers.Add(new NodeViewer(_window.SerializedGraph.CommonNodeProperty, this, _window));
            }

            _nodeViewers.ForEach(item => item.CreateConnections());
        }

        public void Draw(Event e)
        {
            GraphGrid.Draw(_window);

            DrawConnectionLine(e);
            DrawNodes();
            DrawSelectionRect(e);
            ReorderNodes();
            HandleNodeEvents(e);
            ReorderNodes();
            HandleMapEvents(e);
        }

        public void BeginTransitionFrom(NodeViewer source)
        {
            _selectedPort = source.Out;
        }

        public void FinishTransitionTo(NodeViewer destination)
        {
            if (_selectedPort.Node == destination)
                return;

            if (destination.Type == NodeType.Common)
            {
                Debug.LogWarning("Common node cannot have input.");
                return;
            }

            if (_selectedPort.Node.Type == NodeType.Hub && destination.Type == NodeType.Hub)
            {
                _selectedPort = destination.Out;
                Debug.LogWarning("Hub nodes cannot be connected to each other.");
                return;
            }

            _selectedPort.Node.CreateTransition(destination.In);

            _selectedPort = null;
        }

        public void OnPortSelected(PortViewer targetPort)
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
                Debug.LogWarning("Hub nodes cannot be connected to each other.");
                return;
            }

            PortViewer source = targetPort.Type == PortType.Out ? targetPort : _selectedPort;
            PortViewer destination = targetPort.Type == PortType.In ? targetPort : _selectedPort;

            source.Node.CreateTransition(destination);

            _selectedPort = null;
        }

        public void DeleteNode(NodeViewer node)
        {
            SerializedGraph serializedGraph = _window.SerializedGraph;

            serializedGraph.SerializedObject.Update();

            OnNodeSelectionChanged(node, false);
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

        public NodeDrawer GetNodeDrawer(Type nodeType)
        {
            if (_nodeDrawers.TryGetValue(nodeType, out NodeDrawer drawer))
                return drawer;

            Type drawerType = TypeCache.GetTypesDerivedFrom(typeof(NodeDrawer))
                                       .Where(item => !item.IsAbstract && item.IsDefined(typeof(CustomNodeDrawerAttribute), true))
                                       .FirstOrDefault(item => item.GetCustomAttribute<CustomNodeDrawerAttribute>().NodeType == nodeType);
            if (drawerType == null)
                return _nodeDrawers.Place(nodeType, _regularNodeDrawer);

            NodeDrawer nodeDrawer = (NodeDrawer)drawerType.CreateInstance();
            nodeDrawer.SetUp(_nodeIgnoredFields, _window.SidePanel);
            return _nodeDrawers.Place(nodeType, nodeDrawer);
        }

        public NodeViewer GetSelectedNode()
        {
            foreach (NodeViewer node in _selectedNodes)
            {
                return node;
            }

            return null;
        }

        public void Save()
        {
            _nodeViewers.ForEach(item => item.Save());
        }

        public void OnNodeSelectionChanged(NodeViewer node, bool on)
        {
            if (on)
            {
                _lastSelectionVersion = node.SelectionVersion;
                _selectedNodes.Add(node);
            }
            else
            {
                _selectedNodes.Remove(node);
            }
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
                    bool overlaps = selectionRect.Overlaps(_nodeViewers[i].ScreenRect, true);
                    _nodeViewers[i].Select(overlaps);
                }

                GUI.changed = true;
            }
        }

        private void DrawConnectionLine(Event e)
        {
            if (_selectedPort == null)
                return;

            if (_window.ShowPorts)
            {
                Vector2 startTangentDir = _selectedPort.Type == PortType.In ? Vector2.left : Vector2.right;
                Vector2 endTangentDir = _selectedPort.Type == PortType.In ? Vector2.right : Vector2.left;
                TransitionViewer.DrawSpline(_selectedPort.ScreenRect.center, e.mousePosition, startTangentDir, endTangentDir);
            }
            else
            {
                TransitionViewer.DrawDirection(_selectedPort.Node.ScreenRect.center, e.mousePosition);
            }

            GUI.changed = true;
        }

        private void HandleMapEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    switch (e.button)
                    {
                        case 0:
                            if (!_selectionRectOn)
                            {
                                _mouseDown = true;
                                _downPoint = e.mousePosition;
                                _selectedPort = null;
                            }
                            break;

                        case 1:
                            _selectionRectOn = false;
                            OpenContextMenu(e.mousePosition);
                            break;
                    }
                    break;

                case EventType.MouseDrag:
                    if (_mouseDown && e.button == 0)
                        _selectionRectOn = true;
                    break;

                case EventType.MouseUp:
                    if (e.button == 0)
                    {
                        _mouseDown = false;
                        _selectionRectOn = false;
                    }
                    break;
            }
        }

        private void HandleNodeEvents(Event e)
        {
            if (_selectionRectOn)
                return;

            bool needLockEvent = false;
            bool pointed = false;

            for (int i = _nodeViewers.Count - 1; i >= 0; i--)
            {
                if (!pointed && _nodeViewers[i].ScreenRect.Contains(e.mousePosition))
                {
                    needLockEvent |= _nodeViewers[i].HandleBaseEventsUnderPointer(e);
                    pointed = true;
                }
                else
                {
                    needLockEvent |= _nodeViewers[i].HandleBaseEventsOutOfPointer(e);
                }

                _nodeViewers[i].HandleBaseEvents(e);
            }

            if (needLockEvent)
            {
                e.Use();
                return;
            }

            for (int i = 0; i < _nodeViewers.Count; i++)
            {
                if (_nodeViewers[i].HandleLineEvents(e))
                {
                    e.Use();
                    break;
                }
            }
        }

        private void ReorderNodes()
        {
            if (_nodeViewers.Count == 0 || _selectedNodes.Count == 0)
                return;

            if (_nodeViewers.FromEnd(0).SelectionVersion == _lastSelectionVersion)
                return;

            _nodeViewers.Sort();
        }

        private void OpenContextMenu(Vector2 mousePosition)
        {
            SerializedGraph serializedGraph = _window.SerializedGraph;

            GenericMenu menu = new GenericMenu();

            addServiceMenuItem(typeof(CommonNode), "Common", serializedGraph.CommonNodeProperty.HasManagedReferenceValue());
            addServiceMenuItem(typeof(HubNode), "Hub", false);
            addServiceMenuItem(typeof(ExitNode), "Exit", _nodeViewers.Contains(item => item.Type == NodeType.Exit));

            menu.AddSeparator(string.Empty);

            Type nodeType = serializedGraph.GraphAsset.GetNodeRootType();

            if (!nodeType.IsAssignableTo(serializedGraph.GraphAsset.RootNodeType))
            {
                Debug.LogError($"{nodeType} is not assignable to {serializedGraph.GraphAsset.RootNodeType}.");
                return;
            }

            addNodeMenuItem(nodeType);

            foreach (Type type in TypeCache.GetTypesDerivedFrom(nodeType))
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
            _nodeViewers.Add(new NodeViewer(nodeProp, this, _window));

            _window.SerializedGraph.SerializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
