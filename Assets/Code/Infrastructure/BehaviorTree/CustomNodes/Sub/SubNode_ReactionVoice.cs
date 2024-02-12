using Code.Components.Characters;
using Code.Infrastructure.DI;

namespace Code.Infrastructure.BehaviorTree.CustomNodes.Sub
{
    public class SubNode_ReactionVoice: BaseNode
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
            _audioReaction.StartReaction();
            Return(true);
        }
    }
}