using Code.Infrastructure.DI;
using Code.Infrastructure.Reactions;
using Code.Utils;

namespace Code.Infrastructure.BehaviorTree.Diva
{
    public class SubNode_ReactionToVoice : BaseNode
    {
        private readonly AudioReaction _audioReaction;

        public SubNode_ReactionToVoice()
        {
            _audioReaction = Container.Instance.FindReaction<AudioReaction>();

            _audioReaction.EndReactionEvent += _onEndReaction;
        }

        protected override void Run()
        {
            if (IsCanRun())
            {
#if DEBUGGING
                Debugging.Log(this, $"[Run]", Debugging.Type.BehaviorTree);
#endif
                _audioReaction.StartReaction();
            }
            else
            {
#if DEBUGGING
                Debugging.Log(this, $"[Run] Is not ready.", Debugging.Type.BehaviorTree);
#endif
                Return(false);
            }
        }

        protected override bool IsCanRun()
        {
            return _audioReaction.IsReady();
        }

        private void _onEndReaction()
        {
            Return(true);
        }
    }
}