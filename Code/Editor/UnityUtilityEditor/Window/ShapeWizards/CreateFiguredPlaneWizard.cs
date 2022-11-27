using System;
using System.Linq;
using UnityEngine;
using UnityUtility.Engine;
using UnityUtility.Mathematics;

namespace UnityUtilityEditor.Window.ShapeWizards
{
    internal class CreateFiguredPlaneWizard : CreateMeshWizard
    {
        public int Vertices = 3;
        public float Radius = 0.5f;
        public Finding Orientation = Finding.XY;

        private void Update()
        {
            Vertices = Vertices.ClampMin(3);
        }

        protected override string GenerateObjectName()
        {
            return "Plane";
        }

        protected override string GenerateMeshName()
        {
            return string.Concat(GenerateObjectName(),
                                 Orientation.GetName(),
                                 Vertices.ToString(), "v",
                                 Radius.ToString(), "r");
        }

        protected override Mesh CreateMesh()
        {
            Mesh mesh = new Mesh
            {
                vertices = GetVertices(),
                triangles = GetTriangles(),
                uv = GetUvCoords()
            };

            mesh.RecalculateNormals();

            return mesh;
        }

        // -- //

        private Vector3[] GetVertices()
        {
            Vector3[] vertices = new Vector3[Vertices + 1];

            for (int i = 0; i < Vertices; i++)
            {
                float angle = (360f / Vertices * i).ToRadians();

                vertices[i] = new Vector3(MathF.Sin(angle), MathF.Cos(angle)) * Radius;
            }

            if (Orientation == Finding.ZX)
            {
                vertices = vertices.Select(itm => ((Vector2)itm).To_XyZ()).ToArray();
            }

            vertices[Vertices] = Vector3.zero;

            return vertices;
        }

        private int[] GetTriangles()
        {
            int[] triangles = new int[Vertices * 3];

            for (int i = 0; i < Vertices; i++)
            {
                int trIndex = i * 3;

                triangles[trIndex] = Vertices;
                triangles[++trIndex] = i;
                triangles[++trIndex] = (i + 1) % Vertices;
            }

            return triangles;
        }

        private Vector2[] GetUvCoords()
        {
            Vector2[] uv = new Vector2[Vertices + 1];

            Vector2 shift = Vector2.one * 0.5f;

            uv[Vertices] = shift;

            for (int i = 0; i < Vertices; i++)
            {
                float angle = (360f / Vertices * i).ToRadians();
                uv[i] = new Vector2(MathF.Sin(angle), MathF.Cos(angle)) * 0.5f + shift;
            }

            return uv;
        }
    }
}
