using Code.Components.Characters.Reactions;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Characters
{
    public class CharacterMouseReaction : CharacterReaction, IGameTickListener
    {
        [SerializeField] private CharacterAnimator _characterAnimator;
        [SerializeField] private float _centralNormalValue = 0.3f;
        [SerializeField] private Vector2 _offset;

        private bool _isActive;
        
        public void GameTick()
        {
            if (_isActive)
            {
                SetAnimationMousePosition();
            }
        }

        public override void StartReaction()
        {
            _isActive = true;
            _characterAnimator.StartPlayReactionMouse();
            base.StartReaction();
        }

        public override void StopReaction()
        {
            _isActive = false;
            _characterAnimator.StopPlayReactionMouse();
            base.StopReaction();
        }

        private void SetAnimationMousePosition()
        {
            var normal = (PositionService.GetMouseWorldPosition() - (transform.position + _offset.AsVector3())).normalized;
            var roundedX = Mathf.Abs(normal.x) < _centralNormalValue ? 0 : (normal.x < 0 ? -1 : 1);
            var roundedY = Mathf.Abs(normal.y) < _centralNormalValue ? 0 : (normal.y < 0 ? -1 : 1);
            _characterAnimator.SetMouseNormal(roundedX, roundedY);
        }
    }
}