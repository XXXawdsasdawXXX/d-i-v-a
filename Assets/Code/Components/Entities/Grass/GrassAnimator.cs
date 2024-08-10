using UnityEngine;

namespace Code.Components.Entities.Grass
{
    public class GrassAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _grassAnimator;
        
        private static readonly int IsActive = Animator.StringToHash("IsActive");

        public void PlayGrow()
        {
            _grassAnimator.SetBool(IsActive, true);
        }

        public void PlayDie()
        {
            _grassAnimator.SetBool(IsActive, false);
        }
    }
}