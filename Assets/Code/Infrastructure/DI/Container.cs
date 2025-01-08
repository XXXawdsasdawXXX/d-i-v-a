using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Code.Data;
using Code.Entities;
using Code.Infrastructure.CustomActions;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Reactions;
using Code.Infrastructure.Save;
using Code.Infrastructure.Services.Interactions;
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
        private List<InteractionObserver> _interactionObservers = new();
        private List<IMono> _mono = new();
        private List<IGetter> _getters = new();
        private List<Reaction> _reactions = new();

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
            InitList(ref _interactionObservers);
            InitList(ref _mono);
            InitList(ref _getters);
            InitList(ref _reactions);
        }

        private void InitList<T>(ref List<T> list)
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();

            IEnumerable<Type> serviceTypes = types.Where(t =>
                typeof(T).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract &&
                !typeof(MonoBehaviour).IsAssignableFrom(t));

            foreach (Type serviceType in serviceTypes)
            {
                if (Activator.CreateInstance(serviceType) is T service)
                {
                    list.Add(service);
                }
            }

            IEnumerable<T> mbServices = _allObjects.OfType<T>();
            if (mbServices.Any())
            {
                list.AddRange(mbServices);
            }

            Debugging.Log(
                $"Init {typeof(T).Name} | mb count = {mbServices.Count()}| list count = {list.Count}",
                Debugging.Type.DiContainer);
        }

        public UniWindowController GetUniWindowController()
        {
            return _uniWindowController;
        }

        public T FindConfig<T>() where T : ScriptableObject
        {
            foreach (ScriptableObject scriptableObject in _configs)
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
            foreach (IService service in _services)
            {
                if (service is T findService)
                {
                    return findService;
                }
            }

            return default;
        }


        public T FindGetter<T>() where T : class
        {
            foreach (IGetter getter in _getters)
            {
                if (getter is T findGetter)
                {
                    return findGetter;
                }
            }

            return default;
        }

        public T FindStorage<T>() where T : IStorage
        {
            foreach (IStorage storage in _storages)
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
            foreach (Entity entity in _entities)
            {
                if (entity is T findEntity)
                {
                    return findEntity;
                }
            }

            return default;
        }
        
        public T FindInteractionObserver<T>() where T : InteractionObserver
        {
            foreach (InteractionObserver interactionObserver in _interactionObservers)
            {
                if (interactionObserver is T findInteractionObserver)
                {
                    return findInteractionObserver;
                }
            }

            return default;
        }
        
        public T FindReaction<T>() where T : Reaction
        {
            foreach (Reaction reaction in _reactions)
            {
                if (reaction is T characterReaction)
                {
                    return characterReaction;
                }
            }

            return null;
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
            List<T> list = new();

            list.AddRange(_services.OfType<T>().ToList());
            list.AddRange(_storages.OfType<T>().ToList());
            list.AddRange(_entities.OfType<T>().ToList());
            list.AddRange(_customActions.OfType<T>().ToList());
            list.AddRange(_interactionObservers.OfType<T>().ToList());
            list.AddRange(_reactions.OfType<T>().ToList());
            list.AddRange(_mono.OfType<T>().ToList());

            IEnumerable<T> mbListeners = _allObjects.OfType<T>();
            foreach (T mbListener in mbListeners)
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