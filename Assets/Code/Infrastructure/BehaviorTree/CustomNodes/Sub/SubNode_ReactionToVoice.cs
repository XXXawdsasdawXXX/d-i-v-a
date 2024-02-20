using Code.Components.Characters;
using Code.Infrastructure.DI;
using Code.Utils;

namespace Code.Infrastructure.BehaviorTree.CustomNodes.Sub
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

        public bool IsReady()
        {
            return _audioReaction.IsReady();
        }

        protected override void Run()
        {
            if (IsReady())
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
    }
}