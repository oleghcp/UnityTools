using System.IO;
using UnityEditor;
using UnityEngine;
using UnityUtility;

#pragma warning disable CS0649
namespace UnityUtilityEditor.Window.ShapeWizards
{
    internal abstract class CreateMeshWizard : ScriptableWizard
    {
        public enum Finding : byte { XY, ZX }

        public bool AddCollider;
        public bool CreateAsset = true;

        private static Material m_defaultMaterial;

        protected void OnWizardCreate()
        {
            Mesh mesh;

            if (CreateAsset)
            {
                string dir = $"{EditorUtilityExt.ASSET_FOLDER}Meshes/";
                if (!Directory.Exists(dir)) { Directory.CreateDirectory(dir); }
                string meshPrefabPath = $"{dir}{GenerateMeshName()}{EditorUtilityExt.ASSET_EXTENSION}";

                mesh = (Mesh)AssetDatabase.LoadAssetAtPath(meshPrefabPath, typeof(Mesh));

                if (mesh == null)
                {
                    mesh = CreateMesh();

                    AssetDatabase.CreateAsset(mesh, meshPrefabPath);
                    AssetDatabase.SaveAssets();
                }
            }
            else
            {
                mesh = CreateMesh();
            }

            MeshFilter mf = ComponentUtility.CreateInstance<MeshFilter>(GenerateObjectName());
            mf.mesh = mesh;

            if (AddCollider)
            {
                MeshCollider mc = mf.gameObject.AddComponent<MeshCollider>();
                mc.sharedMesh = mf.sharedMesh;
            }

            MeshRenderer mr = mf.gameObject.AddComponent<MeshRenderer>();
            mr.material = f_getDefaultMaterial();

            Selection.activeObject = mf.gameObject;
        }

        private Material f_getDefaultMaterial()
        {
            if (m_defaultMaterial == null)
            {
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                m_defaultMaterial = go.GetComponent<MeshRenderer>().sharedMaterial;
                DestroyImmediate(go);
            }

            return m_defaultMaterial;
        }

        protected abstract string GenerateObjectName();
        protected abstract string GenerateMeshName();
        protected abstract Mesh CreateMesh();
    }
}
