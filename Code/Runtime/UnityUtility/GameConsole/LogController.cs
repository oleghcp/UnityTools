using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Collections;
using UnityUtility.MathExt;

#pragma warning disable CS0649
namespace UnityUtility.GameConsole
{
    internal class LogController : UiMonoBehaviour
    {
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
            m_lines.AddAndGet(m_pool.Get())
                   .SetText(text, color);
        }

        public void Clear()
        {
            m_pool.Release(m_lines);
            m_lines.Clear();
        }

        public void Scroll(float dir)
        {
            Vector2 newPos = _root.anchoredPosition + new Vector2(0f, dir * -100f);
            newPos.y = newPos.y.Clamp(-_root.rect.size.y, 0f);
            _root.anchoredPosition = newPos;

            _border.SetActive(newPos.y < 0f);
        }

        private LogLine f_createLine()
        {
            LogLine newLine = _prefab.Install(_root);
            newLine.gameObject.SetActive(true);

            return newLine;
        }
    }
}
