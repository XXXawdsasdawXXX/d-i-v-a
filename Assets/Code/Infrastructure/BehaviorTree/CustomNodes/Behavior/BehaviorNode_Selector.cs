using Code.Components.Character.LiveState;
using Code.Components.Characters;
using Code.Data.Enums;
using Code.Infrastructure.BehaviorTree.CustomNodes;
using Code.Infrastructure.DI;
using Code.Utils;

namespace Code.Infrastructure.BehaviorTree.BaseNodes
{
    public sealed class BehaviorNode_Selector : BaseNode, IBehaviourCallback
    {
        private readonly BaseNode[] _orderedNodes;
        private BaseNode _currentChild;

        private int _currentChildIndex;
        private readonly LiveStatesAnalytics _stateAnalytics;

        public BehaviorNode_Selector()
        {
            _stateAnalytics = Container.Instance.FindEntity<Character>().StatesAnalytics;
            
            _orderedNodes = new BaseNode[]
            {
                new BehaviorNode_Sleep(),
                new BehaviorNode_Seat(),
                new BehaviorNode_Stand(),
            };
            
            SubscribeToEvents(true);
        }

        ~BehaviorNode_Selector()
        {
            SubscribeToEvents(false);
        }
        
        protected override void Run()
        {
            if (_orderedNodes == null || _orderedNodes.Length <= 0)
            {
                Return(false);
                return;
            }

            _currentChildIndex = 0;
            _currentChild = _orderedNodes[_currentChildIndex];
            _currentChild.Run(callback: this);
        }

        void IBehaviourCallback.InvokeCallback(BaseNode node, bool success)
        {
            if (success)
            {
                Return(true);
                return;
            }

            if (_currentChildIndex + 1 >= _orderedNodes.Length)
            {
                Return(false);
                return;
            }

            _currentChildIndex++;
            _currentChild = _orderedNodes[_currentChildIndex];
            _currentChild.Run(callback: this);
        }

        protected override void OnBreak()
        {
            if (_currentChild != null && _currentChild.IsRunning)
            {
                _currentChild.Break();
                _currentChild = null;
            }
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _stateAnalytics.SwitchLowerStateKeyEvent += OnSwitchLowerLiveState;
            }
            else
            {
                _stateAnalytics.SwitchLowerStateKeyEvent -= OnSwitchLowerLiveState;
            }
        }

        private void OnSwitchLowerLiveState(LiveStateKey key)
        {
            Debugging.Instance.Log($"Селектор: среагировать на изменение нижнего показателя ", Debugging.Type.BehaviorTree);
            _currentChild?.Break();
            Run();
        }
    }
}