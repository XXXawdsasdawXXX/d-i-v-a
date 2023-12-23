using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.DI
{
    public class Container : MonoBehaviour
    {
        public static Container Instance;
        [SerializeField] private CoroutineController coroutineController;
        [SerializeField] private List<ScriptableObject> _configs;
        private List<IService> _services = new();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
            Instance = this;
            
            InitList(out _services);
        }

        private void InitList<T>(out List<T> list)
        {
            list = new List<T>();

            var types = Assembly.GetExecutingAssembly().GetTypes();

            var serviceTypes = types.Where(t => typeof(T).IsAssignableFrom(t) && t.IsClass);

            foreach (var serviceType in serviceTypes)
            {
                if (Activator.CreateInstance(serviceType) is T service)
                {
                    list.Add(service);
                }
            }

            var mbServices = FindObjectsOfType<MonoBehaviour>().OfType<T>();
            var enumerable = mbServices as T[] ?? mbServices.ToArray();
            if (enumerable.Any())
            {
                list.AddRange(enumerable);
            }
            
            foreach (var l in list)
            {
                Debugging.Instance.Log($"Init {l.GetType().Name} ", Debugging.Type.DiContainer);
            }
            Debugging.Instance.Log(
                $"Init {typeof(T).Name} | mb count = {enumerable.Count()}| list count = {list.Count}",
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
                Debugging.Instance.Log($"Try find {typeof(T)}  {service is T}  ", Debugging.Type.DiContainer);
                if (service is T findService)
                {
                      Debugging.Instance.Log($"return find {typeof(T)} ", Debugging.Type.DiContainer);
                    return findService;
                }
            }

            return default;
        }

        public CoroutineController GetCoroutineRunner()
        {
            return coroutineController;
        }

        public List<IGameListeners> GetGameListeners()
        {
            var listenersList = new List<IGameListeners>();

            listenersList.AddRange(_services.OfType<IGameListeners>().ToList());

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
    }
}