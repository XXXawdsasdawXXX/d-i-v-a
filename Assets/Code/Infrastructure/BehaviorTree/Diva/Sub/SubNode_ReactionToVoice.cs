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
            _audioReaction.EndReactionEvent += AudioReactionOnEndReactionEvent;
        }

        private void AudioReactionOnEndReactionEvent()
        {
            Return(true);
        }

        protected override void Run()
        {
            if (IsCanRun())
            {
                Debugging.Instance.Log($"Саб нода реакция на звук: запуск", Debugging.Type.BehaviorTree);
                _audioReaction.StartReaction();
            }
            else
            {
                Debugging.Instance.Log($"Саб нода реакция на звук: отказ", Debugging.Type.BehaviorTree);
                Return(false);
            }
        }

        protected override bool IsCanRun()
        {
            return _audioReaction.IsReady();
        }
    }
}