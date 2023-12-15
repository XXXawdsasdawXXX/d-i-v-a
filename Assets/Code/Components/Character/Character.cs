using UnityEngine;

namespace Code.Components.Character
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private CharacterAnimator _characterAnimator;
        public CharacterAnimator Animator => _characterAnimator;

        [SerializeField] private CharacterStateTracker _characterStateTracker;
        public CharacterStateTracker StateTracker => _characterStateTracker;

    }
}