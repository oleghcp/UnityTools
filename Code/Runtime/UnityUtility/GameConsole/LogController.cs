using System.Collections.Generic;
using UnityEngine;
using UnityUtility.MathExt;
using UnityUtility.Pool;

#pragma warning disable CS0649
namespace UnityUtility.GameConsole
{
    [DisallowMultipleComponent]
    internal class LogController : UiMonoBehaviour, IObjectFactory<LogLine>
    {
        [SerializeField]
        private LogLine _prefab;
        [SerializeField]
        private GameObject _border;
        [SerializeField]
        private RectTransform _root;

        private Terminal _terminal;
        private ObjectPool<LogLine> _pool;
        private List<LogLine> _lines;

        private void Awake()
        {
            _pool = new ObjectPool<LogLine>(this);
            _lines = new List<LogLine>();
        }

        public void SetUp(Terminal terminal)
        {
            _terminal = terminal;
        }

        public void WriteLine(Color color, string text, string info = null)
        {
            GetLine().SetText(text, info, color);
        }

        public void Clear()
        {
            _pool.Release(_lines);
            _lines.Clear();
        }

        public void Scroll(float dir)
        {
            Vector2 newPos = _root.anchoredPosition + new Vector2(0f, dir * -100f);
            newPos.y = newPos.y.Clamp(-_root.rect.size.y, 0f);
            _root.anchoredPosition = newPos;

            _border.SetActive(newPos.y < 0f);
        }

        private LogLine GetLine()
        {
            if (_root.childCount < _terminal.Options.LinesLimit)
                return _lines.Place(_pool.Get());

            return _root.GetChild(0)
                        .GetComponent<LogLine>()
                        .Reuse();
        }

        LogLine IObjectFactory<LogLine>.Create()
        {
            LogLine newLine = _prefab.Install(_root);
            newLine.gameObject.SetActive(true);

            return newLine;
        }
    }
}
