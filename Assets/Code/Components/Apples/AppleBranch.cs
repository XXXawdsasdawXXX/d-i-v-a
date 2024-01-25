using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components.Objects
{
    public class AppleBranch : Entity, IGameInitListener, IGameExitListener
    {
        [SerializeField] private AppleBranchAnimator _branchAnimator;
        [SerializeField] private ColliderButton _colliderButton;
        [SerializeField] private Apple _apple;
        [Space]
        [SerializeField] private Transform _applePoint;
        [SerializeField] private Transform _smallApplePoint;

        private bool _isActive;

        public void GameInit()
        {
            SubscribeToEvents(true);
        }
        
        public void GameExit()
        {
            SubscribeToEvents(false);
        }

        public void GrowBranch()
        {
            _branchAnimator.PlayEnter(onEndAnimation: () =>
            {
                _isActive = true;
                _apple.transform.position = _applePoint.position;
                _apple.Grow();
            });
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _colliderButton.DownEvent += TryDestroyBranch;
                _apple.ColliderButton.DownEvent += TryDestroyBranch;
            }
            else
            {
                _colliderButton.DownEvent -= TryDestroyBranch;
                _apple.ColliderButton.DownEvent -= TryDestroyBranch;
            }
        }

        private void TryDestroyBranch(Vector2 position)
        {
            if (_isActive)
            {
                DestroyBranch();
            }
        }

        private void DestroyBranch()
        {
            _isActive = false;
            _branchAnimator.PlayExit();
            _apple.Fall();
        }
    }
}