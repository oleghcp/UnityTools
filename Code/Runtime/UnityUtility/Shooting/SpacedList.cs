using System;

namespace UnityUtility.Shooting
{
    internal class SpacedList
    {
        private IProjectile[] _items;
        private int _count;

        public int Count => _count;
        public IProjectile this[int index] => _items[index];

        public SpacedList() : this(16)
        {

        }

        public SpacedList(int capacity)
        {
            _items = new IProjectile[capacity];
        }

        public void Insert(IProjectile item)
        {
            for (int i = 0; i < _count; i++)
            {
                if (_items[i] == null)
                {
                    _items[i] = item;
                    return;
                }
            }

            if (_count == _items.Length)
                Array.Resize(ref _items, _count * 2);

            _items[_count++] = item;
        }

        public void Remove(IProjectile item)
        {
            for (int i = 0; i < _count; i++)
            {
                if (_items[i] == item)
                {
                    _items[i] = null;

                    if (i == _count - 1)
                        _count--;

                    break;
                }
            }
        }
    }
}
