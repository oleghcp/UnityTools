using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.MathExt;
using UnityUtility.NodeBased;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtilityEditor.Window.NodeBased.Stuff
{
    internal class NodeViewer
    {
        private readonly Vector2 UI_OFFSET = new Vector2(10f, 10f);
        private readonly Vector2 UI_SHRINK = new Vector2(20f, 22f);

        private GraphEditorWindow _window;
        private Node _nodeAsset;
        private SerializedObject _serializedObject;
        private SerializedProperty _nameProperty;
        private SerializedProperty _transitionsProperty;
        private SerializedProperty _positionProperty;

        private bool _isDragged;
        private bool _isSelected;
        private bool _renaming;
        private float _height;
        private Vector2 _dragedPosition;

        private PortViewer _in;
        private PortViewer _out;

        public Node NodeAsset => _nodeAsset;
        public PortViewer In => _in;
        public PortViewer Out => _out;
        public bool IsSelected => _isSelected;
        public Vector2 Position
        {
            get => _positionProperty.vector2Value;
            set
            {
                _positionProperty.vector2Value = value;
                _serializedObject.ApplyModifiedProperties();
            }
        }

        public NodeViewer(Node nodeAsset, GraphEditorWindow window)
        {
            _nodeAsset = nodeAsset;
            _window = window;
            _serializedObject = new SerializedObject(nodeAsset);
            _nameProperty = _serializedObject.FindProperty(EditorUtilityExt.ASSET_NAME_FIELD);
            _positionProperty = _serializedObject.FindProperty(Node.PositionFieldName);
            _transitionsProperty = _serializedObject.FindProperty(Node.ArrayFieldName);
            _height = CalcNodeHeight();

            _in = new PortViewer(this, PortType.In, window);
            _out = new PortViewer(this, PortType.Out, window);
        }

        public IEnumerable<(SerializedProperty transitionProp, Node connectedNode)> ParseTransitionsList()
        {
            return _transitionsProperty.EnumerateArrayElements().Select(getPair);

            (SerializedProperty transitionProp, Node node) getPair(SerializedProperty transitionProp)
            {
                using (SerializedProperty nodeProp = transitionProp.FindPropertyRelative(Transition.NodeFieldName))
                {
                    Node node = nodeProp.objectReferenceValue as Node;
                    return (transitionProp, node);
                }
            }
        }

        public SerializedProperty GetTransitionProperty(TransitionViewer transition)
        {
            Node nextNode = transition.In.Node._nodeAsset;

            foreach (var transitionProp in _transitionsProperty.EnumerateArrayElements())
            {
                using (var nodeProp = transitionProp.FindPropertyRelative(Transition.NodeFieldName))
                {
                    if (nodeProp.objectReferenceValue == nextNode)
                        return transitionProp;
                }

                transitionProp.Dispose();
            }

            throw new InvalidOperationException("Transition not found.");
        }

        public Rect GetRectInScreen()
        {
            return new Rect(_window.Camera.WorldToScreen(Position),
                            new Vector2(_window.NodeWidth / _window.Camera.Size,
                            _window.Camera.Size > 1 ? SmallHeight() : _height));
        }

        public Rect GetRectInWorld()
        {
            return new Rect(Position,
                            new Vector2(_window.NodeWidth,
                                        _window.Camera.Size > 1 ? SmallHeight() : _height));
        }

        public void Select(bool on)
        {
            if (_isSelected == on)
                return;

            SetSelection(on);
            GUI.changed = true;
        }

        public SerializedProperty AddTransition(Node next, Transition transition)
        {
            transition.Node = next;
            SerializedProperty property = _transitionsProperty.PlaceArrayElement();
            property.managedReferenceValue = transition;
            _serializedObject.ApplyModifiedProperties();
            return property;
        }

        public void RemoveReference(Node next)
        {
            int length = _transitionsProperty.arraySize;

            for (int i = 0; i < length; i++)
            {
                using (SerializedProperty transitionProp = _transitionsProperty.GetArrayElementAtIndex(i))
                {
                    using (SerializedProperty nodeProp = transitionProp.FindPropertyRelative(Transition.NodeFieldName))
                    {
                        if (nodeProp.objectReferenceValue == next)
                        {
                            _transitionsProperty.DeleteArrayElementAtIndex(i);
                            _serializedObject.ApplyModifiedProperties();
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

            _in.Draw();
            _out.Draw();

            Rect nodeRect = GetRectInScreen();

            GUI.Box(nodeRect, (string)null, _isSelected ? GraphEditorStyles.Styles.SelectedNode : GraphEditorStyles.Styles.Node);

            nodeRect.position += UI_OFFSET;
            nodeRect.size -= UI_SHRINK;

            using (new GUILayout.AreaScope(nodeRect))
            {
                DrawHeader();

                if (_window.Camera.Size <= 1f)
                    DrawContent(nodeRect.width);

                if (GUI.changed)
                {
                    _serializedObject.ApplyModifiedProperties();
                    _height = CalcNodeHeight();
                }
            }
        }

        public bool ProcessEvents(Event e)
        {
            bool needLock = false;

            switch (e.type)
            {
                case EventType.MouseDown:
                    Rect nodeRect = GetRectInScreen();

                    if (e.button == 0)
                    {
                        if (nodeRect.Contains(e.mousePosition))
                        {
                            _dragedPosition = Position;
                            _isDragged = true;
                            SetSelection(true);
                            needLock = true;
                        }
                        else
                        {
                            _renaming = false;
                            if (!(_isSelected && e.control))
                                SetSelection(false);
                        }
                        GUI.changed = true;
                    }
                    else if (e.button == 1)
                    {
                        if (nodeRect.Contains(e.mousePosition))
                        {
                            SetSelection(true);
                            GUI.changed = true;
                            needLock = true;
                            ProcessContextMenu();
                        }
                    }
                    break;

                case EventType.MouseUp:
                    _isDragged = false;
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && (_isDragged || _isSelected))
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

        private void DrawHeader()
        {
            if (_renaming)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    _nameProperty.stringValue = EditorGUILayout.TextField(_nameProperty.stringValue);
                    if (GUILayout.Button("V", GUILayout.Width(EditorGUIUtilityExt.SmallButtonWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                        _renaming = false;
                }
            }
            else
            {
                GUI.color = _window.IsRootNode(_nodeAsset) ? Colours.Orange : Colours.Cyan;
                EditorGUILayout.LabelField(_nodeAsset.name, GraphEditorStyles.Styles.NodeHeader);
                GUI.color = Colours.White;
            }
        }

        private void DrawContent(float width)
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = width * 0.5f;

            foreach (SerializedProperty item in _serializedObject.EnumerateProperties())
            {
                if (!IsServiceField(item))
                    EditorGUILayout.PropertyField(item, true);
            }

            EditorGUIUtility.labelWidth = labelWidth;
        }

        private void SetSelection(bool on)
        {
            _isSelected = on;
        }

        private void ProcessContextMenu()
        {
            GenericMenu genericMenu = new GenericMenu();

            genericMenu.AddItem(new GUIContent("Rename"), false, () => _renaming = true);

            if (_window.IsRootNode(_nodeAsset))
                genericMenu.AddDisabledItem(new GUIContent("Set as root"));
            else
                genericMenu.AddItem(new GUIContent("Set as root"), false, () => _window.SetAsRoot(this));

            genericMenu.AddItem(new GUIContent("Duplicate"), false, () => _window.CopySelectedNode());
            genericMenu.AddSeparator(null);
            genericMenu.AddItem(new GUIContent("Delete"), false, () => _window.DeleteNode(this));
            genericMenu.AddSeparator(null);
            genericMenu.AddItem(new GUIContent("Info"), false, () => NodeInfoWindow.Open(this, _window));

            genericMenu.ShowAsContext();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float CalcNodeHeight()
        {
            return SmallHeight() + EditorGUIUtilityExt.GetDrawHeight(_serializedObject, IsServiceField);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float SmallHeight()
        {
            return EditorGUIUtility.singleLineHeight + UI_SHRINK.y;
        }

        private bool IsServiceField(SerializedProperty property)
        {
            string fieldName = property.propertyPath;

            return fieldName == EditorUtilityExt.SCRIPT_FIELD ||
                   fieldName == Node.ArrayFieldName ||
                   fieldName == Node.GraphFieldName ||
                   fieldName == Node.IdFieldName ||
                   fieldName == Node.PositionFieldName;
        }

        private void Drag(Vector2 mouseDelta)
        {
            if (GraphEditorWindow.GridSnapping)
            {
                _dragedPosition += mouseDelta * _window.Camera.Size;
                Position = new Vector2(_dragedPosition.x.Round(GraphGrid.SMALL_STEP), _dragedPosition.y.Round(GraphGrid.SMALL_STEP));
            }
            else
            {
                Position += mouseDelta * _window.Camera.Size;
            }
        }
    }
}
#endif
