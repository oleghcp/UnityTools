using UnityEngine;
using UnityEditor;
using UnityUtility;

namespace UnityUtilityEditor.CustomEditors
{
    [CustomEditor(typeof(RenderSorter)), CanEditMultipleObjects]
    internal class RenderSorterEditor : Editor
    {
        private SerializedProperty m_sortingLayer;
        private SerializedProperty m_sortingOrder;
        private Renderer m_renderer;

        private void Awake()
        {
            m_sortingLayer = serializedObject.FindProperty("_sortingLayer");
            m_sortingOrder = serializedObject.FindProperty("_sortingOrder");

            SerializedProperty renderer = serializedObject.FindProperty("_renderer");
            m_renderer = renderer.objectReferenceValue as Renderer;
            if (m_renderer == null || m_renderer.gameObject != (target as RenderSorter).gameObject)
                renderer.objectReferenceValue = m_renderer = (target as RenderSorter).GetComponent<Renderer>();

            m_sortingLayer.intValue = m_renderer.sortingLayerID;
            m_sortingOrder.intValue = m_renderer.sortingOrder;
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();
            m_renderer.sortingLayerID = m_sortingLayer.intValue;
            m_renderer.sortingOrder = m_sortingOrder.intValue;
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
