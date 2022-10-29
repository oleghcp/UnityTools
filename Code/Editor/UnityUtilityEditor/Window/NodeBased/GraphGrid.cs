#if UNITY_2019_3_OR_NEWER
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.MathExt;

namespace UnityUtilityEditor.Window.NodeBased
{
    internal class GraphGrid
    {
        private readonly Color _smallColor = Colours.Grey.AlterA(0.3f);
        private readonly Color _largeColor = Colours.Grey.AlterA(0.5f);
        public const float SMALL_STEP = 20f;
        private const float LARGE_STEP = 160f;

        private GraphEditorWindow _window;

        public GraphGrid(GraphEditorWindow window)
        {
            _window = window;
        }

        public void Draw()
        {
            Rect worldRect = _window.Camera.WorldRect;

            Handles.color = _smallColor;
            drawInternal(SMALL_STEP);
            Handles.color = _largeColor;
            drawInternal(LARGE_STEP);
            Handles.color = Colours.White;

            void drawInternal(float spacing)
            {
                Vector2 winSize = _window.MapSize;

                int widthDivs = (int)(worldRect.width / spacing).Ceiling() + 1;
                int heightDivs = (int)(worldRect.height / spacing).Ceiling() + 1;

                Vector2 lineStartPos = new Vector2(worldRect.x.Round(spacing),
                                                   worldRect.y.Round(spacing));

                lineStartPos = _window.Camera.WorldToScreen(lineStartPos);
                spacing /= _window.Camera.Size;

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
}
#endif
