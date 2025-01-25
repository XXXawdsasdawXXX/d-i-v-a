using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using UnityEngine;

namespace Code.BehaviorTree.Diva
{
    public sealed class BehaviourRunner_Character : MonoBehaviour, IService, IInitListener, IUpdateListener,
        IExitListener
    {
        [SerializeField] private bool _isRun;

        private BaseNode _rootNode;
        private TimeObserver _timeObserver;

        public bool IsInitBehaviorTree { get; private set; }

        public void GameInitialize()
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
                _timeObserver.OnTimeInitialized += _onTimeInitialized;
            }
            else
            {
                _timeObserver.OnTimeInitialized -= _onTimeInitialized;
            }
        }

        private void _onTimeInitialized(bool obj)
        {
            _rootNode = new BehaviourSelector_Character();
            IsInitBehaviorTree = true;
        }
    }
}