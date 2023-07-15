using System.Collections.Generic;

namespace UnityUtility.Shooting
{
    internal class ProjectileRunner
    {
        private static ProjectileRunner _intance;
        private SpacedList _items = new SpacedList();
        private Stack<IProjectile> _newItems = new Stack<IProjectile>();
        private Stack<IProjectile> _deadItems = new Stack<IProjectile>();
        private bool _locked;

        public static ProjectileRunner I => _intance ?? (_intance = new ProjectileRunner());

        public ProjectileRunner()
        {
            ApplicationUtility.OnLateUpdate_Event += OnLateUpdate;
        }

        public void Add(IProjectile projectile)
        {
            if (_locked)
                _newItems.Push(projectile);
            else
                _items.Insert(projectile);
        }

        public void Remove(IProjectile projectile)
        {
            if (_locked)
                _deadItems.Push(projectile);
            else
                _items.Remove(projectile);
        }

        private void OnLateUpdate()
        {
            _locked = true;
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i]?.OnTick();
            }
            _locked = false;

            while (_deadItems.Count > 0)
            {
                _items.Remove(_deadItems.Pop());
            }

            while (_newItems.Count > 0)
            {
                _items.Insert(_newItems.Pop());
            }
        }
    }
}
