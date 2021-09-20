using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.MathExt;
using UnityUtility.NodeBased;

namespace UnityUtilityEditor.Window.NodeBased.Stuff
{
    internal class NodeViewer
    {
        private readonly Vector2 UI_OFFSET;
        private readonly Vector2 UI_SHRINK;
        private readonly float SMALL_NODE_HEIGHT;

        private readonly int _id;
        private readonly Type _systemType;
        private readonly NodeType _type;
        private readonly PortViewer _in;
        private readonly PortViewer _out;
        private readonly GraphEditorWindow _window;
        private readonly List<TransitionViewer> _transitionViewers;

        private SerializedProperty _nodeProp;

        private bool _isBeingDragged;
        private bool _isSelected;
        private bool _renaming;
        private float _height;

        private Vector2 _position;
        private Vector2 _dragedPosition;

        private Rect _screenRect;
        private Rect _worldRect;
        private bool _isInCamera;

        private int _heightVersion;
        private int _screenRectVersion;
        private int _worldRectVersion;
        private int _overlapVersion;

        public PortViewer In => _in;
        public PortViewer Out => _out;
        public bool IsSelected => _isSelected;

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public Rect ScreenRect
        {
            get
            {
                if (_screenRectVersion != _window.OnGuiCounter)
                {
                    _screenRect.position = _window.Camera.WorldToScreen(_position);
                    float width = _type.ServiceNode() ? GraphAssetEditor.SERV_NODE_WIDTH : _window.GraphAssetEditor.NodeWidth;
                    _screenRect.width = width / _window.Camera.Size;
                    _screenRect.height = _window.Camera.Size > 1f ? SMALL_NODE_HEIGHT : GetBigNodeHeight();
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
                    _worldRect.width = _type.ServiceNode() ? GraphAssetEditor.SERV_NODE_WIDTH : _window.GraphAssetEditor.NodeWidth;
                    _worldRect.height = _window.Camera.Size > 1f ? SMALL_NODE_HEIGHT : GetBigNodeHeight();
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

        public int Id => _id;
        public NodeType Type => _type;
        public Type SystemType => _systemType;

        public NodeViewer(SerializedProperty nodeProp, GraphEditorWindow window)
        {
            UI_OFFSET = new Vector2(10f, 10f);
            UI_SHRINK = new Vector2(20f, 22f);
            SMALL_NODE_HEIGHT = EditorGUIUtility.singleLineHeight + UI_SHRINK.y;

            _nodeProp = nodeProp;
            _window = window;

            _id = nodeProp.FindPropertyRelative(RawNode.IdFieldName).intValue;
            _systemType = EditorUtilityExt.GetTypeFromSerializedPropertyTypename(nodeProp.managedReferenceFullTypename);
            _type = GetNodeType(_systemType);
            _position = nodeProp.FindPropertyRelative(RawNode.PositionFieldName).vector2Value;

            _transitionViewers = new List<TransitionViewer>();

            _in = new PortViewer(this, PortType.In, window);
            _out = new PortViewer(this, PortType.Out, window);
        }

        public void ReinitSerializedProperty(SerializedProperty nodeProp)
        {
            _nodeProp = nodeProp;
        }

        public SerializedProperty FindSubProperty(string name)
        {
            return _nodeProp.FindPropertyRelative(name);
        }

        public void CreateConnections()
        {
            foreach (SerializedProperty transitionProp in _nodeProp.FindPropertyRelative(RawNode.ArrayFieldName).EnumerateArrayElements())
            {
                int nextNodeId = transitionProp.FindPropertyRelative(Transition.NodeIdFieldName).intValue;
                NodeViewer connectedNodeViewer = _window.NodeViewers.First(itm => itm.Id == nextNodeId);
                _transitionViewers.Add(new TransitionViewer(_out, connectedNodeViewer.In, _window));
            }
        }

        public void Select(bool on)
        {
            if (_isSelected == on)
                return;

            _isSelected = on;
            GUI.changed = true;
        }

        public void CreateTransition(NodeViewer nextNodeViewer)
        {
            if (_transitionViewers.Any(item => item.Destination.Node == nextNodeViewer))
                return;

            SerializedProperty transitionsProperty = _nodeProp.FindPropertyRelative(RawNode.ArrayFieldName);
            SerializedProperty newItem = transitionsProperty.PlaceArrayElement();

            newItem.ResetToDefault();
            newItem.FindPropertyRelative(Transition.NodeIdFieldName).intValue = nextNodeViewer.Id;
            _transitionViewers.Add(new TransitionViewer(_out, nextNodeViewer._in, _window));
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
            for (int i = 0; i < _transitionViewers.Count; i++)
            {
                _transitionViewers[i].Draw();
            }

            if (IsInCamera)
            {
                _in.Draw();

                if (_type != NodeType.Exit)
                    _out.Draw();

                Rect nodeRect = ScreenRect;

                GUI.Box(nodeRect, (string)null, _isSelected ? GraphEditorStyles.Styles.SelectedNode : GraphEditorStyles.Styles.Node);

                nodeRect.position += UI_OFFSET;
                nodeRect.size -= UI_SHRINK;

                using (new GUILayout.AreaScope(nodeRect))
                {
                    DrawHeader();

                    if (_window.Camera.Size <= 1f)
                        DrawContent(nodeRect.width);
                }
            }

            void DrawHeader()
            {
                SerializedProperty nameProperty = _nodeProp.FindPropertyRelative(RawNode.NameFieldName);

                if (_renaming)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        nameProperty.stringValue = EditorGUILayout.TextField(nameProperty.stringValue);
                        if (GUILayout.Button("V", GUILayout.Width(EditorGuiUtility.SmallButtonWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                            _renaming = false;
                    }
                }
                else
                {
                    switch (_type)
                    {
                        case NodeType.Real:
                            GUI.color = _window.RootNodeId == _id ? Colours.Orange : Colours.Cyan;
                            break;

                        case NodeType.Hub:
                            GUI.color = Colours.Silver;
                            break;

                        case NodeType.Exit:
                            GUI.color = Colours.Yellow;
                            break;

                        default:
                            throw new UnsupportedValueException(_type);
                    }

                    EditorGUILayout.LabelField(nameProperty.stringValue, GraphEditorStyles.Styles.NodeHeader);
                    GUI.color = Colours.White;
                }
            }

            void DrawContent(float width)
            {
                switch (_type)
                {
                    case NodeType.Real:
                        float labelWidth = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = width * 0.5f;

                        foreach (SerializedProperty item in _nodeProp.EnumerateInnerProperties())
                        {
                            if (IsServiceField(item))
                                continue;

                            EditorGUILayout.PropertyField(item, true);
                        }

                        EditorGUIUtility.labelWidth = labelWidth;
                        break;

                    case NodeType.Hub:
                        EditorGUILayout.LabelField("► ► ►");
                        break;

                    case NodeType.Exit:
                        EditorGUILayout.LabelField("→ → →");
                        break;

                    default:
                        throw new UnsupportedValueException(_type);
                }
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

                    if (e.button == 0)
                    {
                        if (nodeRect.Contains(e.mousePosition))
                        {
                            _dragedPosition = _position;
                            _isBeingDragged = true;
                            _isSelected = true;
                            needLock = true;
                        }
                        else
                        {
                            _renaming = false;
                            if (!(_isSelected && e.control))
                                _isSelected = false;
                            GUI.FocusControl(null);
                        }
                        GUI.changed = true;
                    }
                    else if (e.button == 1)
                    {
                        if (nodeRect.Contains(e.mousePosition))
                        {
                            _isSelected = true;
                            GUI.changed = true;
                            needLock = true;
                            ProcessContextMenu();
                        }
                    }
                    break;

                case EventType.MouseUp:
                    _isBeingDragged = false;
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && (_isBeingDragged || _isSelected))
                    {
                        Drag(e.delta);
                        GUI.changed = true;
                        needLock = true;
                    }
                    break;

                case EventType.KeyDown:
                    if (e.keyCode == KeyCode.Delete)
                    {
                        if (_isSelected)
                        {
                            _window.DeleteNode(this);
                            GUI.changed = true;
                        }
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
            GenericMenu genericMenu = new GenericMenu();

            if (_type.ServiceNode())
            {
                genericMenu.AddItem(new GUIContent("Delete"), false, () => _window.DeleteNode(this));

                if (_type == NodeType.Hub)
                {
                    genericMenu.AddSeparator(null);
                    genericMenu.AddItem(new GUIContent("Info"), false, () => NodeInfoWindow.Open(this, _window));
                }
            }
            else
            {
                genericMenu.AddItem(new GUIContent("Rename"), false, () => _renaming = true);
                genericMenu.AddItem(new GUIContent("Set default name"), false, () => renameAsset());

                if (_window.RootNodeId == Id && _type.RealNode())
                    genericMenu.AddDisabledItem(new GUIContent("Set as root"));
                else
                    genericMenu.AddItem(new GUIContent("Set as root"), false, () => _window.SetAsRoot(this));

                genericMenu.AddItem(new GUIContent("Duplicate"), false, () => _window.CopySelectedNode());
                genericMenu.AddSeparator(null);
                genericMenu.AddItem(new GUIContent("Delete"), false, () => _window.DeleteNode(this));
                genericMenu.AddSeparator(null);
                genericMenu.AddItem(new GUIContent("Info"), false, () => NodeInfoWindow.Open(this, _window));
            }

            genericMenu.ShowAsContext();

            void renameAsset()
            {
                string defaultName = GraphAssetEditor.GetDefaultNodeName(_systemType);
                SerializedProperty nameProperty = _nodeProp.FindPropertyRelative(RawNode.NameFieldName);
                nameProperty.stringValue = defaultName;
            }
        }

        private float GetBigNodeHeight()
        {
            if (_heightVersion != _window.OnGuiCounter)
            {
                _height = SMALL_NODE_HEIGHT + EditorGuiUtility.GetDrawHeight(_nodeProp, IsServiceField).CutBefore(EditorGUIUtility.singleLineHeight);
                _heightVersion = _window.OnGuiCounter;
            }

            return _height;
        }

        private bool IsServiceField(SerializedProperty property)
        {
            string fieldName = property.name;

            return fieldName == RawNode.NameFieldName ||
                   fieldName == RawNode.ArrayFieldName ||
                   fieldName == RawNode.GraphFieldName ||
                   fieldName == RawNode.IdFieldName ||
                   fieldName == RawNode.PositionFieldName;
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

        private static NodeType GetNodeType(Type type)
        {
            if (type.Is(typeof(ExitNode)))
                return NodeType.Exit;
            else if (type.Is(typeof(HubNode)))
                return NodeType.Hub;
            else
                return NodeType.Real;
        }
    }
}
