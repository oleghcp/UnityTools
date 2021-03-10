using UnityEditor;
using UnityEngine;
using UnityUtility;
using static UnityUtilityEditor.Drawers.SortingLayerIDDrawer;

namespace UnityUtilityEditor.CustomEditors
{
    [CustomEditor(typeof(RenderSorter))]
    internal class RenderSorterEditor : Editor<RenderSorter>
    {
        private DrawTool _drawer;
        private Renderer _renderer;

        private void Awake()
        {
            _drawer = new DrawTool();

            SerializedProperty rendererProp = serializedObject.FindProperty(RenderSorter.RendererFieldName);
            _renderer = rendererProp.objectReferenceValue as Renderer;

            if (_renderer == null || _renderer.gameObject != target.gameObject)
            {
                rendererProp.objectReferenceValue = _renderer = target.GetComponent<Renderer>();
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            int sortingLayerID = _drawer.Draw("Sorting Layer", _renderer.sortingLayerID);
            int sortingOrder = EditorGUILayout.IntField("Sorting Order", _renderer.sortingOrder);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(_renderer, "Renderer sorting");

                _renderer.sortingLayerID = sortingLayerID;
                _renderer.sortingOrder = sortingOrder;

                EditorUtility.SetDirty(_renderer);
            }
        }
    }
}
