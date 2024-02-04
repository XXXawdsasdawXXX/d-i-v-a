using Code.Components.Character.LiveState;
using Code.Components.Characters.Reactions;
using Code.Components.Objects;
using UnityEngine;

namespace Code.Components.Characters
{
    public class Character : Entity
    {
        public LiveStatesAnalytics StatesAnalytics { get; private set; }= new();
        
        
        [SerializeField] private ColliderButton _colliderButton;
        public ColliderButton ColliderButton => _colliderButton;

        
        [SerializeField] private CharacterAnimator _characterAnimator;
        public CharacterAnimator Animator => _characterAnimator;

        
        [SerializeField] private CharacterAnimationManager characterAnimationManager;
        public CharacterAnimationManager AnimationManager => characterAnimationManager;

        
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