using System;
using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using UnityEngine;

namespace Code.Entities.Common
{
    public class MovementToMouse : CommonComponent, 
        IInitListener, IUpdateListener, 
        IToggle
    {
        [Header("Static value")] 
        [SerializeField] private float _speed;
        [SerializeField] private float _stopMouseDistance = 1;
        [SerializeField] private float _stopDivaDistance = 2;

        [Header("Dynamic value")] 
        private Vector3 _target;
        private bool _isMove;

        [Header("Service")] 
        private PositionService _positionService;
        private Transform _divaTransform;


        public void GameInitialize()
        {
            _positionService = Container.Instance.FindService<PositionService>();
            _divaTransform = Container.Instance.FindEntity<Diva.DivaEntity>().transform;
        }

        public void GameUpdate()
        {
            if (_isMove)
            {
                Vector3 mouse = _positionService.GetMouseWorldPosition();

                if (Vector3.Distance(transform.position, mouse) > _stopMouseDistance &&
                    Vector3.Distance(_divaTransform.position, mouse) > _stopDivaDistance)
                {
                    _target = mouse;
                }

                transform.position = Vector3.Lerp(transform.position, _target, _speed * Time.deltaTime);
            }
        }

        public void Active(Action OnTurnedOn = null)
        {
            _isMove = true;
        }

        public void Disable(Action onTurnedOff = null)
        {
            _isMove = false;
        }
    }
}