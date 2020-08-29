using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Collections;
using UnityUtility.MathExt;

#pragma warning disable CS0649
namespace UnityUtility.GameConsole
{
    internal class LogController : UiMonoBehaviour
    {
        private const float PADDING = 5f;

        [SerializeField]
        private LogLine _prefab;
        [SerializeField]
        private GameObject _border;
        [SerializeField]
        private RectTransform _root;

        private ObjectPool<LogLine> m_pool;
        private List<LogLine> m_lines;

        private void Awake()
        {
            m_pool = new ObjectPool<LogLine>(f_createLine);
            m_lines = new List<LogLine>();
        }

        public void WriteLine(string text, Color color)
        {
            LogLine newLine = m_pool.Get();

            newLine.Text.color = color;
            newLine.Text.text = text;

            Vector2 shift = new Vector2(0f, newLine.Text.preferredHeight + PADDING);

            for (int i = 0; i < m_lines.Count; i++)
            {
                m_lines[i].rectTransform.anchoredPosition += shift;
            }

            m_lines.Add(newLine);
        }

        public void Clear()
        {
            for (int i = 0; i < m_lines.Count; i++)
            {
                m_pool.Release(m_lines[i]);
            }

            m_lines.Clear();
        }

        public void Scroll(float dir)
        {
            Vector2 newPos = _root.anchoredPosition + new Vector2(0f, dir * -100f);
            newPos.y = newPos.y.Clamp(-f_getFullHeight(), 0f);
            _root.anchoredPosition = newPos;

            _border.SetActive(newPos.y < 0f);
        }

        //--//

        private float f_getFullHeight()
        {
            float h = 0f;

            for (int i = 0; i < m_lines.Count; i++)
            {
                h += m_lines[i].Text.preferredHeight + PADDING;
            }

            return h;
        }

        private LogLine f_createLine()
        {
            LogLine newLine = _prefab.Install(_root);
            newLine.gameObject.SetActive(true);

            return newLine;
        }
    }
}
