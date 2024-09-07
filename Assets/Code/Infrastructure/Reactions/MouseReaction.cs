using Code.Data.Configs;
using Code.Data.Interfaces;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Entities.Characters.Reactions
{
    public class MouseReaction : Reaction, IGameTickListener
    {
        private PositionService _positionService;
         
        private CharacterAnimator _characterAnimator;
        private Transform _divaTransform;
        
        private readonly float _centralNormalValue = 0.3f;
        private readonly Vector2 _offset = new(0,1);

        private bool _isActive;
        
        protected override void Init()
        {
            _positionService = Container.Instance.FindService<PositionService>();
           
            var diva = Container.Instance.FindEntity<DIVA>();
            _divaTransform = diva.transform;
            _characterAnimator = diva.FindCharacterComponent<CharacterAnimator>();
            
            base.Init();
        }

        protected override int GetCooldownMinutes()
        {
            return Container.Instance.FindConfig<TimeConfig>().Cooldown.ReactionMouse;
        }

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
            var normal = (_positionService.GetMouseWorldPosition() - (_divaTransform.position + _offset.AsVector3()))
                .normalized;
            var roundedX = Mathf.Abs(normal.x) < _centralNormalValue ? 0 : (normal.x < 0 ? -1 : 1);
            var roundedY = Mathf.Abs(normal.y) < _centralNormalValue ? 0 : (normal.y < 0 ? -1 : 1);
            _characterAnimator.SetMouseNormal(roundedX, roundedY);
        }
    }
}