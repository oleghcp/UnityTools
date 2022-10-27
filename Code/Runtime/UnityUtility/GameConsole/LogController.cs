#if !UNITY_2019_2_OR_NEWER || INCLUDE_UNITY_UI
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Pool;

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
            getLine().SetText(text, info, color);

            LogLine getLine()
            {
                if (_root.childCount < _terminal.Options.LinesLimit)
                    return _lines.Place(_pool.Get());

                return _root.GetChild(0)
                            .GetComponent<LogLine>()
                            .Reuse();
            }
        }

        public void Clear()
        {
            _pool.Release(_lines);
            _lines.Clear();
        }

        public void OnScroll()
        {
            _border.SetActive(_root.anchoredPosition.y < -0.1f);
        }

        LogLine IObjectFactory<LogLine>.Create()
        {
            LogLine newLine = _prefab.Install(_root);
            newLine.gameObject.SetActive(true);

            return newLine;
        }
    }
}
#endif
