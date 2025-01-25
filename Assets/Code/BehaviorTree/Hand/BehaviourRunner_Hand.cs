using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using UnityEngine;

namespace Code.BehaviorTree.Hand
{
    public class BehaviourRunner_Hand : MonoBehaviour, IService, IInitListener, IUpdateListener, IExitListener
    {
        public bool IsInitBehaviorTree { get; private set; }
        
        [SerializeField] private bool _isRun;
        [SerializeField] private float _runDelaySeconds = 5;
  
        private BaseNode _rootNode;
        private TimeObserver _timeObserver;
        private CoroutineRunner _coroutineRunner;


        public void GameInitialize()
        {
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();
            _subscribeToEvents(true);
        }

        public void GameUpdate()
        {
            if (!_isRun)
            {
                return;
            }

            if (_rootNode is { IsRunning: false })
            {
                _rootNode.Run(null);
            }
        }

        public void GameExit()
        {
            _subscribeToEvents(false);
        }

        private void _subscribeToEvents(bool flag)
        {
            if (flag)
            {
                _timeObserver.OnTimeInitialized += _onTimeInitialized;
            }
            else
            {
                _timeObserver.OnTimeInitialized -= _onTimeInitialized;
            }
        }

        private void _onTimeInitialized(bool obj)
        {
            _coroutineRunner.StartActionWithDelay(() =>
            {
                _rootNode = new BehaviourSelector_Hand();
                IsInitBehaviorTree = true;
            }, _runDelaySeconds);
        }
    }
}