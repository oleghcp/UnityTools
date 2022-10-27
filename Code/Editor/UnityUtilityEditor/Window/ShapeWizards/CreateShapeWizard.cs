using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.MathExt;

namespace UnityUtilityEditor.Window.ShapeWizards
{
    internal class CreateShapeWizard : CreateMeshWizard
    {
        public int Edges = 3;
        public float Pivot = 0.5f;
        public float TopRadius = 0.5f;
        public float BottomRadius = 0.5f;
        public float Height = 1f;
        public bool Smooth;
        public bool Tile;

        private void Update()
        {
            Edges = Edges.ClampMin(3);
            Pivot = Pivot.Clamp01();
            TopRadius = TopRadius.ClampMin(0f);
            BottomRadius = BottomRadius.ClampMin(TopRadius);
            Height = Height.ClampMin(0f);
        }

        protected override string GenerateObjectName()
        {
            return TopRadius < BottomRadius ? "Pyramid" : "Prism";
        }

        protected override string GenerateMeshName()
        {
            return string.Concat(GenerateObjectName(),
                                 Edges.ToString(), "e",
                                 Pivot.ToString(), "p",
                                 TopRadius.ToString(), "tr",
                                 BottomRadius.ToString(), "br",
                                 Height.ToString(), "h",
                                 Smooth ? "_s" : string.Empty,
                                 Tile ? "_t" : string.Empty);
        }

        protected override Mesh CreateMesh()
        {
            var newVertices = GetVertices();
            var newTriangles = GetTriangles(newVertices.Length);
            var newUV = GetUvCoords(newVertices.Length);

            Mesh mesh = new Mesh
            {
                vertices = newVertices,
                triangles = newTriangles,
                uv = newUV
            };

            if (Smooth) { mesh.normals = GetNormals(newVertices); }
            else { mesh.RecalculateNormals(); }

            return mesh;
        }

        private Vector3[] GetVertices()
        {
            Vector3[] vertices;

            Vector3 heightVector = new Vector3(0f, Height, 0f);

            if (TopRadius.Approx(0f))
            {
                vertices = new Vector3[Edges * 4 + 1];

                for (int i = 0; i < Edges; i++)
                {
                    float angle = (360f / Edges * i).ToRadians();

                    vertices[i] = new Vector3(MathF.Sin(-angle), 0f, MathF.Cos(-angle)) * BottomRadius - heightVector * Pivot;

                    vertices[i + Edges] = heightVector * (1f - Pivot);

                    Vector3 vertPos = new Vector3(MathF.Sin(angle), 0f, MathF.Cos(angle)) * BottomRadius - heightVector * Pivot;
                    vertices[i + Edges * 2] = vertPos;
                    vertices[i + Edges * 3] = vertPos;
                }

                vertices.FromEnd(0) = -heightVector * Pivot;
            }
            else
            {
                vertices = new Vector3[Edges * 6 + 2];

                for (int i = 0; i < Edges; i++)
                {
                    float angle = (360f / Edges * i).ToRadians();

                    Vector3 vertexPos = new Vector3(MathF.Sin(angle), 0f, MathF.Cos(angle)) * TopRadius + heightVector * (1f - Pivot);

                    vertices[i] = vertexPos;
                    vertices[i + Edges * 2] = vertexPos;
                    vertices[i + Edges * 3] = vertexPos;

                    vertices[i + Edges] = new Vector3(MathF.Sin(-angle), 0f, MathF.Cos(-angle)) * BottomRadius - heightVector * Pivot;

                    vertexPos = new Vector3(MathF.Sin(angle), 0f, MathF.Cos(angle)) * BottomRadius - heightVector * Pivot;

                    vertices[i + Edges * 4] = vertexPos;
                    vertices[i + Edges * 5] = vertexPos;
                }

                vertices.FromEnd(1) = heightVector * (1f - Pivot);
                vertices.FromEnd(0) = -heightVector * Pivot;
            }

            return vertices;
        }

        private int[] GetTriangles(int vertices)
        {
            int[] triangles;

            int step = 0;
            void createTriangle(int iteration, int a, int b, int c)
            {
                int trIndex = (Edges * step + iteration) * 3;

                triangles[trIndex] = a;
                triangles[++trIndex] = b;
                triangles[++trIndex] = c;

                step++;
            }

            if (TopRadius.Approx(0f))
            {
                triangles = new int[Edges * 6];

                for (int i = 0; i < Edges; i++)
                {
                    createTriangle(i, vertices - 1, i, (i + 1) % Edges);
                    createTriangle(i, i + Edges, i + Edges * 2, (i + 1) % Edges + Edges * 3);
                    step = 0;
                }
            }
            else
            {
                triangles = new int[Edges * 12];

                for (int i = 0; i < Edges; i++)
                {
                    createTriangle(i, vertices - 2, i, (i + 1) % Edges);
                    createTriangle(i, vertices - 1, i + Edges, (i + 1) % Edges + Edges);
                    createTriangle(i, i + Edges * 2, i + Edges * 4, (i + 1) % Edges + Edges * 5);
                    createTriangle(i, (i + 1) % Edges + Edges * 5, (i + 1) % Edges + Edges * 3, i + Edges * 2);
                    step = 0;
                }
            }

            return triangles;
        }

        private Vector2[] GetUvCoords(int vertices)
        {
            Vector2[] uv = new Vector2[vertices];

            Vector2 shift = Vector2.one * 0.5f;

            if (TopRadius.Approx(0f))
            {
                uv.FromEnd(0) = shift;

                for (int i = 0; i < Edges; i++)
                {
                    float angle = (360f / Edges * i).ToRadians();
                    Vector2 pos = new Vector2(MathF.Sin(angle), MathF.Cos(angle)) * 0.5f + shift;
                    uv[i] = pos;
                    uv[i + Edges] = pos;

                    if (Tile)
                    {
                        uv[i + Edges] = new Vector2(0.5f, 1f);
                        uv[i + Edges * 2] = new Vector2(1f, 0f);
                        uv[i + Edges * 3] = new Vector2(0f, 0f);
                    }
                    else
                    {
                        float ratio = 1f / Edges;
                        float iRatio = 1f - ratio * i;

                        uv[i + Edges] = new Vector2(0.5f, 1f);
                        uv[i + Edges * 2] = new Vector2(iRatio, 0f);
                        uv[(i + 1) % Edges + Edges * 3] = new Vector2(iRatio - ratio, 0f);
                    }
                }
            }
            else
            {
                uv.FromEnd(1) = shift;
                uv.FromEnd(0) = shift;

                for (int i = 0; i < Edges; i++)
                {
                    float angle = (360f / Edges * i).ToRadians();
                    Vector2 pos = new Vector2(MathF.Sin(angle), MathF.Cos(angle)) * 0.5f + shift;
                    uv[i] = pos;
                    uv[i + Edges] = pos;

                    if (Tile)
                    {
                        uv[i + Edges * 2] = new Vector2(1f, 1f);
                        uv[i + Edges * 3] = new Vector2(0f, 1f);
                        uv[i + Edges * 4] = new Vector2(1f, 0f);
                        uv[i + Edges * 5] = new Vector2(0f, 0f);
                    }
                    else
                    {
                        float ratio = 1f / Edges;
                        float iRatio = 1f - ratio * i;

                        uv[i + Edges * 2] = new Vector2(iRatio, 1f);
                        uv[(i + 1) % Edges + Edges * 3] = new Vector2(iRatio - ratio, 1f);
                        uv[i + Edges * 4] = new Vector2(iRatio, 0f);
                        uv[(i + 1) % Edges + Edges * 5] = new Vector2(iRatio - ratio, 0f);
                    }
                }
            }

            return uv;
        }

        private Vector3[] GetNormals(Vector3[] vertices)
        {
            int len = vertices.Length;
            Vector3[] normals = new Vector3[len];

            if (TopRadius.Approx(0f))
            {
                normals[len - 1] = Vector3.down;

                for (int i = 0; i < Edges; i++)
                {
                    normals[i] = Vector3.down;

                    Vector3 side = (vertices[i + Edges * 3] - vertices[len - 1]).normalized;
                    Vector3 orthoSide = Vector3.Cross(side, Vector3.up);
                    Vector3 slopeDir = (vertices[i + Edges * 3] - vertices[i + Edges]).normalized;
                    Vector3 normal = Vector3.Cross(orthoSide, slopeDir);

                    normals[i + Edges] = normal;
                    normals[i + Edges * 2] = normal;
                    normals[i + Edges * 3] = normal;
                }
            }
            else
            {
                normals[len - 2] = Vector3.up;
                normals[len - 1] = Vector3.down;

                for (int i = 0; i < Edges; i++)
                {
                    normals[i] = Vector3.up;
                    normals[i + Edges] = Vector3.down;

                    Vector3 side = (vertices[i + Edges * 4] - vertices[len - 1]).normalized;
                    Vector3 orthoSide = Vector3.Cross(side, Vector3.up);
                    Vector3 slopeDir = (vertices[i + Edges * 4] - vertices[i + Edges * 2]).normalized;
                    Vector3 normal = Vector3.Cross(orthoSide, slopeDir);

                    normals[i + Edges * 2] = normal;
                    normals[i + Edges * 3] = normal;
                    normals[i + Edges * 4] = normal;
                    normals[i + Edges * 5] = normal;
                }
            }

            return normals;
        }
    }
}
