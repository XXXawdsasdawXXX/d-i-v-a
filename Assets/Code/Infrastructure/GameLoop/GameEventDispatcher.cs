using System.Collections.Generic;
using Code.Infrastructure.DI;
using UnityEngine;

namespace Code.Infrastructure.GameLoop
{
    public class GameEventDispatcher : MonoBehaviour
    {
        private readonly List<IGameLoadListener> _loadListeners = new();
        private readonly List<IGameStartListener> _startListeners = new();
        private readonly List<IGameTickListener> _tickListeners = new();
        private readonly List<IGameExitListener> _exitListeners = new();
        private readonly List<IGameSaveListener> _saveListeners = new();

        public void Awake()
        {
            InitializeListeners();
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
            NotifyGameLoad();
            NotifyGameExit();
        }

        private void InitializeListeners()
        {
            var gameListeners = Container.Instance.GetGameListeners();

            foreach (var listener in gameListeners)
            {
                if (listener is IGameLoadListener loadListener)
                    _loadListeners.Add(loadListener);
                if (listener is IGameStartListener startListener)
                    _startListeners.Add(startListener);
                if (listener is IGameTickListener tickListener)
                    _tickListeners.Add(tickListener);
                if (listener is IGameSaveListener saveListener)
                    _saveListeners.Add(saveListener);
                if (listener is IGameExitListener exitListener)
                    _exitListeners.Add(exitListener);
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

        private void NotifyGameSave()
        {
            foreach (var listener in _saveListeners)
            {
                listener.GameSave();
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