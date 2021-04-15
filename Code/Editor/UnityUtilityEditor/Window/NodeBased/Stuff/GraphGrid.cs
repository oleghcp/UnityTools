using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.MathExt;

namespace UnityUtilityEditor.Window.NodeBased.Stuff
{
    internal class GraphGrid
    {
        private readonly Color SMALL_COLOR = Colours.Grey.AlterA(0.3f);
        private readonly Color LARGE_COLOR = Colours.Grey.AlterA(0.5f);
        public const float SMALL_STEP = 20f;
        private const float LARGE_STEP = 160f;

        private GraphEditorWindow _window;

        public GraphGrid(GraphEditorWindow window)
        {
            _window = window;
        }

        public void Draw()
        {
            Rect worldRect = _window.Camera.GetWorldRect();

            Handles.color = SMALL_COLOR;
            drawInternal(SMALL_STEP);
            Handles.color = LARGE_COLOR;
            drawInternal(LARGE_STEP);
            Handles.color = Colours.White;

            void drawInternal(float spacing)
            {
                Vector2 winSize = _window.position.size;

                int widthDivs = (worldRect.width / spacing).Ceiling() + 1;
                int heightDivs = (worldRect.height / spacing).Ceiling() + 1;

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
