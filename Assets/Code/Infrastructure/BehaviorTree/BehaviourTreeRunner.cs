using Code.Components.Character.LiveState;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Infrastructure.BehaviorTree.BaseNodes;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;

namespace Code.Infrastructure.BehaviorTree
{
    public sealed class BehaviourTreeRunner : IService, IGameInitListener , IGameTickListener, IGameExitListener
    {
        private CharacterLiveStatesAnalytics _statesAnalytics;
        private BehaviourNode _rootNode;
        private TimeObserver _timeObserver;

        public void GameInit()
        {
            _statesAnalytics = Container.Instance.FindLiveStateLogic<CharacterLiveStatesAnalytics>();
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            
            _timeObserver.InitTimeEvent += OnInitTime;
            _statesAnalytics.SwitchLowerStateKeyEvent += OnVariableChanged;
        }

        public void GameTick()
        {
            /*if (_rootNode is { IsRunning: false })
            {
                _rootNode.Run(null);
            }*/
        }

        public void GameExit()
        {
            _timeObserver.InitTimeEvent -= OnInitTime;
            _statesAnalytics.SwitchLowerStateKeyEvent -= OnVariableChanged;
        }


        private void OnInitTime()
        {
            _rootNode = new BehaviourSelector();
        }

        private void OnVariableChanged(LiveStateKey key)
        {
            _rootNode?.Break();
        }
    }
}