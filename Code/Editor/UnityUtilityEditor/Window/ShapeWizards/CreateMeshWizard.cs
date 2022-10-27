using System.IO;
using UnityEditor;
using UnityEngine;
using UnityUtility;

namespace UnityUtilityEditor.Window.ShapeWizards
{
    internal abstract class CreateMeshWizard : ScriptableWizard
    {
        public enum Finding : byte { XY, ZX }

        public bool AddCollider;
        public bool CreateAsset = true;

        private static Material _defaultMaterial;

        protected void OnWizardCreate()
        {
            Mesh mesh;

            if (CreateAsset)
            {
                string dir = $"{AssetDatabaseExt.ASSET_FOLDER}Meshes/";
                if (!Directory.Exists(dir)) { Directory.CreateDirectory(dir); }
                string meshPrefabPath = $"{dir}{GenerateMeshName()}{AssetDatabaseExt.ASSET_EXTENSION}";

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
            mr.material = GetDefaultMaterial();

            Selection.activeObject = mf.gameObject;
        }

        private Material GetDefaultMaterial()
        {
            if (_defaultMaterial == null)
            {
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                _defaultMaterial = go.GetComponent<MeshRenderer>().sharedMaterial;
                DestroyImmediate(go);
            }

            return _defaultMaterial;
        }

        protected abstract string GenerateObjectName();
        protected abstract string GenerateMeshName();
        protected abstract Mesh CreateMesh();
    }
}
