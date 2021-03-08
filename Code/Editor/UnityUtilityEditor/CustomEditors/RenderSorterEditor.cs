using UnityEditor;
using UnityEngine;
using UnityUtility;
using static UnityUtilityEditor.Drawers.SortingLayerIDDrawer;

namespace UnityUtilityEditor.CustomEditors
{
    [CustomEditor(typeof(RenderSorter))]
    internal class RenderSorterEditor : Editor<RenderSorter>
    {
        private Renderer _renderer;
        private DrawTool _drawer;
        private SerializedObject _serializedObject;

        private void Awake()
        {
            SerializedProperty rendererProp = serializedObject.FindProperty("_renderer");
            _renderer = rendererProp.objectReferenceValue as Renderer;
            _serializedObject = new SerializedObject(_renderer);
            _drawer = new DrawTool();

            if (_renderer == null || _renderer.gameObject != target.gameObject)
            {
                rendererProp.objectReferenceValue = _renderer = target.GetComponent<Renderer>();
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            _renderer.sortingLayerID = _drawer.Draw("Sorting Layer", _renderer.sortingLayerID);
            _renderer.sortingOrder = EditorGUILayout.IntField("Sorting Order", _renderer.sortingOrder);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(_renderer, "Renderer sorting");
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
