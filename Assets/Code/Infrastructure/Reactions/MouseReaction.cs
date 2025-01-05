using Code.Components.Entities;
using Code.Data.Configs;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.Reactions
{
    public class MouseReaction : Reaction, IGameUpdateListener
    {
        private PositionService _positionService;
         
        private DivaAnimator _divaAnimator;
        private Transform _divaTransform;
        
        private readonly float _centralNormalValue = 0.3f;
        private readonly Vector2 _offset = new(0,1);

        private bool _isActive;
        
        protected override void Init()
        {
            _positionService = Container.Instance.FindService<PositionService>();
           
            Diva diva = Container.Instance.FindEntity<Diva>();
            _divaTransform = diva.transform;
            _divaAnimator = diva.FindCharacterComponent<DivaAnimator>();
            
            base.Init();
        }

        protected override int GetCooldownMinutes()
        {
            return Container.Instance.FindConfig<TimeConfig>().Cooldown.MouseReactionMin;
        }

        public void GameUpdate()
        {
            if (_isActive)
            {
                SetAnimationMousePosition();
            }
        }

        public override void StartReaction()
        {
            _isActive = true;
            _divaAnimator.StartPlayReactionMouse();
            base.StartReaction();
        }

        public override void StopReaction()
        {
            _isActive = false;
            _divaAnimator.StopPlayReactionMouse();
            base.StopReaction();
        }

        private void SetAnimationMousePosition()
        {
            Vector3 normal = (_positionService.GetMouseWorldPosition() - (_divaTransform.position + _offset.AsVector3()))
                .normalized;
            int roundedX = Mathf.Abs(normal.x) < _centralNormalValue ? 0 : (normal.x < 0 ? -1 : 1);
            int roundedY = Mathf.Abs(normal.y) < _centralNormalValue ? 0 : (normal.y < 0 ? -1 : 1);
            _divaAnimator.SetMouseNormal(roundedX, roundedY);
        }
    }
}