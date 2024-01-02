using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components.Objects
{
    public class AppleBranch : MonoBehaviour, IGameInitListener, IGameExitListener
    {
        [SerializeField] private AppleBranchAnimator _branchAnimator;
        [SerializeField] private ButtonSprite _buttonSprite;
        [SerializeField] private Apple _apple;
        [SerializeField] private Transform _applePoint;


        public void GameInit()
        {
            
        }

        public void GameExit()
        {
            
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _buttonSprite.MouseDownEvent += OnMouseDown;
            }
            else
            {
                
            }
        }

        private void OnMouseDown(Vector2 position)
        {
            
        }

        public void GrowBranch()
        {
            _branchAnimator.PlayEnter(onEndAnimation: GrowApple);
        }

        public void DestroyBranch()
        {
            
        }
        private void GrowApple()
        {
            _apple.Grow();
        }
    }
}