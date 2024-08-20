using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Services.Pools
{
    public interface  IPoolEntity
    {
        /// <summary>
        /// Pool methods
        /// </summary>
        void Init(params object[] parameters);
        void Enable();
        void Disable();
    }
    
    [Serializable]
    public class MonoPool<T> where T: MonoBehaviour, IPoolEntity
    {
        [SerializeField] private Transform _root;
        [SerializeField] private T _prefab;
        [SerializeField] private List<T> _all = new();
        [SerializeField] private List<T> _enabled = new();

        public T GetNext(params object[] initParams)
        {
            T entity = GetDisabledEntity() ?? AddNewEntity(initParams);
            _enabled.Add(entity);
            entity.Enable();
            return entity;
        }

        private T GetDisabledEntity()
        {
            return _all.FirstOrDefault(entity => entity != null && !entity.gameObject.activeSelf);
        }

        private T AddNewEntity(params object[] initParams)
        {
            var entity = GameObject.Instantiate(_prefab, _root);
            entity.Init(initParams);
            _all.Add(entity);
            return entity;
        }

        public IEnumerable<T> GetAll()
        {
            return _all;
        }

        public IEnumerable<T> GetAllEnabled()
        {
            return _enabled;
        }

        public void Disable(T entity)
        {
            if (entity == null || !entity.gameObject.activeSelf) return;
            entity.Disable();
            _enabled.Remove(entity);
        }

        public void DisableAll()
        {
            foreach (var entity in _all)
            {
                entity.Disable();
            }

            _enabled.Clear();
        }

        public T GetByIndex(int tabIndex)
        {
            return _all[tabIndex];
        }
    }
}