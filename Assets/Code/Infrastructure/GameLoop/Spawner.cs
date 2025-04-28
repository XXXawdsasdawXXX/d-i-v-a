using System.Linq;
using Code.Game.Effects;
using Code.Infrastructure.ServiceLocator;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting;

namespace Code.Infrastructure.GameLoop
{
    [Preserve]
    public class Spawner : IService, IInitializeListener
    {
        private GameEventDispatcher _gameEventDispatcher;

        public UniTask GameInitialize()
        {
            _gameEventDispatcher = Container.Instance.GetService<GameEventDispatcher>();
            
            return UniTask.CompletedTask;
        }
        
        public T Instantiate<T>(T prefab) where T : Object
        {
            T instance = Object.Instantiate(prefab);

            IGameListeners[] listeners = instance.GameObject().GetComponentsInChildren<IGameListeners>(true).ToArray();

            foreach (IGameListeners listener in listeners)
            {
                _gameEventDispatcher.AddRuntimeListener(listener);
            }
            
            return instance;
        }

        public T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation) where T : Object
        {
            T instance = Object.Instantiate(prefab, position, rotation);

            IGameListeners[] listeners = instance.GameObject().GetComponentsInChildren<IGameListeners>(true).ToArray();

            foreach (IGameListeners listener in listeners)
            {
                _gameEventDispatcher.AddRuntimeListener(listener);
            }
            
            return instance;
        }

        public void Destroy(GameObject instance)
        {
            IGameListeners[] listeners = instance.GetComponentsInChildren<IGameListeners>(true).ToArray();

            foreach (IGameListeners listener in listeners)
            {
                _gameEventDispatcher.RemoveRuntimeListener(listener);
            }
            
            Object.Destroy(instance);
        }
    }
}