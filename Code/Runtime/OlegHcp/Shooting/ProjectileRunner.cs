using System.Collections.Generic;
using System.Runtime.CompilerServices;
using OlegHcp.Pool;
using UnityEngine;

namespace OlegHcp.Shooting
{
    internal class ProjectileRunner
    {
        private static ProjectileRunner _instance;

        private List<IProjectile> _items = new List<IProjectile>();
        private Stack<ChangeToken> _changes = new Stack<ChangeToken>();
        private bool _locked;
        private ObjectPool<HashSet<Component>> _objectPool;
        private List<HitInfo> _hits = new List<HitInfo>();
        private List<HitInfo2D> _hits2D = new List<HitInfo2D>();

        public static ProjectileRunner I => _instance ?? (_instance = new ProjectileRunner());

        public List<HitInfo> Hits => _hits;
        public List<HitInfo2D> Hits2D => _hits2D;

        public ProjectileRunner()
        {
            _objectPool = new ObjectPool<HashSet<Component>>(() => new HashSet<Component>());
            ApplicationUtility.OnLateUpdate_Event += OnLateUpdate;
        }

        public void Add(IProjectile projectile)
        {
            if (_locked)
            {
                _changes.Push(new ChangeToken(projectile, ChangeActionType.Add));
                return;
            }

            _items.Add(projectile);
        }

        public void Remove(IProjectile projectile)
        {
            if (_locked)
            {
                _changes.Push(new ChangeToken(projectile, ChangeActionType.Remove));
                return;
            }

            _items.Remove(projectile);
        }

        public HashSet<Component> GetSet()
        {
            return _objectPool.Get();
        }

        public void ReleaseSet(HashSet<Component> set)
        {
            if (set == null)
                return;

            set.Clear();
            _objectPool.Release(set);
        }

        private void OnLateUpdate()
        {
            _locked = true;

            try
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    _items[i].OnUpdate();
                }
            }
            finally
            {
                _locked = false;

                while (_changes.Count > 0)
                {
                    _changes.Pop()
                            .Proceed(_items);
                }
            }
        }

        private struct ChangeToken
        {
            private IProjectile _item;
            private ChangeActionType _actionType;

            public ChangeToken(IProjectile item, ChangeActionType actionType)
            {
                _item = item;
                _actionType = actionType;
            }

            public void Proceed(List<IProjectile> mainCollection)
            {
                switch (_actionType)
                {
                    case ChangeActionType.Add:
                        mainCollection.Add(_item);
                        break;

                    case ChangeActionType.Remove:
                        mainCollection.Remove(_item);
                        break;

                    default:
                        throw new SwitchExpressionException(_actionType);
                }
            }
        }

        private enum ChangeActionType
        {
            Add,
            Remove,
        }
    }
}
