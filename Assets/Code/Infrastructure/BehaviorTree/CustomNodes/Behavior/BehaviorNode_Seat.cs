using Code.Components.Character;
using Code.Components.Character.LiveState;
using Code.Components.Characters;
using Code.Data.Enums;
using Code.Infrastructure.DI;
using Code.Services;
using Code.Utils;

namespace Code.Infrastructure.BehaviorTree.CustomNodes
{
    public class BehaviorNode_Seat : BaseNode
    {
        private readonly CharacterAnimator _characterAnimator;
        private readonly LiveStatesAnalytics _statesAnalytics;

        public BehaviorNode_Seat()
        {
            var character = Container.Instance.FindEntity<Character>();
            _characterAnimator = character.Animator;
            _statesAnalytics = character.StatesAnalytics;
        }

        protected override void Run()
        {
            if (IsCanSeat())
            {
                _characterAnimator.EnterToMode(CharacterAnimationMode.Seat);
                Debugging.Instance.Log($"Нода сидения: выбрано", Debugging.Type.BehaviorTree);

                return;
            }

            Debugging.Instance.Log($"Нода сидения: отказ ", Debugging.Type.BehaviorTree);
            Return(false);
        }

        private bool IsCanSeat()
        {
            return _statesAnalytics.TryGetLowerSate(out var key, out var statePercent)
                   && statePercent < 0.4f
                   && key is LiveStateKey.Trust or LiveStateKey.Hunger;
        }
    }
}