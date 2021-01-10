using UnityEditor;
using UnityEngine;
using UnityUtility;
using static UnityUtilityEditor.Drawers.SortingLayerIDDrawer;

namespace UnityUtilityEditor.CustomEditors
{
    [CustomEditor(typeof(RenderSorter)), CanEditMultipleObjects]
    internal class RenderSorterEditor : Editor
    {
        private RenderSorter m_target;
        private Renderer m_renderer;
        private DrawTool m_drawer;

        private void Awake()
        {
            m_target = target as RenderSorter;
            m_drawer = new DrawTool();

            SerializedProperty renderer = serializedObject.FindProperty("_renderer");
            m_renderer = renderer.objectReferenceValue as Renderer;

            if (m_renderer == null || m_renderer.gameObject != m_target.gameObject)
            {
                renderer.objectReferenceValue = m_renderer = m_target.GetComponent<Renderer>();
                serializedObject.ApplyModifiedProperties();
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            m_renderer.sortingLayerID = m_drawer.Draw("Sorting Layer", m_renderer.sortingLayerID);
            m_renderer.sortingOrder = EditorGUILayout.IntField("Sorting Order", m_renderer.sortingOrder);

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
