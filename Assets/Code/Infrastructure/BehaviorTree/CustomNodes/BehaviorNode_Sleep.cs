using Code.Components.Character;
using Code.Components.Character.LiveState;
using Code.Data.Enums;
using Code.Data.Value.RangeFloat;
using Code.Infrastructure.BehaviorTree.BaseNodes;
using Code.Infrastructure.DI;
using Code.Services;
using Code.Utils;

namespace Code.Infrastructure.BehaviorTree.CustomNodes
{
    public class BehaviorNode_Sleep : BehaviourNode, IBehaviourCallback
    {
        private readonly CharacterLiveStatesAnalytics _characterLiveStateAnalytics;
        private readonly Character _character;
        private readonly TimeObserver _timeObserver;
        private readonly BehaviourNode_RandomSequence _randomSequence;


        public BehaviorNode_Sleep()
        {
            _character = Container.Instance.GetCharacter();

            _timeObserver = Container.Instance.FindService<TimeObserver>();
            _characterLiveStateAnalytics = Container.Instance.FindLiveStateLogic<CharacterLiveStatesAnalytics>();
            
            _randomSequence = new BehaviourNode_RandomSequence(new BehaviourNode[]
            {
                new BehaviourNode_WaitForSeconds(new RangedFloat()
                {
                    MinValue = 60 * 1,
                    MaxValue = 60 * 5
                }),
            });
        }

        protected override void OnBreak()
        {
            _randomSequence.Break();
            base.OnBreak();
        }

        protected override void Run()
        {
            if (_timeObserver.IsNightTime())
            {
                if (_characterLiveStateAnalytics.GetLowerSate(out LiveStateKey key, out float percent))
                {
                    if (key == LiveStateKey.Fear && percent < 0.3f)
                    {
                        Debugging.Instance.Log($"Нода сна: выбран минимальный страх ",Debugging.Type.BehaviorTree);

                        _character.Animator.EnterToMode(CharacterAnimationMode.Seat);
                        //Show Spikes (шипы)
                        //Show Wound  (раны)
                        Return(true);
                        return;
                    }
                    else if (key == LiveStateKey.Hunger && percent < 0.3f)
                    {
                        Debugging.Instance.Log($"Нода сна: выбран минимальный голод ",Debugging.Type.BehaviorTree);

                        _character.Animator.EnterToMode(CharacterAnimationMode.Seat);
                        //Show Spikes (шипы)
                        //Show Wound  (раны)
                        Return(true);
                        return;
                    }
                }

                Debugging.Instance.Log($"Нода сна: выбран сон ",Debugging.Type.BehaviorTree);
                _character.Animator.EnterToMode(CharacterAnimationMode.Sleep);
                _randomSequence.Run(this);
            }
            else
            {
                Debugging.Instance.Log($"Нода сна: отказ запуска -> не ночное время ");
                Return(false);
            }

        }

        void IBehaviourCallback.Invoke(BehaviourNode node, bool success)
        {
                Return(true);
            
        }
    }
}