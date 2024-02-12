using Code.Components.Character.LiveState;
using Code.Components.Characters;
using Code.Data.Enums;
using Code.Infrastructure.BehaviorTree.CustomNodes.Sub;
using Code.Infrastructure.DI;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.CustomNodes
{
    public class BehaviorNode_Seat : BaseNode
    {
        [Header("Character")]
        private readonly CharacterAnimator _characterAnimator;
        private readonly CharacterLiveStatesAnalytic _statesAnalytic;
        
        [Header("Services")]

        
        [Header("Sub nodes")] 
        private readonly SubNode_Eat _node_eat;
        
        public BehaviorNode_Seat()
        {
            var character = Container.Instance.FindEntity<Character>();
            //character-------------------------------------------------------------------------------------------------
            _characterAnimator = character.FindCharacterComponent<CharacterAnimator>();
            _statesAnalytic = character.FindCharacterComponent<CharacterLiveStatesAnalytic>();
            //services--------------------------------------------------------------------------------------------------
            
            //nodes-----------------------------------------------------------------------------------------------------
            _node_eat = new SubNode_Eat();
        }

        protected override void Run()
        {
            if (IsCanSeat())
            {
                _characterAnimator.EnterToMode(CharacterAnimationMode.Seat);
                Debugging.Instance.Log($"Нода сидения: выбрано", Debugging.Type.BehaviorTree);
            }
            else
            {
                Debugging.Instance.Log($"Нода сидения: отказ ", Debugging.Type.BehaviorTree);
                Return(false);
            }
        }

        private bool IsCanSeat()
        {
            return _statesAnalytic.TryGetLowerSate(out var key, out var statePercent)
                   && statePercent < 0.4f
                   && key is LiveStateKey.Trust or LiveStateKey.Hunger;
        }
    }
}