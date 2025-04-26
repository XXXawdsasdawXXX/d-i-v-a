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
                Log.Info(this, "[run]", Log.Type.BehaviorTree);

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

            Log.Info(this, "[break]", Log.Type.BehaviorTree);
        }

        protected override void OnReturn(bool success)
        {
            _mouseReaction.StopReaction();
            
            Log.Info(this, $"[on Return] {success}", Log.Type.BehaviorTree);

            base.OnReturn(success);
        }

        void IBehaviourCallback.InvokeCallback(BaseNode node, bool success)
        {
            Log.Info(this, "[InvokeCallback]", Log.Type.BehaviorTree);

            _mouseReaction.StopReaction();
        
            Return(true);
        }
    }
}