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
        private readonly List<IGameUpdateListener> _tickListeners = new();
        private readonly List<IGameExitListener> _exitListeners = new();

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
              
                Debugging.Log(this, "[Awake] Test", Debugging.Type.GameState);
            }
            else
            {
                _controller.OnStateChanged += _onWindowControllerStateChanged;
               
                _initializeListeners();
                _notifyGameInit();
                _notifyGameLoad();
                
                Debugging.Log(this, "[Awake]", Debugging.Type.GameState);
            }
        }

        private void Update()
        {
            _notifyGameUpdate();
        }

        private void OnApplicationQuit()
        {
            _notifyGameExit();
            
            Debugging.Log(this, "[OnApplicationQuit]", Debugging.Type.GameState);
        }

        #endregion

        public void InitializeRuntimeListener(IGameListeners listener)
        {
            if (listener is IGameInitListener initListener) initListener.GameInit();
            if (listener is IGameLoadListener loadListener) loadListener.GameLoad();
            if (listener is IGameStartListener startListener) startListener.GameStart();
            if (listener is IGameUpdateListener tickListener) _tickListeners.Add(tickListener);
            if (listener is IGameExitListener exitListener) _exitListeners.Add(exitListener);
        }

        public void RemoveRuntimeListener(IGameListeners listener)
        {
            if (listener is IGameUpdateListener tickListener) _tickListeners.Remove(tickListener);
            if (listener is IGameExitListener exitListener) _exitListeners.Remove(exitListener);
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

            Debugging.Log(this, "[_startWithDelay] Completed", Debugging.Type.GameState);
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
                if (listener is IGameInitListener initListener)
                    _initListeners.Add(initListener);
                if (listener is IGameLoadListener loadListener)
                    _loadListeners.Add(loadListener);
                if (listener is IGameStartListener startListener)
                    _startListeners.Add(startListener);
                if (listener is IGameUpdateListener tickListener)
                    _tickListeners.Add(tickListener);
                if (listener is IGameExitListener exitListener)
                    _exitListeners.Add(exitListener);
            }
        }
        
        private void _notifyGameInit()
        {
            foreach (IGameInitListener listener in _initListeners)
            {
                listener.GameInit();
            }
        }

        private void _notifyGameLoad()
        {
            foreach (IGameLoadListener listener in _loadListeners)
            {
                listener.GameLoad();
            }
        }

        private void _notifyGameStart()
        {
            foreach (IGameStartListener listener in _startListeners)
            {
                listener.GameStart();
            }
        }

        private void _notifyGameUpdate()
        {
            foreach (IGameUpdateListener listener in _tickListeners)
            {
                listener.GameUpdate();
            }
        }

        private void _notifyGameExit()
        {
            foreach (IGameExitListener listener in _exitListeners)
            {
                listener.GameExit();
            }
        }
    }
}