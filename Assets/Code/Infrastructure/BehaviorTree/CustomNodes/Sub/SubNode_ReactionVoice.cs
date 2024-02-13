using Code.Components.Characters;
using Code.Infrastructure.DI;
using Code.Utils;

namespace Code.Infrastructure.BehaviorTree.CustomNodes.Sub
{
    public class SubNode_ReactionVoice : BaseNode
    {
        private readonly CharacterAudioReaction _audioReaction;

        public SubNode_ReactionVoice()
        {
            _audioReaction = Container.Instance.FindEntity<Character>().FindReaction<CharacterAudioReaction>();
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
                Return(true);
            }
            else
            {
                Debugging.Instance.Log($"Саб нода реакция на звук: отказ", Debugging.Type.BehaviorTree);
                Return(false);
            }
        }
    }
}