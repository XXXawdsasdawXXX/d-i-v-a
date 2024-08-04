using Code.Components.Characters;
using Code.Components.Entities;
using Code.Data.Enums;
using Code.Infrastructure.BehaviorTree.BaseNodes;
using Code.Infrastructure.DI;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.CustomNodes.Character.Behavior
{
    public sealed class BehaviourSelector_Character : BaseNode, IBehaviourCallback
    {
        [Header("Services")]
        private readonly CharacterLiveStatesAnalytic _stateAnalytic;
        
        [Header("Values")]
        private readonly BaseNode[] _orderedNodes;
        private BaseNode _currentChild;
        private int _currentChildIndex;

        public BehaviourSelector_Character()
        {
            _stateAnalytic = Container.Instance.FindEntity<DIVA>().FindCharacterComponent<CharacterLiveStatesAnalytic>();
            
            _orderedNodes = new BaseNode[]
            {
                new BehaviourNode_Sleep(),
                new BehaviourNode_Seat(),
                new BehaviourNode_Stand(),
            };
            
            SubscribeToEvents(true);
        }

        ~BehaviourSelector_Character()
        {
            SubscribeToEvents(false);
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

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _stateAnalytic.SwitchLowerStateKeyEvent += OnSwitchLowerLiveState;
            }
            else
            {
                _stateAnalytic.SwitchLowerStateKeyEvent -= OnSwitchLowerLiveState;
            }
        }

        private void OnSwitchLowerLiveState(LiveStateKey key)
        {
            Debugging.Instance.Log($"Селектор: среагировать на изменение нижнего показателя", Debugging.Type.BehaviorTree);
            _currentChild?.Break();
            Run();
        }
    }
}