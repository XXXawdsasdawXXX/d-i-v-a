using Code.Data;
using Code.Entities.Diva.Reactions;
using Code.Infrastructure.DI;
using Code.Utils;

namespace Code.BehaviorTree.Diva
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
                Log.Info(this, "[run]", Log.Type.BehaviorTree);
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
            Log.Info(this, "[break]", Log.Type.BehaviorTree);
#endif
        }

        protected override void OnReturn(bool success)
        {
            _mouseReaction.StopReaction();

#if DEBUGGING
            Log.Info(this, $"[on Return] {success}", Log.Type.BehaviorTree);
#endif

            base.OnReturn(success);
        }

        void IBehaviourCallback.InvokeCallback(BaseNode node, bool success)
        {
#if DEBUGGING
            Log.Info(this, "[InvokeCallback]", Log.Type.BehaviorTree);
#endif
        
            _mouseReaction.StopReaction();
        
            Return(true);
        }
    }
}