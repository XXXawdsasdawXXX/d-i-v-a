using Code.Components.Objects;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components.Apples
{
    public class AppleBranch : Entity, IGameInitListener, IGameExitListener
    {
        [SerializeField] private AppleBranchAnimator _branchAnimator;
        [SerializeField] private ColliderButton _colliderButton;
        [SerializeField] private Apple _apple;
        [Space] 
        [SerializeField] private Transform _applePoint;
        [SerializeField] private Transform _smallApplePoint;

        private bool _isActiveBranch;
        private bool _isBigApple;

        public void GameInit()
        {
            SubscribeToEvents(true);
        }

        public void GameExit()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _apple.ColliderButton.DownEvent += TryDestroyBranch;
                _apple.Event.StartIllEvent += TryDestroyBranch;
                _colliderButton.DownEvent += TryDestroyBranch;
                _apple.Event.SetBigAppleEvent += SetBigApplePosition;
            }
            else
            {
                _apple.ColliderButton.DownEvent -= TryDestroyBranch;
                _apple.Event.StartIllEvent -= TryDestroyBranch;
                _colliderButton.DownEvent -= TryDestroyBranch;
                _apple.Event.SetBigAppleEvent -= SetBigApplePosition;
            }
        }

        public void GrowBranch()
        {
            _branchAnimator.PlayEnter(onEndAnimation: () =>
            {
                _isActiveBranch = true;
                _isBigApple = false;
                _apple.transform.position = _smallApplePoint.position;
                _apple.Grow();
            });
        }

        private void SetBigApplePosition()
        {
            if (_isBigApple)
            {
                return;
            }

            _isBigApple = true;
            _apple.transform.position = _applePoint.position;
        }

        private void TryDestroyBranch(Vector2 position)
        {
            if (_isActiveBranch)
            {
                DestroyBranch();
            }
        }

        private void TryDestroyBranch()
        {
            if (_isActiveBranch)
            {
                DestroyBranch();
            }
        }

        private void DestroyBranch()
        {
            _isActiveBranch = false;
            _branchAnimator.PlayExit();
            _apple.Fall();
        }
    }
}