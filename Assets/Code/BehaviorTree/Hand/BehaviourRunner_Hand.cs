using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.BehaviorTree.Hand
{
    public class BehaviourRunner_Hand : MonoBehaviour, IService, IInitializeListener, IStartListener, IUpdateListener, IExitListener
    {
        public bool IsInitBehaviorTree { get; private set; }
        
        [SerializeField] private bool _isRun;
        [SerializeField] private float _runDelaySeconds = 5;
  
        private BaseNode _rootNode;
        private TimeObserver _timeObserver;
        private CoroutineRunner _coroutineRunner;
        
        public UniTask GameInitialize()
        {
            _timeObserver = Container.Instance.GetService<TimeObserver>();
            
            _coroutineRunner = Container.Instance.GetService<CoroutineRunner>();
            
            return UniTask.CompletedTask;
        }

        public UniTask GameStart()
        {
            _subscribeToEvents(true);
            
            return UniTask.CompletedTask;
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