using Code.Entities.Diva;
using Code.Entities.Hand;
using Code.Infrastructure.DI;
using Code.Utils;

namespace Code.BehaviorTree.Diva
{
    public class SubNode_HideHand : BaseNode
    {
        private readonly DivaAnimator _divaAnimator;

        public SubNode_HideHand()
        {
            _divaAnimator = Container.Instance.FindEntity<DivaEntity>().FindCharacterComponent<DivaAnimator>();
            
            HandBehaviorEvents handBehaviorEvents = Container.Instance.FindEntity<HandEntity>()
                .FindHandComponent<HandBehaviorEvents>();

            handBehaviorEvents.OnHidden += _onHandHidden;
        }

        protected override void Run()
        {
#if DEBUGGING
            Debugging.Log(this, "[run]", Debugging.Type.BehaviorTree);
#endif

            _divaAnimator.PlayHideHand();
        }

        protected override bool IsCanRun()
        {
            return true;
        }

        protected override void OnBreak()
        {
            _divaAnimator.PlayShowHand();

            base.OnBreak();
        }

        private void _onHandHidden()
        {
#if DEBUGGING
            Debugging.Log(this, "[_onHandHidden]", Debugging.Type.BehaviorTree);
#endif
            
            _divaAnimator.PlayShowHand();
            
            Return(true);
        }
    }
}