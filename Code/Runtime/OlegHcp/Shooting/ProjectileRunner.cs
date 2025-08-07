using System.Collections.Generic;
using OlegHcp.Pool;
using UnityEngine;

namespace OlegHcp.Shooting
{
    internal class PoolableSet : HashSet<Component>
    {
    }

    internal class ProjectileRunner : IObjectFactory<PoolableSet>
    {
        private static ProjectileRunner _instance;
        private List<IProjectile> _items = new List<IProjectile>();
        private Stack<IProjectile> _newItems = new Stack<IProjectile>();
        private Stack<IProjectile> _deadItems = new Stack<IProjectile>();
        private bool _locked;
        private ObjectPool<PoolableSet> _objectPool;

        public static ProjectileRunner I => _instance ?? (_instance = new ProjectileRunner());

        public ProjectileRunner()
        {
            _objectPool = new ObjectPool<PoolableSet>(this);
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

        public PoolableSet GetSet()
        {
            return _objectPool.Get();
        }

        public void ReleaseSet(ref PoolableSet set)
        {
            if (set != null)
            {
                _objectPool.Clear();
                _objectPool.Release(set);
                set = null;
            }
        }

        PoolableSet IObjectFactory<PoolableSet>.Create()
        {
            return new PoolableSet();
        }

        private void OnLateUpdate()
        {
            _locked = true;
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].OnUpdate();
            }
            _locked = false;

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
