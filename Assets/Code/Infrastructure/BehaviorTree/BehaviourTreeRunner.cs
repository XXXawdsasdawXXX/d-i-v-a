using Code.Data.Interfaces;
using Code.Infrastructure.BehaviorTree.BaseNodes;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree
{
    public sealed class BehaviourTreeRunner : MonoBehaviour, IService, IGameInitListener , IGameTickListener,IGameStartListener
    {
        [SerializeField] private bool _isRun;
        
        private BaseNode _rootNode;
        private TimeObserver _timeObserver;

        public bool IsInitBehaviorTree { get; private set; }

        public void GameInit()
        {
            _timeObserver = Container.Instance.FindService<TimeObserver>();
        }

        public void GameStart()
        {
            _rootNode = new BehaviourNode_Selector();
            IsInitBehaviorTree = true;
        }


        public void GameTick()
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


    }
}