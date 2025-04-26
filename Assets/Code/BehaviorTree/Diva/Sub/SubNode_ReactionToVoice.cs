using Code.Entities.Diva.Reactions;
using Code.Infrastructure.DI;
using Code.Utils;

namespace Code.BehaviorTree.Diva
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
                Log.Info(this, $"[Run]", Log.Type.BehaviorTree);
#endif
                _audioReaction.StartReaction();
            }
            else
            {
#if DEBUGGING
                Log.Info(this, $"[Run] Is not ready.", Log.Type.BehaviorTree);
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