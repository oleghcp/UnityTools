using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.Mathematics;
using UnityUtility.NodeBased;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Window.NodeBased.NodeDrawing
{
    internal class NodeViewer
    {
        private const float SERV_NODE_WIDTH = 150f;

        private readonly int _id;
        private readonly Type _systemType;
        private readonly NodeType _type;
        private readonly PortViewer _in;
        private readonly PortViewer _out;
        private readonly GraphMap _map;
        private readonly GraphEditorWindow _window;
        private readonly List<TransitionViewer> _transitionViewers;

        private NodeDrawer _nodeDrawer;
        private SerializedProperty _nodeProp;
        private SerializedProperty _nameProp;

        private bool _isSelected;
        private bool _isDragging;
        private bool _renaming;
        private float _height;

        private Vector2 _position;
        private Vector2 _dragedPosition;

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

        public IReadOnlyList<TransitionViewer> TransitionViewers => _transitionViewers;
        public SerializedProperty NodeProp => _nodeProp;
        public SerializedProperty NameProp => _nameProp;
        public NodeDrawer NodeDrawer => _nodeDrawer;

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public float Width
        {
            get
            {
                if (_widthVersion != _window.OnGuiCounter)
                {
                    _width = _type.ServiceNode() ? SERV_NODE_WIDTH : _window.SerializedGraph.NodeWidth;
                    _widthVersion = _window.OnGuiCounter;
                }

                return _width;
            }
        }

        public Rect ScreenRect
        {
            get
            {
                if (_screenRectVersion != _window.OnGuiCounter)
                {
                    _screenRect.position = _window.Camera.WorldToScreen(_position);
                    _screenRect.width = Width / _window.Camera.Size;
                    _screenRect.height = _window.Camera.Size > 1f ? _map.NodeHeaderHeight : GetBigNodeHeight();
                    _screenRectVersion = _window.OnGuiCounter;
                }

                return _screenRect;
            }
        }

        public Rect WorldRect
        {
            get
            {
                if (_worldRectVersion != _window.OnGuiCounter)
                {
                    _worldRect.position = _position;
                    _worldRect.width = Width;
                    _worldRect.height = _window.Camera.Size > 1f ? _map.NodeHeaderHeight : GetBigNodeHeight();
                    _worldRectVersion = _window.OnGuiCounter;
                }

                return _worldRect;
            }
        }

        public bool IsInCamera
        {
            get
            {
                if (_overlapVersion != _window.OnGuiCounter)
                {
                    _isInCamera = _window.Camera.WorldRect.Overlaps(WorldRect);
                    _overlapVersion = _window.OnGuiCounter;
                }

                return _isInCamera;
            }
        }

        public NodeViewer(SerializedProperty nodeProp, GraphMap map)
        {
            _map = map;
            _window = map.Window;

            SetSerializedProperty(nodeProp);

            _id = nodeProp.FindPropertyRelative(RawNode.IdFieldName).intValue;
            _systemType = EditorUtilityExt.GetTypeFromSerializedPropertyTypename(nodeProp.managedReferenceFullTypename);
            _type = RawNode.GetNodeType(_systemType);
            _position = nodeProp.FindPropertyRelative(RawNode.PositionFieldName).vector2Value;
            _transitionViewers = new List<TransitionViewer>();
            _in = new PortViewer(this, PortType.In, _map);
            _out = new PortViewer(this, PortType.Out, _map);
            _nodeDrawer = _map.GetDrawer(_systemType);
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
                _transitionViewers.Add(new TransitionViewer(_out, connectedNodeViewer.In, _window));
            }
        }

        public void Select(bool on)
        {
            SelectInternal(on);
        }

        public void CreateTransition(PortViewer dest)
        {
            if (_transitionViewers.Any(item => item.Destination.Node.Id == dest.Node.Id))
                return;

            SerializedProperty transitionsProperty = _nodeProp.FindPropertyRelative(RawNode.ArrayFieldName);
            SerializedProperty newItem = transitionsProperty.AddArrayElement();

            newItem.FindPropertyRelative(Transition.NodeIdFieldName).intValue = dest.Node.Id;
            newItem.FindPropertyRelative(Transition.ConditionFieldName).managedReferenceValue = null;
            newItem.FindPropertyRelative(Transition.PointsFieldName).ClearArray();

            _transitionViewers.Add(new TransitionViewer(_out, dest, _window));
        }

        public void RemoveTransition(NodeViewer nextNodeViewer)
        {
            TransitionViewer transition = _transitionViewers.Find(item => item.Destination.Node == nextNodeViewer);
            if (transition != null)
                RemoveTransition(transition);
        }

        public void RemoveTransition(TransitionViewer transition)
        {
            _transitionViewers.Remove(transition);
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
            _nodeDrawer.OnHeaderGui(_window.RootNodeId == _id, _nameProp, ref _renaming);
            if (_window.Camera.Size <= 1f)
                _nodeDrawer.OnGui(_nodeProp, nodeRect.width, _window.FullDrawing);
            GUILayout.EndArea();

            void drawPorts()
            {
                if (_window.TransitionView == TransitionViewType.Splines)
                {
                    if (_type != NodeType.Common) _in.Draw();
                    if (_type != NodeType.Exit) _out.Draw();
                }
            }
        }

        public void DrawTransitions()
        {
            switch (_window.TransitionView)
            {
                case TransitionViewType.Splines:
                    for (int i = 0; i < _transitionViewers.Count; i++)
                    {
                        _transitionViewers[i].DrawSpline();
                    }
                    break;

                case TransitionViewType.Arrows:
                    for (int i = 0; i < _transitionViewers.Count; i++)
                    {
                        _transitionViewers[i].DrawArrow();
                    }
                    break;

                default:
                    throw new UnsupportedValueException(_window.TransitionView);
            }
        }

        public bool ProcessEvents(Event e)
        {
            if (!IsInCamera)
                return false;

            bool needLock = false;

            switch (e.type)
            {
                case EventType.MouseDown:
                    Rect nodeRect = ScreenRect;

                    if (nodeRect.Contains(e.mousePosition))
                    {
                        if (e.button == 0)
                        {
                            _dragedPosition = _position;
                            SelectInternal(true);
                            _isDragging = true;
                            needLock = true;
                            GUI.changed = true;
                        }
                        else if (e.button == 1)
                        {
                            SelectInternal(true);
                            needLock = true;
                            ProcessContextMenu();
                            GUI.changed = true;
                        }
                    }
                    else
                    {
                        _renaming = false;
                        if (!(_isSelected && e.control))
                            SelectInternal(false);
                        GUI.FocusControl(null);
                        GUI.changed = true;
                    }
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && _isSelected && _isDragging)
                    {
                        Drag(e.delta);
                        GUI.changed = true;
                        needLock = true;
                    }
                    break;

                case EventType.MouseUp:
                    if (e.button == 0)
                        _isDragging = false;
                    break;

                case EventType.KeyDown:
                    if (e.keyCode == KeyCode.Delete)
                    {
                        if (_isSelected)
                        {
                            _map.DeleteNode(this);
                            GUI.changed = true;
                        }
                    }
                    else if (e.keyCode == KeyCode.Return)
                    {
                        _renaming = false;
                        GUI.changed = true;
                    }

                    break;
            }

            for (int i = 0; i < _transitionViewers.Count; i++)
            {
                if (_transitionViewers[i].ProcessEvents(e))
                    needLock = true;
            }

            return needLock;
        }

        public void Save()
        {
            SerializedProperty positionProperty = _nodeProp.FindPropertyRelative(RawNode.PositionFieldName);
            positionProperty.vector2Value = _position;
            _transitionViewers.ForEach(item => item.Save());
        }

        private void ProcessContextMenu()
        {
            Vector2 clickPosition = Event.current.mousePosition;
            GenericMenu genericMenu = new GenericMenu();

            switch (_type)
            {
                case NodeType.Real:
                    genericMenu.AddItem(new GUIContent("Add Transition"), false, () => ProcessDropdownList(clickPosition));
                    genericMenu.AddItem(new GUIContent("Rename"), false, () => _renaming = true);
                    genericMenu.AddItem(new GUIContent("Set default name"), false, () => renameAsset());

                    if (_window.RootNodeId == _id && _type.RealNode())
                        genericMenu.AddDisabledItem(new GUIContent("Set as root"));
                    else
                        genericMenu.AddItem(new GUIContent("Set as root"), false, () => _window.SetAsRoot(this));

                    //genericMenu.AddItem(new GUIContent("Duplicate"), false, () => _field.CopySelectedNode());
                    //genericMenu.AddSeparator(null);
                    genericMenu.AddItem(new GUIContent("Delete"), false, () => _map.DeleteNode(this));
                    genericMenu.AddSeparator(null);
                    genericMenu.AddItem(new GUIContent("Info"), false, () => NodeInfoPopup.Open(this, _window));
                    break;

                case NodeType.Hub:
                    genericMenu.AddItem(new GUIContent("Add Transition"), false, () => ProcessDropdownList(clickPosition));
                    genericMenu.AddItem(new GUIContent("Delete"), false, () => _map.DeleteNode(this));
                    genericMenu.AddSeparator(null);
                    genericMenu.AddItem(new GUIContent("Info"), false, () => NodeInfoPopup.Open(this, _window));
                    break;

                case NodeType.Common:
                    genericMenu.AddItem(new GUIContent("Add Transition"), false, () => ProcessDropdownList(clickPosition));
                    genericMenu.AddItem(new GUIContent("Delete"), false, () => _map.DeleteNode(this));
                    genericMenu.AddSeparator(null);
                    genericMenu.AddItem(new GUIContent("Info"), false, () => NodeInfoPopup.Open(this, _window));
                    break;

                case NodeType.Exit:
                    genericMenu.AddItem(new GUIContent("Delete"), false, () => _map.DeleteNode(this));
                    break;

                default:
                    throw new UnsupportedValueException(_type);
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

        private void ProcessDropdownList(Vector2 clickPosition)
        {
            DropDownWindow list = ScriptableObject.CreateInstance<DropDownWindow>();

            foreach (NodeViewer item in _map.NodeViewers)
            {
                if (item == this || item.Type == NodeType.Common)
                    continue;

                list.AddItem(item._nameProp.stringValue, false, () => createTransition(item.In));
            }

            list.ShowMenu(clickPosition);

            void createTransition(PortViewer port)
            {
                _nodeProp.serializedObject.Update();
                CreateTransition(port);
                _nodeProp.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private float GetBigNodeHeight()
        {
            if (_heightVersion != _window.OnGuiCounter)
            {
                _height = _map.NodeHeaderHeight + _nodeDrawer.GetHeight(_nodeProp, _window.FullDrawing);
                _heightVersion = _window.OnGuiCounter;
            }

            return _height;
        }

        private void Drag(Vector2 mouseDelta)
        {
            if (_window.GridSnapping)
            {
                _dragedPosition += mouseDelta * _window.Camera.Size;
                _position = new Vector2(_dragedPosition.x.Round(GraphGrid.SMALL_STEP), _dragedPosition.y.Round(GraphGrid.SMALL_STEP));
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

            _isSelected = on;
            _map.OnNodeSelectionChanched(this, on);
        }
    }
}
