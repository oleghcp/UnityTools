using UnityEngine;
using UnityEditor;
using System.IO;
using UnityUtility;

#pragma warning disable CS0649
namespace UnityUtilityEditor.Window
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
                string dir = "Assets/Meshes/";
                if (!Directory.Exists(dir)) { Directory.CreateDirectory(dir); }
                string meshPrefabPath = string.Concat(dir, GenerateMeshName(), ".asset");

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

            MeshFilter mf = Script.CreateInstance<MeshFilter>(GenerateObjectName());
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
