using System;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using UnityEngine;

namespace Code.Components.Hands
{
    public class HandMovement : MonoBehaviour, IGameInitListener, IGameTickListener
    {
        [Header("Static value")]
        [SerializeField] private float _speed;
        [SerializeField] private float _stopDistance;
        [Header("Dynamic value")] 
        private Vector3 _target;
        [Header("Service")]
        private PositionService _positionService;


        public void GameInit()
        {
            _positionService = Container.Instance.FindService<PositionService>();
        }


        public void GameTick()
        {
            var mouse = _positionService.GetMouseWorldPosition();
            if (Vector3.Distance(transform.position,mouse) > _stopDistance)
            {
                _target = mouse;
            }
            transform.position = Vector3.Lerp(transform.position, _target, _speed * Time.deltaTime);
        }
    }
}