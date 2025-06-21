using OlegHcp;
using OlegHcp.Engine;
using OlegHcp.Mathematics;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Window.NodeBased
{
    internal static class GraphGrid
    {
        private static readonly Color _smallColor = Colours.Grey.AlterA(0.3f);
        private static readonly Color _largeColor = Colours.Grey.AlterA(0.5f);
        public const float SMALL_STEP = 20f;
        private const float LARGE_STEP = 160f;

        public static void Draw(GraphEditorWindow window)
        {
            Vector2 winSize = window.MapSize;
            GraphCamera camera = window.Camera;
            Rect worldRect = camera.WorldRect;

            Handles.color = _smallColor;
            DrawInternal(camera, worldRect, winSize, SMALL_STEP);
            Handles.color = _largeColor;
            DrawInternal(camera, worldRect, winSize, LARGE_STEP);
            Handles.color = Colours.White;
        }

        private static void DrawInternal(GraphCamera camera, in Rect worldRect, Vector2 winSize, float spacing)
        {
            int widthDivs = (int)(worldRect.width / spacing).Ceiling() + 1;
            int heightDivs = (int)(worldRect.height / spacing).Ceiling() + 1;

            Vector2 lineStartPos = new Vector2(worldRect.x.Round(spacing),
                                               worldRect.y.Round(spacing));

            lineStartPos = camera.WorldToScreen(lineStartPos);
            spacing /= camera.Size;

            for (int i = 0; i < widthDivs; i++)
            {
                float xPos = lineStartPos.x + spacing * i;
                Handles.DrawLine(new Vector3(xPos, 0f), new Vector3(xPos, winSize.y));
            }

            for (int j = 0; j < heightDivs; j++)
            {
                float yPos = lineStartPos.y + spacing * j;
                Handles.DrawLine(new Vector3(0f, yPos), new Vector3(winSize.x, yPos));
            }
        }
    }
}
