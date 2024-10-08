﻿using System;
using Code.Components.Common;
using Code.Components.Entities.Characters;
using Code.Data.Interfaces;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using UnityEngine;

namespace Code.Components.Entities.Hands
{
    public class MovementToMouse : CommonComponent, 
        IGameInitListener, IGameTickListener, 
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


        public void GameInit()
        {
            _positionService = Container.Instance.FindService<PositionService>();
            _divaTransform = Container.Instance.FindEntity<DIVA>().transform;
        }

        public void GameTick()
        {
            if (_isMove)
            {
                var mouse = _positionService.GetMouseWorldPosition();

                if (Vector3.Distance(transform.position, mouse) > _stopMouseDistance &&
                    Vector3.Distance(_divaTransform.position, mouse) > _stopDivaDistance)
                {
                    _target = mouse;
                }

                transform.position = Vector3.Lerp(transform.position, _target, _speed * Time.deltaTime);
            }
        }

        public void On(Action OnTurnedOn = null)
        {
            _isMove = true;
        }

        public void Off(Action onTurnedOff = null)
        {
            _isMove = false;
        }
    }
}