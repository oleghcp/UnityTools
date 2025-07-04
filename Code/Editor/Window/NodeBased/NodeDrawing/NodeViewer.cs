﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using OlegHcp.Mathematics;
using OlegHcp.NodeBased.Service;
using OlegHcpEditor.Engine;
using OlegHcpEditor.NodeBased;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Window.NodeBased.NodeDrawing
{
    internal class NodeViewer : IComparable<NodeViewer>
    {
        private const float SERV_NODE_WIDTH = 150f;

        private static int _selectionCounter;

        private readonly int _id;
        private readonly Type _systemType;
        private readonly NodeType _type;
        private readonly PortViewer _in;
        private readonly PortViewer _out;
        private readonly GraphMap _map;
        private readonly GraphEditorWindow _window;
        private readonly List<TransitionViewer> _lineViewers;

        private NodeDrawer _nodeDrawer;
        private SerializedProperty _nodeProp;
        private SerializedProperty _nameProp;
        private int _selectionVersion;
        private bool _isSelected;
        private bool _renaming;
        private float _height;

        private Vector2 _position;
        private Vector2 _draggedPosition;

        private float _width;
        private Rect _screenRect;
        private Rect _worldRect;
        private bool _isInCamera;

        private int _heightVersion;
        private int _widthVersion;
        private int _screenRectVersion;
        private int _worldRectVersion;
        private int _overlapVersion;

        public PortViewer In => _in;
        public PortViewer Out => _out;
        public bool IsSelected => _isSelected;
        public int Id => _id;
        public NodeType Type => _type;
        public Type SystemType => _systemType;

        public IReadOnlyList<TransitionViewer> LineViewers => _lineViewers;
        public SerializedProperty NodeProp => _nodeProp;
        public SerializedProperty NameProp => _nameProp;
        public NodeDrawer NodeDrawer => _nodeDrawer;
        public int SelectionVersion => _selectionVersion;

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public float Width
        {
            get
            {
                if (_widthVersion != GraphEditorWindow.OnGuiCounter)
                {
                    _width = _type.IsServiceNode() ? SERV_NODE_WIDTH : _window.SerializedGraph.NodeWidth;
                    _widthVersion = GraphEditorWindow.OnGuiCounter;
                }

                return _width;
            }
        }

        public Rect ScreenRect
        {
            get
            {
                if (_screenRectVersion != GraphEditorWindow.OnGuiCounter)
                {
                    _screenRect.position = _window.Camera.WorldToScreen(_position);
                    _screenRect.width = Width / _window.Camera.Size;
                    _screenRect.height = _window.Camera.Size > 1f ? _map.NodeHeaderHeight : GetBigNodeHeight();
                    _screenRectVersion = GraphEditorWindow.OnGuiCounter;
                }

                return _screenRect;
            }
        }

        public Rect WorldRect
        {
            get
            {
                if (_worldRectVersion != GraphEditorWindow.OnGuiCounter)
                {
                    _worldRect.position = _position;
                    _worldRect.width = Width;
                    _worldRect.height = _window.Camera.Size > 1f ? _map.NodeHeaderHeight : GetBigNodeHeight();
                    _worldRectVersion = GraphEditorWindow.OnGuiCounter;
                }

                return _worldRect;
            }
        }

        public bool IsInCamera
        {
            get
            {
                if (_overlapVersion != GraphEditorWindow.OnGuiCounter)
                {
                    _isInCamera = _window.Camera.WorldRect.Overlaps(WorldRect);
                    _overlapVersion = GraphEditorWindow.OnGuiCounter;
                }

                return _isInCamera;
            }
        }

        public NodeViewer(SerializedProperty nodeProp, GraphMap map, GraphEditorWindow window)
        {
            _map = map;
            _window = window;

            SetSerializedProperty(nodeProp);

            _id = nodeProp.FindPropertyRelative(RawNode.IdFieldName).intValue;
            _systemType = EditorUtilityExt.GetTypeFromSerializedPropertyTypename(nodeProp.managedReferenceFullTypename);
            _type = GraphUtility.GetNodeType(_systemType);
            _position = nodeProp.FindPropertyRelative(RawNode.PositionFieldName).vector2Value;
            _lineViewers = new List<TransitionViewer>();
            _in = new PortViewer(this, PortType.In, _map);
            _out = new PortViewer(this, PortType.Out, _map);
            _nodeDrawer = _map.GetNodeDrawer(_systemType);
        }

        public void SetSerializedProperty(SerializedProperty nodeProp)
        {
            _nodeProp = nodeProp;
            _nameProp = nodeProp.FindPropertyRelative(RawNode.NameFieldName);
        }

        public void CreateConnections()
        {
            foreach (SerializedProperty transitionProp in _nodeProp.FindPropertyRelative(RawNode.ArrayFieldName).EnumerateArrayElements())
            {
                int nextNodeId = transitionProp.FindPropertyRelative(Transition.NodeIdFieldName).intValue;
                NodeViewer connectedNodeViewer = _map.NodeViewers.First(itm => itm.Id == nextNodeId);
                _lineViewers.Add(new TransitionViewer(_out, connectedNodeViewer.In, _window));
            }
        }

        public void Select(bool on)
        {
            SelectInternal(on);
        }

        public bool CreateTransition(PortViewer destination)
        {
            if (_lineViewers.Any(item => item.Destination.Node == destination.Node))
                return false;

            SerializedProperty transitionsProperty = _nodeProp.FindPropertyRelative(RawNode.ArrayFieldName);
            SerializedProperty newItem = transitionsProperty.AddArrayElement();

            newItem.FindPropertyRelative(Transition.NodeIdFieldName).intValue = destination.Node.Id;
            newItem.FindPropertyRelative(Transition.ConditionFieldName).managedReferenceValue = null;
            newItem.FindPropertyRelative(Transition.PointsFieldName).ClearArray();

            _lineViewers.Add(new TransitionViewer(_out, destination, _window));

            return true;
        }

        public void RemoveTransition(NodeViewer nextNodeViewer)
        {
            TransitionViewer transition = _lineViewers.Find(item => item.Destination.Node == nextNodeViewer);
            if (transition != null)
                RemoveTransition(transition);
        }

        public void RemoveTransition(TransitionViewer transition)
        {
            _lineViewers.Remove(transition);
            NodeViewer next = transition.Destination.Node;

            SerializedProperty transitionsProperty = _nodeProp.FindPropertyRelative(RawNode.ArrayFieldName);
            int index = transitionsProperty.GetArrayElement(out _, item => item.FindPropertyRelative(Transition.NodeIdFieldName).intValue == next.Id);
            transitionsProperty.DeleteArrayElementAtIndex(index);
        }

        public void Draw()
        {
            if (!IsInCamera)
                return;

            drawPorts();

            Rect nodeRect = ScreenRect;

            GUI.Box(nodeRect, (string)null, _isSelected ? GraphEditorStyles.Styles.NodeSelected : GraphEditorStyles.Styles.NodeRegular);

            nodeRect.position += _map.UiOffset;
            nodeRect.size -= _map.UiShrink;

            GUILayout.BeginArea(nodeRect);
            _nodeDrawer.DrawHeader(_window.RootNodeId == _id, _nameProp, ref _renaming);
            if (_window.Camera.Size <= 1f)
                _nodeDrawer.Draw(_nodeProp, nodeRect.width, _window.FullDrawing);
            GUILayout.EndArea();

            void drawPorts()
            {
                if (_window.ShowPorts)
                {
                    if (_type != NodeType.Common) _in.Draw();
                    if (_type != NodeType.Exit) _out.Draw();
                }
            }
        }

        public void DrawTransitions()
        {
            if (_window.ShowPorts)
            {
                for (int i = 0; i < _lineViewers.Count; i++)
                {
                    _lineViewers[i].DrawSpline();
                }
            }
            else
            {
                for (int i = 0; i < _lineViewers.Count; i++)
                {
                    _lineViewers[i].DrawArrow();
                }
            }
        }

        public bool HandleBaseEventsUnderPointer(Event e)
        {
            bool needLock = false;

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (_map.CreatingTransition)
                    {
                        _map.FinishTransitionTo(this);
                    }
                    else
                    {
                        switch (e.button)
                        {
                            case 0:
                                if (e.control && _isSelected)
                                {
                                    SelectInternal(false);
                                    needLock = true;
                                    GUI.changed = true;
                                }
                                else
                                {
                                    _draggedPosition = _position;
                                    SelectInternal(true);
                                    needLock = true;
                                    GUI.changed = true;
                                }
                                break;

                            case 1:
                                SelectInternal(true);
                                needLock = true;
                                OpenContextMenu();
                                GUI.changed = true;
                                break;
                        }
                    }
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && _isSelected)
                    {
                        Drag(e.delta);
                        GUI.changed = true;
                        needLock = true;
                    }
                    break;
            }

            return needLock;
        }

        public bool HandleBaseEventsOutOfPointer(Event e)
        {
            bool needLock = false;

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (_renaming)
                    {
                        _renaming = false;
                        GUI.FocusControl(null);
                        GUI.changed = true;
                    }

                    if (!(_isSelected && e.control))
                    {
                        SelectInternal(false);
                        GUI.FocusControl(null);
                        GUI.changed = true;
                    }
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && _isSelected)
                    {
                        Drag(e.delta);
                        GUI.changed = true;
                        needLock = true;
                    }
                    break;
            }

            return needLock;
        }

        public void HandleBaseEvents(Event e)
        {
            if (e.type == EventType.KeyDown)
            {
                switch (e.keyCode)
                {
                    case KeyCode.Delete:
                        if (_isSelected)
                        {
                            _map.DeleteNode(this);
                            GUI.changed = true;
                        }
                        break;

                    case KeyCode.Return:
                        if (_renaming)
                        {
                            _renaming = false;
                            GUI.changed = true;
                        }
                        break;
                }
            }
        }

        public bool HandleLineEvents(Event e)
        {
            bool needLock = false;

            if (IsInCamera)
            {
                for (int i = 0; i < _lineViewers.Count; i++)
                {
                    if (_lineViewers[i].HandleEvents(e, _window.ShowPorts))
                        needLock = true;
                }
            }

            return needLock;
        }

        public void Save()
        {
            SerializedProperty positionProperty = _nodeProp.FindPropertyRelative(RawNode.PositionFieldName);
            positionProperty.vector2Value = _position;
            _lineViewers.ForEach(item => item.Save());
        }

        public int CompareTo(NodeViewer other)
        {
            return _selectionVersion.CompareTo(other._selectionVersion);
        }

        private void OpenContextMenu()
        {
            Vector2 clickPosition = Event.current.mousePosition;
            GenericMenu genericMenu = new GenericMenu();

            string setAsRoot = "Set as root";
            string addTransition = "Add Transition";
            string delete = "Delete";
            string info = "Info";

            switch (_type)
            {
                case NodeType.Regular:
                    if (!_window.ShowPorts)
                        genericMenu.AddItem(new GUIContent(addTransition), false, () => _map.BeginTransitionFrom(this));
                    genericMenu.AddItem(new GUIContent("Rename"), false, () => _renaming = true);
                    genericMenu.AddItem(new GUIContent("Set default name"), false, () => renameAsset());

                    if (_window.RootNodeId != _id)
                        genericMenu.AddItem(new GUIContent(setAsRoot), false, () => _window.SetAsRoot(this));
                    else
                        genericMenu.AddDisabledItem(new GUIContent(setAsRoot));

                    //genericMenu.AddItem(new GUIContent("Duplicate"), false, () => _field.CopySelectedNode());
                    //genericMenu.AddSeparator(null);
                    genericMenu.AddItem(new GUIContent(delete), false, () => _map.DeleteNode(this));
                    genericMenu.AddSeparator(null);
                    genericMenu.AddItem(new GUIContent(info), false, () => NodeInfoPopup.Open(this, _window));
                    break;

                case NodeType.Hub:
                    if (!_window.ShowPorts)
                        genericMenu.AddItem(new GUIContent(addTransition), false, () => _map.BeginTransitionFrom(this));
                    genericMenu.AddItem(new GUIContent(delete), false, () => _map.DeleteNode(this));
                    genericMenu.AddSeparator(null);
                    genericMenu.AddItem(new GUIContent(info), false, () => NodeInfoPopup.Open(this, _window));
                    break;

                case NodeType.Common:
                    if (!_window.ShowPorts)
                        genericMenu.AddItem(new GUIContent(addTransition), false, () => _map.BeginTransitionFrom(this));
                    genericMenu.AddItem(new GUIContent(delete), false, () => _map.DeleteNode(this));
                    genericMenu.AddSeparator(null);
                    genericMenu.AddItem(new GUIContent(info), false, () => NodeInfoPopup.Open(this, _window));
                    break;

                case NodeType.Exit:
                    genericMenu.AddItem(new GUIContent(delete), false, () => _map.DeleteNode(this));
                    break;

                default:
                    throw new SwitchExpressionException(_type);
            }

            genericMenu.ShowAsContext();

            void renameAsset()
            {
                string defaultName = SerializedGraph.GetDefaultNodeName(_systemType, _id);
                _nodeProp.serializedObject.Update();
                _nameProp.stringValue = defaultName;
                _nodeProp.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private float GetBigNodeHeight()
        {
            if (_heightVersion != GraphEditorWindow.OnGuiCounter)
            {
                _height = _map.NodeHeaderHeight + _nodeDrawer.GetHeight(_nodeProp, _window.FullDrawing);
                _heightVersion = GraphEditorWindow.OnGuiCounter;
            }

            return _height;
        }

        private void Drag(Vector2 mouseDelta)
        {
            if (_window.GridSnapping)
            {
                _draggedPosition += mouseDelta * _window.Camera.Size;
                _position = new Vector2(_draggedPosition.x.Round(GraphGrid.SMALL_STEP), _draggedPosition.y.Round(GraphGrid.SMALL_STEP));
            }
            else
            {
                _position += mouseDelta * _window.Camera.Size;
            }
        }

        private void SelectInternal(bool on)
        {
            if (_isSelected == on)
                return;

            if (on)
                _selectionVersion = ++_selectionCounter;

            _isSelected = on;
            _map.OnNodeSelectionChanged(this, on);
        }
    }
}
