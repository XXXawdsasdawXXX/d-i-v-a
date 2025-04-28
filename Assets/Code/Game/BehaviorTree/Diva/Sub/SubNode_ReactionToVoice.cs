using Code.Game.Entities.Diva.Reactions;
using Code.Infrastructure.ServiceLocator;
using Code.Utils;

namespace Code.Game.BehaviorTree.Diva
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
                Log.Info(this, $"[Run]", Log.Type.BehaviorTree);

                _audioReaction.StartReaction();
            }
            else
            {
                Log.Info(this, $"[Run] Is not ready.", Log.Type.BehaviorTree);

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