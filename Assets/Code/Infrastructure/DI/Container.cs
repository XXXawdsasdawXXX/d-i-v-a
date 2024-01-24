using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Code.Components.Character;
using Code.Components.Characters;
using Code.Data.Interfaces;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Save;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.DI
{
    public class Container : MonoBehaviour
    {
        public static Container Instance;

        [SerializeField] private Character _character;  
        [SerializeField] private List<ScriptableObject> _configs;
        private List<IService> _services = new();
        private List<IStorage> _storages = new();
        private List<ILiveStateLogic> _liveStateLogics = new();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
            Instance = this;

            InitList(ref _services);
            InitList(ref _storages);
            InitList(ref _liveStateLogics);
        }

        private void InitList<T>(ref List<T> list)
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();

            var serviceTypes = types.Where(t =>
                typeof(T).IsAssignableFrom(t) && t.IsClass && !typeof(MonoBehaviour).IsAssignableFrom(t));

            foreach (var serviceType in serviceTypes)
            {
                if (Activator.CreateInstance(serviceType) is T service)
                {
                    list.Add(service);
                }
            }

            var mbServices = FindObjectsOfType<MonoBehaviour>().OfType<T>();
            if (mbServices.Any())
            {
                list.AddRange(mbServices);
            }

            Debugging.Instance.Log(
                $"Init {typeof(T).Name} | mb count = {mbServices.Count()}| list count = {list.Count}",
                Debugging.Type.DiContainer);
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


        public T FindLiveStateLogic<T>() where T : ILiveStateLogic
        {
            foreach (var liveStateLogic in _liveStateLogics)
            {
                if (liveStateLogic is T typedLiveStateLogic)
                {
                    return typedLiveStateLogic;
                }
            }
            return default;
        }

        public Character GetCharacter() => _character;
        public List<IGameListeners> GetGameListeners()
        {
            var listenersList = new List<IGameListeners>();

            listenersList.AddRange(_services.OfType<IGameListeners>().ToList());
            listenersList.AddRange(_storages.OfType<IGameListeners>().ToList());
            listenersList.AddRange(_liveStateLogics.OfType<IGameListeners>().ToList());

            var mbListeners = FindObjectsOfType<MonoBehaviour>().OfType<IGameListeners>();
            foreach (var mbListener in mbListeners)
            {
                if (!listenersList.Contains(mbListener))
                {
                    listenersList.Add(mbListener);
                }
            }

            return listenersList;
        }

        public List<IProgressReader> GetProgressReaders()
        {
            var listenersList = new List<IProgressReader>();

            listenersList.AddRange(_services.OfType<IProgressReader>().ToList());
            listenersList.AddRange(_storages.OfType<IProgressReader>().ToList());
            listenersList.AddRange(_liveStateLogics.OfType<IProgressReader>().ToList());

            
            var mbListeners = FindObjectsOfType<MonoBehaviour>().OfType<IProgressReader>();
            foreach (var mbListener in mbListeners)
            {
                if (!listenersList.Contains(mbListener))
                {
                    listenersList.Add(mbListener);
                }
            }

            return listenersList;
        }
    }
}