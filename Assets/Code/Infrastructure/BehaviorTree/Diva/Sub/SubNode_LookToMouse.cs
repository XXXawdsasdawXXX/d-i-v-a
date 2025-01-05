using Code.Data.Configs;
using Code.Infrastructure.DI;
using Code.Infrastructure.Reactions;
using Code.Utils;

namespace Code.Infrastructure.BehaviorTree.Diva
{
    public class SubNode_LookToMouse : BaseNode, IBehaviourCallback
    {
        private readonly SubNode_WaitForTicks _waitFor;

        private readonly MouseReaction _mouseReaction;

        public SubNode_LookToMouse()
        {
            _waitFor = new SubNode_WaitForTicks(Container.Instance.FindConfig<TimeConfig>().Duration.LookToMouse);
            _mouseReaction = Container.Instance.FindReaction<MouseReaction>();
        }

        protected override void Run()
        {
            if (IsCanRun())
            {
                Debugging.Instance.Log($"Саб нода смотреть за курсором: выбрано", Debugging.Type.BehaviorTree);
                _mouseReaction.StartReaction();
                _waitFor.Run(this);
                return;
            }

            Return(false);
        }

        protected override bool IsCanRun()
        {
            return _mouseReaction.IsReady();
        }

        protected override void OnBreak()
        {
            _waitFor.Break();
            _mouseReaction.StopReaction();
            Debugging.Instance.Log($"Саб нода смотреть за курсором: брейк", Debugging.Type.BehaviorTree);
        }

        protected override void OnReturn(bool success)
        {
            _mouseReaction.StopReaction();
            Debugging.Instance.Log($"Саб нода смотреть за курсором: ретерн {success}", Debugging.Type.BehaviorTree);
            base.OnReturn(success);
        }
        
        void IBehaviourCallback.InvokeCallback(BaseNode node, bool success)
        {
            Debugging.Instance.Log($"Нода смотреть за курсором: ожидание закончилось колбэк",
                Debugging.Type.BehaviorTree);
            _mouseReaction.StopReaction();
            Return(true);
        }
    }
}