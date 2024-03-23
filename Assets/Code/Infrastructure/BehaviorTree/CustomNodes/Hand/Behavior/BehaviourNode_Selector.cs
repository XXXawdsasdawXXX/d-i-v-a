using Code.Components.Character.LiveState;
using Code.Components.Characters;
using Code.Data.Enums;
using Code.Infrastructure.DI;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.CustomNodes.Hand.Behavior
{
    public class BehaviourNode_Selector : BaseNode, IBehaviourCallback
    { 
        [Header("Services")]
        private readonly CharacterLiveStatesAnalytic _stateAnalytic;
        
        [Header("Values")]
        private readonly BaseNode[] _orderedNodes;
        private BaseNode _currentChild;
        private int _currentChildIndex;
        
        public BehaviourNode_Selector()
        {
            _stateAnalytic = Container.Instance.FindEntity<DIVA>().FindCharacterComponent<CharacterLiveStatesAnalytic>();
            
            _orderedNodes = new BaseNode[]
            {
                new BehaviourNode_ShowApple(),
                new BehaviourNode_EnterTheVoid(),
            };
            
            SubscribeToEvents(true);
        }

        ~BehaviourNode_Selector()
        {
            SubscribeToEvents(false);
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
        
                

        #endregion

        #region Break methods

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

        #endregion
        
    }
}