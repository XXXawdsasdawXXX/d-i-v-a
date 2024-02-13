using Code.Components.Characters;
using Code.Data.Value.RangeFloat;
using Code.Infrastructure.BehaviorTree.BaseNodes;
using Code.Infrastructure.DI;
using Code.Utils;

namespace Code.Infrastructure.BehaviorTree.CustomNodes
{
    public class SubNode_LookToMouse : BaseNode, IBehaviourCallback
    {
        private readonly SubNode_WaitForSeconds _waitFor;
        
        private readonly CharacterMouseReaction _mouseReaction;

        public SubNode_LookToMouse()
        {
            _waitFor = new SubNode_WaitForSeconds(new RangedFloat()
            {
                MinValue = 60 * 0.5f,
                MaxValue = 60 * 0.75f 
            });
            _mouseReaction = Container.Instance.FindEntity<Character>().FindReaction<CharacterMouseReaction>();
        }

        protected override void Run()
        {
            if (!IsReady())
            {
                Return(false);
                return;
            }
        
            Debugging.Instance.Log($"Саб нода смотреть за курсором: выбрано", Debugging.Type.BehaviorTree);
            _mouseReaction.StartReaction();
            _waitFor.Run(this);
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

        private bool IsReady()
        {
            return _mouseReaction.IsReady();
        }

        void IBehaviourCallback.InvokeCallback(BaseNode node, bool success)
        {
            Debugging.Instance.Log($"Нода смотреть за курсором: ожидание закончилось колбэк", Debugging.Type.BehaviorTree);
            _mouseReaction.StopReaction();
            Return(true);
        }
    }
}