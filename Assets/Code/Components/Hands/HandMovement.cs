using System;
using Code.Data.Interfaces;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using UnityEngine;

namespace Code.Components.Hands
{
    public class HandMovement : HandComponent, IGameInitListener, IGameTickListener, IToggle
    {
        [Header("Static value")] 
        [SerializeField] private float _speed;
        [SerializeField] private float _stopDistance;
     
        [Header("Dynamic value")] 
        private Vector3 _target;
        private bool _isMove;
    
        [Header("Service")] 
        private PositionService _positionService;
        
        public void GameInit()
        {
            _positionService = Container.Instance.FindService<PositionService>();
        }
        
        public void GameTick()
        {
            if (_isMove)
            {
                var mouse = _positionService.GetMouseWorldPosition();
                if (Vector3.Distance(transform.position, mouse) > _stopDistance)
                {
                    _target = mouse;
                }

                transform.position = Vector3.Lerp(transform.position, _target, _speed * Time.deltaTime);
            }
        }
        
        public void On(Action onTurnedOn = null)
        {
            _isMove = true;
        }

        public void Off(Action onTurnedOff = null)
        {
            _isMove = false;
        }
    }
}