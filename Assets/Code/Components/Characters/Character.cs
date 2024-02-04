using Code.Components.Characters.Reactions;
using Code.Components.Objects;
using UnityEngine;

namespace Code.Components.Characters
{
    public class Character : Entity
    {
        
        [SerializeField] private ColliderButton _colliderButton;
        public ColliderButton ColliderButton => _colliderButton;

        
        [SerializeField] private CharacterAnimator _characterAnimator;
        public CharacterAnimator Animator => _characterAnimator;

        
        [SerializeField] private CharacterManager _characterManager;
        public CharacterManager Manager => _characterManager;


        [SerializeField] private CharacterReaction[] _reactions;
        
        public T FindReaction<T>() where T : CharacterReaction
        {
            foreach (var reaction in _reactions)
            {
                if (reaction is T characterReaction)
                {
                    return characterReaction;
                }
            }
            return null;
        }
    }
}