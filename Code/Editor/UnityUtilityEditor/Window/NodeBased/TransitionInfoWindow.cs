#if UNITY_2019_3_OR_NEWER
using UnityEditor;
using UnityEngine;
using UnityUtility.NodeBased;
using UnityUtilityEditor.Window.NodeBased.Stuff.NodeDrawing;

namespace UnityUtilityEditor.Window.NodeBased
{
    internal class TransitionInfoWindow : EditorWindow
    {
        private readonly Vector2 _positionOffset = new Vector2(3f, 4f);
        private readonly Vector2 _sizeShrink = new Vector2(6f, 8f);

        private TransitionViewer _transition;
        private SerializedProperty _transitionProp;
        private SerializedProperty _conditionProp;
        private GraphEditorWindow _mainWindow;
        private Vector2 _scrollPos;

        private void OnEnable()
        {
            minSize = new Vector2(300f, 300f);
        }

        public static void Open(TransitionViewer transition, SerializedProperty transitionProp, GraphEditorWindow mainWindow)
        {
            TransitionInfoWindow window = CreateInstance<TransitionInfoWindow>();
            window.titleContent = new GUIContent("Transition Info");
            window.SetUp(transition, transitionProp, mainWindow);
            window.ShowAuxWindow();
        }

        private void SetUp(TransitionViewer transition, SerializedProperty transitionProp, GraphEditorWindow mainWindow)
        {
            _transition = transition;
            _transitionProp = transitionProp;
            _conditionProp = transitionProp.FindPropertyRelative(Transition.ConditionFieldName);
            _mainWindow = mainWindow;
        }

        private void OnDestroy()
        {
            if (_mainWindow != null)
                _mainWindow.Focus();
        }

        private void OnGUI()
        {
            _transitionProp.serializedObject.Update();

            string from = _transition.Source.Node.NameProp.stringValue;
            string to = _transition.Destination.Node.NameProp.stringValue;

            using (new GUILayout.AreaScope(new Rect(_positionOffset, position.size - _sizeShrink), (string)null, EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField($"{from} → {to}");

                GUILayout.Space(5f);

                using (new EditorGUILayout.HorizontalScope())
                {
                    int pointsCount = _transition.PointsCount;
                    EditorGUILayout.LabelField($"Points: {pointsCount}", GUILayout.Width(100f));

                    GUILayout.FlexibleSpace();

                    GUI.enabled = pointsCount > 0;
                    if (GUILayout.Button("-", GUILayout.Width(EditorGuiUtility.SmallButtonWidth)))
                        _transition.RemovePoint();
                    GUI.enabled = true;

                    if (GUILayout.Button("+", GUILayout.Width(EditorGuiUtility.SmallButtonWidth)))
                        _transition.AddPoint();
                }

                if (_transition.Destination.Node.Type != NodeType.Hub)
                {
                    GUILayout.Space(10f);
                    _scrollPos.y = EditorGUILayout.BeginScrollView(_scrollPos, EditorStyles.helpBox).y;
                    EditorGUILayout.PropertyField(_conditionProp, true);
                    EditorGUILayout.EndScrollView();
                    GUILayout.Space(10f);
                }

                GUILayout.FlexibleSpace();


                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Delete", GUILayout.Height(30f), GUILayout.Width(100f)))
                    {
                        _transition.Source.Node.RemoveTransition(_transition);
                        Close();
                    }
                }
            }

            _transitionProp.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
#endif
