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

        private Terminal m_terminal;
        private ObjectPool<LogLine> m_pool;
        private List<LogLine> m_lines;

        private void Awake()
        {
            m_pool = new ObjectPool<LogLine>(f_createLine);
            m_lines = new List<LogLine>();
        }

        public void SetUp(Terminal terminal)
        {
            m_terminal = terminal;
        }

        public void WriteLine(Color color, string text, string info = null)
        {
            f_getLine().SetText(text, info, color);
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

        private LogLine f_getLine()
        {
            if (_root.childCount < m_terminal.Options.LinesLimit)
                return m_lines.Place(m_pool.Get());

            return _root.GetChild(0)
                        .GetComponent<LogLine>()
                        .Reuse();
        }
    }
}
