using System.Linq;
using UnityEngine;
using OlegHcp.CSharp;
using OlegHcp.Engine;

namespace OlegHcpEditor.Window.ShapeWizards
{
    internal class CreateRectPlaneWizard : CreateMeshWizard
    {
        public Vector2 Size = Vector2.one;
        public Vector2 Pivot = new Vector2(0.5f, 0.5f);
        public Finding Orientation = Finding.XY;
        public Rect TextureCoords = Rect.MinMaxRect(0f, 0f, 1f, 1f);

        protected override string GenerateObjectName()
        {
            return "Plane";
        }

        protected override string GenerateMeshName()
        {
            Vector2 offset = TextureCoords.position;
            Vector2 tiling = TextureCoords.size;

            return string.Concat(GenerateObjectName(), Orientation.GetName(),
                                 Size.x.ToString(), "x", Size.y.ToString(),
                                 "p", Pivot.x.ToString(), "x", Pivot.y.ToString(),
                                 "o", offset.x.ToString(), "x", offset.y.ToString(),
                                 "t", tiling.x.ToString(), "x", tiling.y.ToString());
        }

        protected override Mesh CreateMesh()
        {
            Vector3 shift = Vector2.Scale(Pivot, Size);

            Vector3[] newVertices = new[]
            {
                new Vector3(0f, 0f),
                new Vector3(Size.x, 0f),
                new Vector3(0f, Size.y),
                new Vector3(Size.x, Size.y)
            };

            if (Orientation == Finding.ZX)
            {
                newVertices = newVertices.Select(itm => ((Vector2)itm).To_XyZ()).ToArray();
                shift.Set(shift.x, 0f, shift.y);
            }

            for (int i = 0; i < newVertices.Length; i++)
            {
                newVertices[i] -= shift;
            }

            Vector2[] newUv = new[]
            {
                new Vector2(TextureCoords.xMin, TextureCoords.yMin),
                new Vector2(TextureCoords.xMax, TextureCoords.yMin),
                new Vector2(TextureCoords.xMin, TextureCoords.yMax),
                new Vector2(TextureCoords.xMax, TextureCoords.yMax)
            };

            int[] newTriangles = new[]
            {
                0, 2, 3,
                0, 3, 1
            };

            var mesh = new Mesh
            {
                vertices = newVertices,
                uv = newUv,
                triangles = newTriangles
            };

            mesh.RecalculateNormals();

            return mesh;
        }
    }
}
