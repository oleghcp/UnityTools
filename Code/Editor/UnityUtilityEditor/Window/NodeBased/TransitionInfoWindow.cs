using UnityEditor;
using UnityEngine;
using UnityUtility.NodeBased;
using UnityUtilityEditor.Window.NodeBased.Stuff;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtilityEditor.Window.NodeBased
{
    internal class TransitionInfoWindow : EditorWindow
    {
        private readonly Vector2 _positionOffset = new Vector2(3f, 4f);
        private readonly Vector2 _sizeShrink = new Vector2(6f, 8f);

        private TransitionViewer _transition;
        private SerializedProperty _transitionProp;
        private GraphEditorWindow _mainWindow;
        private Vector2 _scrollPos;

        public static void Open(TransitionViewer transition, SerializedProperty transitionProp, GraphEditorWindow mainWindow)
        {
            TransitionInfoWindow window = CreateInstance<TransitionInfoWindow>();

            window.titleContent = new GUIContent("Transition Info");
            window.minSize = new Vector2(300f, 300f);

            window.SetUp(transition, transitionProp, mainWindow);

            window.ShowAuxWindow();
        }

        private void SetUp(TransitionViewer transition, SerializedProperty transitionProp, GraphEditorWindow mainWindow)
        {
            _transition = transition;
            _transitionProp = transitionProp;
            _mainWindow = mainWindow;
        }

        private void OnDestroy()
        {
            if (_mainWindow != null)
                _mainWindow.Focus();
        }

        private void OnGUI()
        {
            string from = _transition.Out.Node.NodeAsset.name;
            string to = _transition.In.Node.NodeAsset.name;

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

                GUILayout.Space(10f);

                _scrollPos.y = EditorGUILayout.BeginScrollView(_scrollPos, EditorStyles.helpBox).y;

                EditorGUILayout.LabelField("Transition properties:", EditorStyles.boldLabel);
                EditorGUILayout.Space();

                EditorGUI.indentLevel++;
                foreach (SerializedProperty item in _transitionProp.EnumerateInnerProperties())
                {
                    if (item.name == Transition.NodeFieldName ||
                        item.name == Transition.PointsFieldName)
                        continue;

                    EditorGUILayout.PropertyField(item);
                }
                EditorGUI.indentLevel--;

                EditorGUILayout.EndScrollView();

                GUILayout.FlexibleSpace();

                GUILayout.Space(10f);

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Delete", GUILayout.Height(30f), GUILayout.Width(100f)))
                    {
                        _mainWindow.DeleteTransition(_transition);
                        Close();
                    }
                }
            }

            _transitionProp.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
#endif