using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Data.Interfaces;
using Code.Infrastructure.DI;
using Code.Utils;
using Kirurobo;
using UnityEngine;

namespace Code.Infrastructure.GameLoop
{
    public class GameEventDispatcher : MonoBehaviour, IService
    {
        [SerializeField] private bool _isTestInit;
        private UniWindowController _controller;

        private readonly List<IGameInitListener> _initListeners = new();
        private readonly List<IGameLoadListener> _loadListeners = new();
        private readonly List<IGameStartListener> _startListeners = new();
        private readonly List<IGameTickListener> _tickListeners = new();
        private readonly List<IGameExitListener> _exitListeners = new();

        public void Awake()
        {
#if !UNITY_EDITOR
            _isTestInit = false;
#endif
            _controller = Container.Instance.GetUniWindowController();
            if (_isTestInit)
            {
                _controller.gameObject.SetActive(false);
                InitializeListeners();
                NotifyGameInit();
                NotifyGameLoad();
                Debugging.Instance.Log("Awake", Debugging.Type.GameState);
                StartCoroutine(StartWithDelay());
                Debugging.Instance.Log("Start", Debugging.Type.GameState);
            }
            else
            {
                _controller.OnStateChanged += OnWindowControllerStateChanged;
                Debugging.Instance.Log("Subscribe", Debugging.Type.GameState);
                InitializeListeners();
                NotifyGameInit();
                NotifyGameLoad();
                Debugging.Instance.Log("Awake", Debugging.Type.GameState);
            }
        }

        private void OnWindowControllerStateChanged(UniWindowController.WindowStateEventType type)
        {
            StartCoroutine(StartWithDelay());
        }

        private IEnumerator StartWithDelay()
        {
            _controller.OnStateChanged -= OnWindowControllerStateChanged;
            yield return new WaitForSeconds(1);

            NotifyGameStart();
            Debugging.Instance.Log("Start", Debugging.Type.GameState);
        }

        private void Update()
        {
            NotifyGameTick();
        }

        private void OnApplicationQuit()
        {
            NotifyGameExit();
            Debugging.Instance.Log("Exit", Debugging.Type.GameState);
        }

        private void InitializeListeners()
        {
            List<IGameListeners> gameListeners = Container.Instance.GetGameListeners();
            if (Extensions.IsMacOs())
            {
                gameListeners = gameListeners
                    .Where(l => l is not IWindowsSpecific)
                    .ToList();
            }

            foreach (IGameListeners listener in gameListeners)
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

        public void InitializeRuntimeListener(IGameListeners listener)
        {
            if (listener is IGameInitListener initListener) initListener.GameInit();
            if (listener is IGameLoadListener loadListener) loadListener.GameLoad();
            if (listener is IGameStartListener startListener) startListener.GameStart();
            if (listener is IGameTickListener tickListener) _tickListeners.Add(tickListener);
            if (listener is IGameExitListener exitListener) _exitListeners.Add(exitListener);
        }
        
        public void RemoveRuntimeListener(IGameListeners listener)
        {
            if (listener is IGameTickListener tickListener) _tickListeners.Remove(tickListener);
            if (listener is IGameExitListener exitListener) _exitListeners.Remove(exitListener);
        }
        
        private void NotifyGameInit()
        {
            foreach (IGameInitListener listener in _initListeners)
            {
                listener.GameInit();
            }
        }

        private void NotifyGameLoad()
        {
            foreach (IGameLoadListener listener in _loadListeners)
            {
                listener.GameLoad();
            }
        }

        private void NotifyGameStart()
        {
            foreach (IGameStartListener listener in _startListeners)
            {
                listener.GameStart();
            }
        }

        private void NotifyGameTick()
        {
            foreach (IGameTickListener listener in _tickListeners)
            {
                listener.GameTick();
            }
        }

        private void NotifyGameExit()
        {
            foreach (IGameExitListener listener in _exitListeners)
            {
                listener.GameExit();
            }
        }
    }
}