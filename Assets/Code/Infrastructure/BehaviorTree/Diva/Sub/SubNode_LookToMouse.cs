using Code.Data;
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
#if DEBUGGING
                Debugging.Log(this, "[run]", Debugging.Type.BehaviorTree);
#endif

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

#if DEBUGGING
            Debugging.Log(this, "[break]", Debugging.Type.BehaviorTree);
#endif
        }

        protected override void OnReturn(bool success)
        {
            _mouseReaction.StopReaction();

#if DEBUGGING
            Debugging.Log(this, $"[on Return] {success}", Debugging.Type.BehaviorTree);
#endif

            base.OnReturn(success);
        }

        void IBehaviourCallback.InvokeCallback(BaseNode node, bool success)
        {
#if DEBUGGING
            Debugging.Log(this, "[InvokeCallback]", Debugging.Type.BehaviorTree);
#endif
        
            _mouseReaction.StopReaction();
        
            Return(true);
        }
    }
}