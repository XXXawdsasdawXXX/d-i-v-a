using UnityEngine;

namespace Code.Components.Character
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private CharacterAnimator _characterAnimator;
        public CharacterAnimator Animator => _characterAnimator;

        [SerializeField] private CharacterLiveStateController _characterLiveStateController;
        public CharacterLiveStateController LiveStateController => _characterLiveStateController;

    }
}