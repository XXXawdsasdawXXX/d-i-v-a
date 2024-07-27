using Code.Data.Configs;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Characters.Reactions
{
    public class CharacterMouseReaction : CharacterReaction ,IGameTickListener
    {
        [SerializeField] private CharacterAnimator _characterAnimator;
        [SerializeField] private float _centralNormalValue = 0.3f;
        [SerializeField] private Vector2 _offset;

        private bool _isActive;
        private PositionService _positionService;

        protected override int _cooldownTickCount { get; set; }

        protected override void Init()
        {
            _positionService = Container.Instance.FindService<PositionService>();
            base.Init();
        }

        public void GameTick()
        {
            if (_isActive)
            {
                SetAnimationMousePosition();
            }
        }

        protected override void SetCooldownMinutes()
        {
            _cooldownTickCount = Container.Instance.FindConfig<TimeConfig>().Cooldown.ReactionMouse;
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
            var normal = (_positionService.GetMouseWorldPosition() - (transform.position + _offset.AsVector3())).normalized;
            var roundedX = Mathf.Abs(normal.x) < _centralNormalValue ? 0 : (normal.x < 0 ? -1 : 1);
            var roundedY = Mathf.Abs(normal.y) < _centralNormalValue ? 0 : (normal.y < 0 ? -1 : 1);
            _characterAnimator.SetMouseNormal(roundedX, roundedY);
        }
    }
}