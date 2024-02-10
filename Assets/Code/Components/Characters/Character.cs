using Code.Components.Character.LiveState;
using Code.Components.Characters.Reactions;
using Code.Components.Objects;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components.Characters
{
    public class Character : Entity, IGameInitListener, IGameStartListener
    {
        
        public LiveStatesAnalytics StatesAnalytics { get; private set; }
        
        
        [SerializeField] private CharacterAnimator _characterAnimator;
        public CharacterAnimator Animator => _characterAnimator;

        
        [SerializeField] private CharacterAnimationAnalytic _characterAnimationAnalytic;
        public CharacterAnimationAnalytic AnimationAnalytic => _characterAnimationAnalytic;

        
        [SerializeField] private ColliderButton _colliderButton;
        public ColliderButton ColliderButton => _colliderButton;
        
        [SerializeField] private CharacterReaction[] _reactions;

        public void GameInit()
        {
            StatesAnalytics = new LiveStatesAnalytics();
        }

        public void GameStart()
        {
            StatesAnalytics.CheckLowerState();            
        }

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