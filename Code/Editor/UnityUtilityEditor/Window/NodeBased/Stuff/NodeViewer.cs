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

        private GraphEditorWindow _window;
        private RawNode _nodeAsset;
        private SerializedObject _serializedObject;
        private SerializedProperty _nameProperty;

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

        private PortViewer _in;
        private PortViewer _out;

        public RawNode NodeAsset => _nodeAsset;
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
                    float width = _nodeAsset.ServiceNode() ? GraphAssetEditor.SERV_NODE_WIDTH : _window.GraphAssetEditor.NodeWidth;
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
                    _worldRect.width = _nodeAsset.ServiceNode() ? GraphAssetEditor.SERV_NODE_WIDTH : _window.GraphAssetEditor.NodeWidth;
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

        public NodeViewer(RawNode nodeAsset, GraphEditorWindow window)
        {
            UI_OFFSET = new Vector2(10f, 10f);
            UI_SHRINK = new Vector2(20f, 22f);
            SMALL_NODE_HEIGHT = EditorGUIUtility.singleLineHeight + UI_SHRINK.y;

            _nodeAsset = nodeAsset;
            _window = window;
            _serializedObject = new SerializedObject(nodeAsset);
            _nameProperty = _serializedObject.FindProperty(EditorUtilityExt.ASSET_NAME_FIELD);
            _position = _serializedObject.FindProperty(RawNode.PositionFieldName).vector2Value;

            _in = new PortViewer(this, PortType.In, window);
            _out = new PortViewer(this, PortType.Out, window);
        }

        public IEnumerable<(SerializedProperty transitionProp, RawNode connectedNode)> ParseTransitionsList()
        {
            SerializedProperty transitionsProperty = _serializedObject.FindProperty(RawNode.ArrayFieldName);
            return transitionsProperty.EnumerateArrayElements().Select(getPair);

            (SerializedProperty transitionProp, RawNode node) getPair(SerializedProperty transitionProp)
            {
                using (SerializedProperty nodeProp = transitionProp.FindPropertyRelative(Transition.NodeFieldName))
                {
                    RawNode node = nodeProp.objectReferenceValue as RawNode;
                    return (transitionProp, node);
                }
            }
        }

        public void Select(bool on)
        {
            if (_isSelected == on)
                return;

            _isSelected = on;
            GUI.changed = true;
        }

        public SerializedProperty AddTransition(RawNode nextNode)
        {
            SerializedProperty transitionsProperty = _serializedObject.FindProperty(RawNode.ArrayFieldName);
            SerializedProperty newItem = transitionsProperty.PlaceArrayElement();

            if (transitionsProperty.arraySize > 1)
                newItem.ResetToDefault();

            newItem.FindPropertyRelative(Transition.NodeFieldName).objectReferenceValue = nextNode;

            _serializedObject.ApplyModifiedPropertiesWithoutUndo();

            return newItem;
        }

        public void RemoveReference(RawNode next)
        {
            SerializedProperty transitionsProperty = _serializedObject.FindProperty(RawNode.ArrayFieldName);
            int length = transitionsProperty.arraySize;

            for (int i = 0; i < length; i++)
            {
                using (SerializedProperty transitionProp = transitionsProperty.GetArrayElementAtIndex(i))
                {
                    using (SerializedProperty nodeProp = transitionProp.FindPropertyRelative(Transition.NodeFieldName))
                    {
                        if (nodeProp.objectReferenceValue == next)
                        {
                            transitionsProperty.DeleteArrayElementAtIndex(i);
                            _serializedObject.ApplyModifiedPropertiesWithoutUndo();
                            break;
                        }
                    }
                }
            }
        }

        public void Draw()
        {
            if (_nodeAsset == null)
                return;

            if (!IsInCamera)
                return;

            bool serviceNode = _nodeAsset.ServiceNode();

            _serializedObject.Update();

            _in.Draw();

            if (!(_nodeAsset is ExitNode))
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

            _serializedObject.ApplyModifiedProperties();

            void DrawHeader()
            {
                if (_renaming)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        _nameProperty.stringValue = EditorGUILayout.TextField(_nameProperty.stringValue);
                        if (GUILayout.Button("V", GUILayout.Width(EditorGuiUtility.SmallButtonWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                            _renaming = false;
                    }
                }
                else
                {
                    if (serviceNode)
                    {
                        if (_nodeAsset is ExitNode)
                            GUI.color = Colours.Yellow;
                        else if (_nodeAsset is HubNode)
                            GUI.color = Colours.Silver;
                        else
                            throw new UnsupportedValueException(_nodeAsset.GetType().Name);
                    }
                    else
                    {
                        GUI.color = _window.RootNode == _nodeAsset ? Colours.Orange : Colours.Cyan;
                    }

                    EditorGUILayout.LabelField(_nodeAsset.name, GraphEditorStyles.Styles.NodeHeader);
                    GUI.color = Colours.White;
                }
            }

            void DrawContent(float width)
            {
                if (serviceNode)
                {
                    if (_nodeAsset is ExitNode)
                        EditorGUILayout.LabelField("→ → →");
                    else if (_nodeAsset is HubNode)
                        EditorGUILayout.LabelField("► ► ►");
                    else
                        throw new UnsupportedValueException(_nodeAsset.GetType().Name);
                }
                else
                {
                    float labelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = width * 0.5f;

                    foreach (SerializedProperty item in _serializedObject.EnumerateProperties())
                    {
                        if (IsServiceField(item))
                            continue;

                        EditorGUILayout.PropertyField(item, true);
                    }

                    EditorGUIUtility.labelWidth = labelWidth;
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

            return needLock;
        }

        public void Save()
        {
            SerializedProperty positionProperty = _serializedObject.FindProperty(RawNode.PositionFieldName);
            positionProperty.vector2Value = _position;
            _serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private void ProcessContextMenu()
        {
            GenericMenu genericMenu = new GenericMenu();

            if (_nodeAsset.ServiceNode())
            {
                genericMenu.AddItem(new GUIContent("Delete"), false, () => _window.DeleteNode(this));

                if (_nodeAsset is HubNode)
                {
                    genericMenu.AddSeparator(null);
                    genericMenu.AddItem(new GUIContent("Info"), false, () => NodeInfoWindow.Open(this, _window));
                }
            }
            else
            {
                genericMenu.AddItem(new GUIContent("Rename"), false, () => _renaming = true);

                string defaultName = GraphAssetEditor.GetDefaultNodeName(_nodeAsset.Id);
                if (_nodeAsset.name != defaultName)
                    genericMenu.AddItem(new GUIContent("Set default name"), false, () => renameAsset(defaultName));

                if (_window.RootNode == _nodeAsset && _nodeAsset.RealNode())
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

            void renameAsset(string name)
            {
                _serializedObject.Update();
                _nameProperty.stringValue = name;
                _serializedObject.ApplyModifiedProperties();
            }
        }

        private float GetBigNodeHeight()
        {
            if (_heightVersion != _window.OnGuiCounter)
            {
                _height = SMALL_NODE_HEIGHT + EditorGuiUtility.GetDrawHeight(_serializedObject, IsServiceField).CutBefore(EditorGUIUtility.singleLineHeight);
                _heightVersion = _window.OnGuiCounter;
            }

            return _height;
        }

        private bool IsServiceField(SerializedProperty property)
        {
            string fieldName = property.propertyPath;

            return fieldName == EditorUtilityExt.SCRIPT_FIELD ||
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
    }
}
