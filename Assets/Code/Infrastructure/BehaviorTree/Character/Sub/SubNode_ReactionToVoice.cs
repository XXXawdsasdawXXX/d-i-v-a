using Code.Components.Entities.Characters;
using Code.Components.Entities.Characters.Reactions;
using Code.Infrastructure.BehaviorTree.BaseNodes;
using Code.Infrastructure.DI;
using Code.Utils;

namespace Code.Infrastructure.BehaviorTree.Character.Sub
{
    public class SubNode_ReactionToVoice : BaseNode
    {
        private readonly CharacterAudioReaction _audioReaction;

        public SubNode_ReactionToVoice()
        {
            _audioReaction = Container.Instance.FindEntity<DIVA>().FindReaction<CharacterAudioReaction>();
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