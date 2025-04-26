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
            Log.Info(this, "[run]", Log.Type.BehaviorTree);

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
            Log.Info(this, "[_onHandHidden]", Log.Type.BehaviorTree);

            _divaAnimator.PlayShowHand();
            
            Return(true);
        }
    }
}