using System.Collections.Generic;
using Code.Infrastructure.DI;
using UnityEngine;

namespace Code.Infrastructure.GameLoop
{
    public class GameEventDispatcher : MonoBehaviour
    {
        private readonly List<IGameInitListener> _initListeners = new();
        private readonly List<IGameLoadListener> _loadListeners = new();
        private readonly List<IGameStartListener> _startListeners = new();
        private readonly List<IGameTickListener> _tickListeners = new();
        private readonly List<IGameExitListener> _exitListeners = new();

        public void Awake()
        {
            InitializeListeners();
            NotifyGameInit();
            NotifyGameLoad();
        }
        
        private void Start()
        {
            NotifyGameStart();
        }

        private void Update()
        {
            NotifyGameTick();
        }

        private void OnApplicationQuit()
        {
            NotifyGameExit();
        }

        private void InitializeListeners()
        {
            var gameListeners = Container.Instance.GetGameListeners();

            foreach (var listener in gameListeners)
            {
                if (listener is IGameInitListener initListener)
                    _initListeners.Add(initListener);
                if (listener is IGameLoadListener loadListener)
                    _loadListeners.Add(loadListener);
                if (listener is IGameStartListener startListener)
                    _startListeners.Add(startListener);
                if (listener is IGameTickListener tickListener)
                    _tickListeners.Add(tickListener);
                if (listener is IGameExitListener exitListener)
                    _exitListeners.Add(exitListener);
            }
        }

        private void NotifyGameInit()
        {
            foreach (var listener in _initListeners)
            {
                listener.GameInit();
            }
        }
        private void NotifyGameLoad()
        {
            foreach (var listener in _loadListeners)
            {
                listener.GameLoad();
            }
        }

        private void NotifyGameStart()
        {
            foreach (var listener in _startListeners)
            {
                listener.GameStart();
            }
        }

        private void NotifyGameTick()
        {
            foreach (var listener in _tickListeners)
            {
                listener.GameTick();
            }
        }
        
        private void NotifyGameExit()
        {
            foreach (var listener in _exitListeners)
            {
                listener.GameExit();
            }
        }
    }

    
}