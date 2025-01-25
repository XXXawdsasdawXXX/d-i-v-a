using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Data;
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

        private readonly List<IInitListener> _initListeners = new();
        private readonly List<ILoadListener> _loadListeners = new();
        private readonly List<IStartListener> _startListeners = new();
        private readonly List<IUpdateListener> _tickListeners = new();
        private readonly List<IExitListener> _exitListeners = new();

        #region runtime

        private void Awake()
        {
#if !UNITY_EDITOR
            _isTestInit = false;
#endif
            _controller = Container.Instance.GetUniWindowController();
            if (_isTestInit)
            {
                _controller.gameObject.SetActive(false);
                
                _initializeListeners();
                _notifyGameInit();
                _notifyGameLoad();

                StartCoroutine(_startWithDelay());

#if DEBUGGING
                Debugging.Log(this, "[Awake] Test", Debugging.Type.GameState);
#endif
            }
            else
            {
                _controller.OnStateChanged += _onWindowControllerStateChanged;

                _initializeListeners();
                _notifyGameInit();
                _notifyGameLoad();

#if DEBUGGING
                Debugging.Log(this, "[Awake]", Debugging.Type.GameState);
#endif
            }
        }

        private void Update()
        {
            _notifyGameUpdate();
        }

        private void OnApplicationQuit()
        {
            _notifyGameExit();

#if DEBUGGING
            Debugging.Log(this, "[OnApplicationQuit]", Debugging.Type.GameState);
#endif
        }

        #endregion

        public void InitializeRuntimeListener(IGameListeners listener)
        {
            if (listener is IInitListener initListener) initListener.GameInitialize();
            if (listener is ILoadListener loadListener) loadListener.GameLoad();
            if (listener is IStartListener startListener) startListener.GameStart();
            if (listener is IUpdateListener tickListener) _tickListeners.Add(tickListener);
            if (listener is IExitListener exitListener) _exitListeners.Add(exitListener);
        }

        public void RemoveRuntimeListener(IGameListeners listener)
        {
            if (listener is IUpdateListener tickListener) _tickListeners.Remove(tickListener);
            if (listener is IExitListener exitListener) _exitListeners.Remove(exitListener);
        }

        private void _onWindowControllerStateChanged(UniWindowController.WindowStateEventType type)
        {
            StartCoroutine(_startWithDelay());
        }

        private IEnumerator _startWithDelay()
        {
            _controller.OnStateChanged -= _onWindowControllerStateChanged;

            yield return new WaitForSeconds(1);

            _notifyGameStart();

#if DEBUGGING
            Debugging.Log(this, "[_startWithDelay] Completed", Debugging.Type.GameState);
#endif
        }

        private void _initializeListeners()
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
                if (listener is IInitListener initListener)
                    _initListeners.Add(initListener);
                if (listener is ILoadListener loadListener)
                    _loadListeners.Add(loadListener);
                if (listener is IStartListener startListener)
                    _startListeners.Add(startListener);
                if (listener is IUpdateListener tickListener)
                    _tickListeners.Add(tickListener);
                if (listener is IExitListener exitListener)
                    _exitListeners.Add(exitListener);
            }
        }

        private void _notifyGameInit()
        {
            foreach (IInitListener listener in _initListeners)
            {
                listener.GameInitialize();
            }
        }

        private void _notifyGameLoad()
        {
            foreach (ILoadListener listener in _loadListeners)
            {
                listener.GameLoad();
            }
        }

        private void _notifyGameStart()
        {
            foreach (IStartListener listener in _startListeners)
            {
                listener.GameStart();
            }
        }

        private void _notifyGameUpdate()
        {
            foreach (IUpdateListener listener in _tickListeners)
            {
                listener.GameUpdate();
            }
        }

        private void _notifyGameExit()
        {
            foreach (IExitListener listener in _exitListeners)
            {
                listener.GameExit();
            }
        }
    }
}