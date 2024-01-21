using Code.Components.Character;
using Code.Components.Character.LiveState;
using Code.Data.Enums;
using Code.Infrastructure.DI;
using Code.Services;
using Code.Utils;

namespace Code.Infrastructure.BehaviorTree.CustomNodes
{
    public class BehaviorNode_Seat : BehaviourNode
    {
        private readonly Character _character;
        private readonly TimeObserver _timeObserver;
        private readonly CharacterLiveStatesAnalytics _characterLiveStateAnalytics;

        public BehaviorNode_Seat()
        {
            _character = Container.Instance.GetCharacter();

            _timeObserver = Container.Instance.FindService<TimeObserver>();
            _characterLiveStateAnalytics = Container.Instance.FindLiveStateLogic<CharacterLiveStatesAnalytics>();
        }

        protected override void Run()
        {
            if (_characterLiveStateAnalytics.TryGetLowerSate(out var key, out var statePercent) && statePercent < 0.4f)
            {
                if (key is LiveStateKey.Trust or LiveStateKey.Hunger)
                {
                    _character.Animator.EnterToMode(CharacterAnimationMode.Seat);
                    Debugging.Instance.Log($"Нода сидения: выбрано",Debugging.Type.BehaviorTree);
                    Return(true);
                    return;
                }
            }
            Debugging.Instance.Log($"Нода сидения: отказ ",Debugging.Type.BehaviorTree);
            Return(false);
        }
    }
}