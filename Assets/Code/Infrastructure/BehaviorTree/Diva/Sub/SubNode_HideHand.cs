using Code.Entities.Diva;
using Code.Entities.Hand;
using Code.Infrastructure.DI;
using Code.Utils;

namespace Code.Infrastructure.BehaviorTree.Diva
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
            Debugging.Log(this, "[Run]", Debugging.Type.BehaviorTree);
            
            _divaAnimator.PlayHideHand();
        }

        protected override bool IsCanRun()
        {
            return true;
        }

        private void _onHandHidden()
        {
            Debugging.Log(this, "[_onHandHidden]", Debugging.Type.BehaviorTree);
            
            _divaAnimator.PlayShowHand();
            
            Return(true);
        }
    }
}