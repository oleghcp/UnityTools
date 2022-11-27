using System.Runtime.CompilerServices;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UnityUtility.Engine
{
    public static class UnityObjectExtensions
    {
        private static ConditionalWeakTable<MeshRenderer, MeshFilter> _meshRendererData;

        /// <summary>
        /// Destroys the unity object.
        /// </summary>
        /// <param name="time">Time  for destruction.</param>
        public static void Destroy(this UnityObject self, float time)
        {
            UnityObject.Destroy(self, time);
        }

        /// <summary>
        /// Destroys the unity object.
        /// </summary>
        public static void Destroy(this UnityObject self)
        {
            UnityObject.Destroy(self, 0f);
        }

        /// <summary>
        /// Marks unity object as DontDestroyOnLoad.
        /// </summary>
        public static void Immortalize(this UnityObject self)
        {
            UnityObject.DontDestroyOnLoad(self);
        }

        /// <summary>
        /// Returns true if a game object is asset reference. For scene objects returns false.
        /// </summary>
        public static bool IsAsset(this GameObject self)
        {
            return UnityObjectUtility.IsAsset(self);
        }

        /// <summary>
        /// Returns true if a Component is asset reference. For scene objects returns false.
        /// </summary>
        public static bool IsAsset(this Component self)
        {
            return UnityObjectUtility.IsAsset(self);
        }

        /// <summary>
        /// Returns true if UnityEngine.Object is asset reference.
        /// </summary>
        public static bool IsAsset(this UnityObject self)
        {
            return UnityObjectUtility.IsAsset(self);
        }

        public static Mesh GetMesh(this MeshRenderer self)
        {
            return GetMeshFilter(self)?.mesh;
        }

        public static Mesh GetSharedMesh(this MeshRenderer self)
        {
            return GetMeshFilter(self)?.sharedMesh;
        }

        private static MeshFilter GetMeshFilter(MeshRenderer renderer)
        {
            if (_meshRendererData == null)
                _meshRendererData = new ConditionalWeakTable<MeshRenderer, MeshFilter>();

            if (!_meshRendererData.TryGetValue(renderer, out MeshFilter filter))
            {
                if (!renderer.TryGetComponent(out filter))
                    return null;

                _meshRendererData.Add(renderer, filter);
            }

            return filter;
        }

        /// <summary>
        /// Creates a sprite.
        /// </summary>
        public static Sprite ToSprite(this Texture2D self, in Vector2 pivot, float pixelsPerUnit = 100f, uint extrude = 0, SpriteMeshType meshType = SpriteMeshType.Tight, in Vector4 border = default)
        {
            return Sprite.Create(self, new Rect(0f, 0f, self.width, self.height), pivot, pixelsPerUnit, extrude, meshType, border);
        }

        /// <summary>
        /// Creates a sprite with the pivot in the center.
        /// </summary>
        public static Sprite ToSprite(this Texture2D self, float pixelsPerUnit = 100f, uint extrude = 0, SpriteMeshType meshType = SpriteMeshType.Tight, in Vector4 border = default)
        {
            float x = self.width;
            float y = self.height;
            return Sprite.Create(self, new Rect(0f, 0f, x, y), new Vector2(x * 0.5f, y * 0.5f), pixelsPerUnit, extrude, meshType, border);
        }
    }
}
