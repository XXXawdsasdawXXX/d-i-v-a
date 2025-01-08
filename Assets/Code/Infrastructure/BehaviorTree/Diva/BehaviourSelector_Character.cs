using Code.Data;
using Code.Entities.Diva;
using Code.Infrastructure.DI;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.Diva
{
    public sealed class BehaviourSelector_Character : BaseNode, IBehaviourCallback
    {
        [Header("Services")]
        private readonly DivaLiveStatesAnalytic _stateAnalytic;

        [Header("Values")] 
        private readonly BaseNode[] _orderedNodes;
        private BaseNode _currentChild;
        private int _currentChildIndex;

        public BehaviourSelector_Character()
        {
            _stateAnalytic = Container.Instance.FindEntity<DivaEntity>()
                .FindCharacterComponent<DivaLiveStatesAnalytic>();

            _orderedNodes = new BaseNode[]
            {
                new BehaviourNode_Sleep(),
                new BehaviourNode_Seat(),
                new BehaviourNode_Stand(),
            };

            _subscribeToEvents(true);
        }

        ~BehaviourSelector_Character()
        {
            _subscribeToEvents(false);
        }

        protected override void Run()
        {
            if (IsCanRun())
            {
                _currentChildIndex = 0;
                _currentChild = _orderedNodes[_currentChildIndex];
                _currentChild.Run(callback: this);
                return;
            }

            Return(false);
        }

        protected override bool IsCanRun()
        {
            return _orderedNodes is { Length: > 0 };
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
            if (_currentChild is { IsRunning: true })
            {
                _currentChild.Break();
                _currentChild = null;
            }
        }

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
#if DEBUGGING
            Debugging.Log(this, $"[_onSwitchLowerLiveState] -> _child?.Break();.", Debugging.Type.BehaviorTree);
#endif
            _currentChild?.Break();
            
            Run();
        }
    }
}