using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components.Objects
{
    public class AppleBranch : MonoBehaviour, IGameInitListener, IGameStartListener, IGameExitListener
    {
        [SerializeField] private AppleBranchAnimator _branchAnimator;
        [SerializeField] private ColliderButton _colliderButton;
        [SerializeField] private Apple _apple;
        [Space]
        [SerializeField] private Transform _applePoint;
        [SerializeField] private Transform _smallApplePoint;


        public void GameInit()
        {
            SubscribeToEvents(true);
        }

        public void GameStart()
        {
        }

        public void GameExit()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _colliderButton.DownEvent += OnDown;
            }
            else
            {
                _colliderButton.DownEvent -= OnDown;
            }
        }

        private void OnDown(Vector2 position)
        {
            DestroyBranch();
        }

        public void GrowBranch()
        {
            _branchAnimator.PlayEnter(onEndAnimation: () =>
            {
                _apple.transform.position = _applePoint.position;
                _apple.Grow();
            });
        }

        public void DestroyBranch()
        {
            _branchAnimator.PlayExit();
            _apple.Fall();
        }
    }
}