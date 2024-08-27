using OlegHcp;
using OlegHcpEditor.MenuItems;
using UnityEditor;
using UnityEngine;
using static OlegHcpEditor.Drawers.Attributes.SortingLayerIDDrawer;

namespace OlegHcpEditor.Inspectors
{
    [CustomEditor(typeof(RenderSorter))]
    internal class RenderSorterEditor : Editor<RenderSorter>
    {
        private DrawTool _drawer;
        private Renderer _renderer;

        private void OnEnable()
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
            Draw(_drawer, _renderer);
        }

        public static void Draw(DrawTool drawer, Renderer target)
        {
            EditorGUI.BeginChangeCheck();

            int sortingLayerID = drawer.Draw("Sorting Layer", target.sortingLayerID);
            int sortingOrder = EditorGUILayout.IntField("Sorting Order", target.sortingOrder);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(target, "Renderer sorting");

                target.sortingLayerID = sortingLayerID;
                target.sortingOrder = sortingOrder;

                EditorUtility.SetDirty(target);
            }
        }

        [MenuItem(MenuItemsUtility.CONTEXT_MENU_NAME + nameof(RenderSorter) + "/" + MenuItemsUtility.RESET_ITEM_NAME)]
        private static void ResetMenuItem(MenuCommand command)
        {
            Renderer renderer = (command.context as RenderSorter).Renderer;

            Undo.RegisterCompleteObjectUndo(renderer, "Renderer sorting");

            renderer.sortingLayerID = 0;
            renderer.sortingOrder = 0;

            EditorUtility.SetDirty(renderer);
        }
    }
}
