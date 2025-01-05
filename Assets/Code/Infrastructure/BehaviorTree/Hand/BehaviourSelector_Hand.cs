using Code.Components.Entities;
using Code.Data.Enums;
using Code.Infrastructure.DI;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.Hand
{
    public class BehaviourSelector_Hand : BaseNode, IBehaviourCallback
    {
        [Header("Services")]
        private readonly DivaLiveStatesAnalytic _stateAnalytic;

        [Header("Values")] 
        private readonly BaseNode[] _orderedNodes;
        private BaseNode _currentChild;
        private int _currentChildIndex;

        public BehaviourSelector_Hand()
        {
            _stateAnalytic = Container.Instance.FindEntity<Components.Entities.Diva>()
                .FindCharacterComponent<DivaLiveStatesAnalytic>();

            _orderedNodes = new BaseNode[]
            {
                new BehaviourNode_WaitCharacterWakeUp(),
                new BehaviourNode_WaitTick(),
                new BehaviourNode_ShowItem(),
            };

            _subscribeToEvents(true);
        }

        ~BehaviourSelector_Hand()
        {
            _subscribeToEvents(false);
        }

        #region Base methods

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

        protected override bool IsCanRun()
        {
            return _orderedNodes is { Length: > 0 };
        }

        void IBehaviourCallback.InvokeCallback(BaseNode node, bool success)
        {
            /*
            if (success)
            {
                Return(true);
                return;
            }
            */

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
            if (_currentChild is { IsRunning: true })
            {
                _currentChild.Break();
                _currentChild = null;
            }
        }

        #endregion

        #region Break methods

        private void _subscribeToEvents(bool flag)
        {
            if (flag)
            {
                _stateAnalytic.SwitchLowerStateKeyEvent += _onSwitchLowerLiveState;
            }
            else
            {
                _stateAnalytic.SwitchLowerStateKeyEvent -= _onSwitchLowerLiveState;
            }
        }

        private void _onSwitchLowerLiveState(ELiveStateKey key)
        {
            Debugging.Instance.Log(this, $"_onSwitchLowerLiveState {key}", Debugging.Type.Hand);
            
            _currentChild?.Break();
            
            Run();
        }

        #endregion
    }
}