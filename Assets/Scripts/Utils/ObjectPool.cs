using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniKingdom.Utils
{
    /// <summary>
    /// A generic object pool for reusable components.
    /// </summary>
    /// <typeparam name="T">Component type</typeparam>
    public class ObjectPool<T> where T : Component
    {
        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly Queue<T> _pool = new Queue<T>();
        private readonly Action<T> _onGet;
        private readonly Action<T> _onReturn;

        public ObjectPool(T prefab, Transform parent = null, Action<T> onGet = null, Action<T> onReturn = null)
        {
            _prefab = prefab;
            _parent = parent;
            _onGet = onGet;
            _onReturn = onReturn;
        }

        public void PreWarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                T obj = CreateNewObject();
                obj.gameObject.SetActive(false);
                _pool.Enqueue(obj);
            }
        }

        public T Get()
        {
            T obj;
            if (_pool.Count > 0)
            {
                obj = _pool.Dequeue();
            }
            else
            {
                obj = CreateNewObject();
            }

            obj.gameObject.SetActive(true);
            _onGet?.Invoke(obj);
            return obj;
        }

        public void Return(T obj)
        {
            if (obj == null) return;
            
            _onReturn?.Invoke(obj);
            obj.gameObject.SetActive(false);
            
            if (_parent != null)
            {
                obj.transform.SetParent(_parent);
            }
            
            _pool.Enqueue(obj);
        }

        private T CreateNewObject()
        {
            T obj = UnityEngine.Object.Instantiate(_prefab, _parent);
            return obj;
        }
    }
}
