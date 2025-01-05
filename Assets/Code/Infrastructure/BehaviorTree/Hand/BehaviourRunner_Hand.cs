using Code.Data.Interfaces;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.Hand
{
    public class BehaviourRunner_Hand : MonoBehaviour, IService, IGameInitListener, IGameUpdateListener,
        IGameExitListener
    {
        [SerializeField] private bool _isRun;
        [SerializeField] private float _runDelaySeconds = 5;
        private BaseNode _rootNode;
        private TimeObserver _timeObserver;
        private CoroutineRunner _coroutineRunner;

        public bool IsInitBehaviorTree { get; private set; }

        public void GameInit()
        {
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();
            SubscribeToEvents(true);
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
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _timeObserver.InitTimeEvent += TimeObserverOnInitTimeEvent;
            }
            else
            {
                _timeObserver.InitTimeEvent -= TimeObserverOnInitTimeEvent;
            }
        }

        private void TimeObserverOnInitTimeEvent(bool obj)
        {
            _coroutineRunner.StartActionWithDelay(() =>
            {
                _rootNode = new BehaviourSelector_Hand();
                IsInitBehaviorTree = true;
            }, _runDelaySeconds);
        }
    }
}