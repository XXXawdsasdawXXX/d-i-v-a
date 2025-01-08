using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.Diva
{
    public sealed class BehaviourRunner_Character : MonoBehaviour, IService, IGameInitListener, IGameUpdateListener,
        IGameExitListener
    {
        [SerializeField] private bool _isRun;

        private BaseNode _rootNode;
        private TimeObserver _timeObserver;

        public bool IsInitBehaviorTree { get; private set; }

        public void GameInit()
        {
            _timeObserver = Container.Instance.FindService<TimeObserver>();
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
            _rootNode = new BehaviourSelector_Character();
            IsInitBehaviorTree = true;
        }
    }
}