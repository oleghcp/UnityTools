using UnityEditor;
using UnityEngine;
using UnityUtility;
using static UnityUtilityEditor.Drawers.SortingLayerIDDrawer;

namespace UnityUtilityEditor.CustomEditors
{
    [CustomEditor(typeof(RenderSorter))]
    internal class RenderSorterEditor : Editor
    {
        private Renderer m_renderer;
        private DrawTool m_drawer;
        private SerializedObject m_serializedObject;

        private void Awake()
        {
            SerializedProperty rendererProp = serializedObject.FindProperty("_renderer");
            m_renderer = rendererProp.objectReferenceValue as Renderer;
            m_serializedObject = new SerializedObject(m_renderer);
            m_drawer = new DrawTool();

            RenderSorter sorter = target as RenderSorter;

            if (m_renderer == null || m_renderer.gameObject != sorter.gameObject)
            {
                rendererProp.objectReferenceValue = m_renderer = sorter.GetComponent<Renderer>();
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            m_renderer.sortingLayerID = m_drawer.Draw("Sorting Layer", m_renderer.sortingLayerID);
            m_renderer.sortingOrder = EditorGUILayout.IntField("Sorting Order", m_renderer.sortingOrder);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(m_renderer, "Renderer sorting");
                m_serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
