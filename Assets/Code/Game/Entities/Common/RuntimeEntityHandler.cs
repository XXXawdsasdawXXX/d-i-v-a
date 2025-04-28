using Code.Infrastructure.GameLoop;
using Code.Infrastructure.ServiceLocator;
using UnityEngine;

namespace Code.Game.Entities.Common
{

    public class RuntimeEntityHandler: MonoBehaviour
    {
        private GameEventDispatcher _gameEventDispatcher;
        private IGameListeners[] _listeners;
        
        private void OnEnable()
        {
            _listeners ??= GetComponentsInChildren<IGameListeners>(true);
            _gameEventDispatcher ??= Container.Instance.GetService<GameEventDispatcher>();

            foreach (IGameListeners listener in _listeners)
            {
                _gameEventDispatcher.AddRuntimeListener(listener);
            }
        }

        private void OnDisable()
        {
            foreach (IGameListeners listener in _listeners)
            {
                _gameEventDispatcher.RemoveRuntimeListener(listener);
            }
        }
    }
}