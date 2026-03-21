using System.Collections.Generic;
using UnityEngine;

namespace CardWar.View.Utils
{
    public class GameObjectPool<T>  where T : MonoBehaviour, IPoolable
    {
        private readonly T _prefab;
        private readonly Stack<T> _pool;
        private readonly GameObject _poolParent;

        public GameObjectPool(T prefab, int initCount = 0)
        {
            _pool = new Stack<T>(initCount);
            _prefab = prefab;
            _poolParent = new GameObject(typeof(T).Name + "Pool");
            
            if (initCount < 1)
                return;
            
            Preheat(initCount);
        }

        private void Preheat(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var obj = Create();
                obj.gameObject.SetActive(false);
                _pool.Push(obj);
            }
        }
        
        public T Get()
        {
            var obj = _pool.Count < 1 ? Create() : _pool.Pop();
            
            obj.gameObject.SetActive(true);
            obj.OnPoolRelease();
            return obj;
        }

        private T Create()
        {
            var obj = Object.Instantiate(_prefab, _poolParent.transform);
            obj.ReturnToPool += OnReturnToPool;
            return obj;
            
            void OnReturnToPool()
            {
                obj.gameObject.SetActive(false);
                _pool.Push(obj);
            }
        }
    }
}