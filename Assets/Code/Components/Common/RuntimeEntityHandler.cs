using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components.Common
{

    public class RuntimeEntityHandler: MonoBehaviour
    {
        private GameEventDispatcher _gameEventDispatcher;
        private IGameListeners[] _listeners;
        
        private void OnEnable()
        {
            _listeners ??= GetComponentsInChildren<IGameListeners>(true);
            _gameEventDispatcher ??= Container.Instance.FindService<GameEventDispatcher>();

            foreach (var listener in _listeners)
            {
                _gameEventDispatcher.InitializeRuntimeListener(listener);
            }
        }

        private void OnDisable()
        {
            foreach (var listener in _listeners)
            {
                _gameEventDispatcher.RemoveRuntimeListener(listener);
            }
        }
    }
}