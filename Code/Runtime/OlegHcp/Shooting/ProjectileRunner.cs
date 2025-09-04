using System.Collections.Generic;
using OlegHcp.Pool;
using UnityEngine;

namespace OlegHcp.Shooting
{
    internal class ProjectileRunner
    {
        private static ProjectileRunner _instance;

        private List<IProjectile> _items = new List<IProjectile>();
        private Stack<IProjectile> _newItems = new Stack<IProjectile>();
        private Stack<IProjectile> _deadItems = new Stack<IProjectile>();
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
                _newItems.Push(projectile);
            else
                _items.Add(projectile);
        }

        public void Remove(IProjectile projectile)
        {
            if (_locked)
                _deadItems.Push(projectile);
            else
                _items.Remove(projectile);
        }

        public HashSet<Component> GetSet()
        {
            return _objectPool.Get();
        }

        public void ReleaseSet(ref HashSet<Component> set)
        {
            if (set != null)
            {
                _objectPool.Clear();
                _objectPool.Release(set);
                set = null;
            }
        }

        private void OnLateUpdate()
        {
            try
            {
                _locked = true;
                for (int i = 0; i < _items.Count; i++)
                {
                    _items[i].OnUpdate();
                }
                _locked = false;
            }
            finally
            {
                while (_deadItems.Count > 0)
                {
                    _items.Remove(_deadItems.Pop());
                }

                while (_newItems.Count > 0)
                {
                    _items.Add(_newItems.Pop());
                }
            }
        }
    }
}
