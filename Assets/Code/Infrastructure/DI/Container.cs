using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Code.Components;
using Code.Components.Entities;
using Code.Components.Items;
using Code.Data.Interfaces;
using Code.Infrastructure.CustomActions;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Save;
using Code.Services;
using Code.Utils;
using Kirurobo;
using UnityEngine;

namespace Code.Infrastructure.DI
{
    public class Container : MonoBehaviour
    {
        public static Container Instance;
        
        [SerializeField] private UniWindowController _uniWindowController;
        [SerializeField] private List<ScriptableObject> _configs;

        private MonoBehaviour[] _allObjects;
        private List<IService> _services = new();
        private List<IStorage> _storages = new();
        private List<CustomAction> _customActions = new();
        private List<Entity> _entities = new();
        private List<Item> _items = new();
        private List<InteractionObserver> _interactionObservers = new();
        private List<IMono> _mono = new();
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
            Instance = this;

            _allObjects = FindObjectsOfType<MonoBehaviour>();
            InitList(ref _services);
            InitList(ref _storages);
            InitList(ref _customActions);
            InitList(ref _entities); 
            InitList(ref _items); 
            InitList(ref _interactionObservers); 
            InitList(ref _mono);
        }

        private void InitList<T>(ref List<T> list)
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();

            var serviceTypes = types.Where(t =>
                typeof(T).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && !typeof(MonoBehaviour).IsAssignableFrom(t));

            foreach (var serviceType in serviceTypes)
            {
                if (Activator.CreateInstance(serviceType) is T service)
                {
                    list.Add(service);
                }
            }

            var mbServices = _allObjects.OfType<T>();
            if (mbServices.Any())
            {
                list.AddRange(mbServices);
            }

            Debugging.Instance.Log(
                $"Init {typeof(T).Name} | mb count = {mbServices.Count()}| list count = {list.Count}",
                Debugging.Type.DiContainer);
        }

        public UniWindowController GetUniWindowController()
        {
            return _uniWindowController;
        }
        public T FindConfig<T>() where T : ScriptableObject
        {
            foreach (var scriptableObject in _configs)
            {
                if (scriptableObject is T findConfig)
                {
                    return findConfig;
                }
            }

            return null;
        }

        public T FindService<T>() where T : IService
        {
            foreach (var service in _services)
            {
                if (service is T findService)
                {
                    return findService;
                }
            }

            return default;
        }

        public T FindStorage<T>() where T : IStorage 
        {
            foreach (var storage in _storages)
            {
                if (storage is T typedStorage)
                {
                    return typedStorage;
                }
            }

            return default;
        }


        public T FindEntity<T>() where T : Entity
        {
            foreach (var entity in _entities)
            {
                if (entity is T findEntity)
                {
                    return findEntity;
                }
            }

            return default;
        }
        
        public T FindItem<T>() where T : Item
        {
            foreach (var item in _items)
            {
                if (item is T findItem)
                {
                    return findItem;
                }
            }

            return default;
        }
        public T FindInteractionObserver<T>() where T : InteractionObserver
        {
            foreach (var interactionObserver in _interactionObservers)
            {
                if (interactionObserver is T findInteractionObserver)
                {
                    return findInteractionObserver;
                }
            }

            return default;
        }

        public List<IGameListeners> GetGameListeners()
        {
            return GetContainerComponents<IGameListeners>();
        }

        public List<IProgressReader> GetProgressReaders()
        {
            return GetContainerComponents<IProgressReader>();
        }

        private List<T> GetContainerComponents<T>()
        {
            var list = new List<T>();

            list.AddRange(_services.OfType<T>().ToList());
            list.AddRange(_storages.OfType<T>().ToList());
            list.AddRange(_entities.OfType<T>().ToList());
            list.AddRange(_items.OfType<T>().ToList());
            list.AddRange(_customActions.OfType<T>().ToList());
            list.AddRange(_interactionObservers.OfType<T>().ToList());
            list.AddRange(_mono.OfType<T>().ToList());

            var mbListeners = _allObjects.OfType<T>();
            foreach (var mbListener in mbListeners)
            {
                if (!list.Contains(mbListener))
                {
                    list.Add(mbListener);
                }
            }

            return list;
        }
    }
}