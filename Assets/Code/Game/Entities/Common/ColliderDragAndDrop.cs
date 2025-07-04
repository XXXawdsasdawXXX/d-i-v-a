﻿using System;
using System.Collections;
using Code.Data;
using Code.Game.Services;
using Code.Game.Services.Position;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.ServiceLocator;
using Code.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Game.Entities.Common
{
    public class ColliderDragAndDrop : CommonComponent, IToggle,
        IInitializeListener, 
        IStartListener,
        IUpdateListener
    {
        [Header("Params")] 
        [SerializeField] protected bool _isActive;
        [SerializeField] private Vector2 _offset;
        [SerializeField] protected ColliderButton _colliderButton;
        private Vector2 _boarder;
        public bool IsActive => _isActive;

        [Header("Services")] 
        private PositionService _positionService;
        private CoroutineRunner _coroutineRunner;

        [Header("Dynamic values")] 
        private Coroutine _coroutine;
        protected bool _isDragging;
        private Vector3 _target;
        private Vector2 _startDragPosition;

        public event Action<float> OnEndedDrag;
        
        
        public async UniTask GameInitialize()
        {
            _coroutineRunner = Container.Instance.GetService<CoroutineRunner>();
            _positionService = Container.Instance.GetService<PositionService>();

            await InitializeDragAndDrop();
        }

        public UniTask GameStart()
        {
            _boarder = (Vector2)_positionService.GetPosition(EPointAnchor.LowerRight);
            
            return UniTask.CompletedTask;
        }

        public void GameUpdate()
        {
            if (_isActive && _isDragging && _colliderButton.IsPressed)
            {
                SetTarget();
                transform.position = Vector3.Lerp(transform.position, _target, 15 * Time.deltaTime);
            }
        }

        #region Unique methods

        public virtual void Active(Action OnTurnedOn = null)
        {
            _isActive = true;
            OnTurnedOn?.Invoke();
            SubscribeToEvents(true);
        }

        public virtual void Disable(Action onTurnedOff = null)
        {
            _isActive = false;
            onTurnedOff?.Invoke();
            SubscribeToEvents(false);
        }

        protected virtual UniTask InitializeDragAndDrop()
        {
            return UniTask.CompletedTask;
        }

        private void SetTarget()
        {
            Vector3 pos = _positionService.GetMouseWorldPosition();
            Vector3 targetPosition = pos + _offset.AsVector3();

            bool isCorrectPosition = targetPosition.y > _boarder.y
                                     && targetPosition.x < _boarder.x
                                     && targetPosition.x > -_boarder.x;

            if (isCorrectPosition)
            {
                _target = targetPosition;
            }
        }

        #endregion

        #region Events

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _colliderButton.OnPressedDown += OnPressedDown;
                _colliderButton.OnPressedUp += OnPressedUp;
            }
            else
            {
                _colliderButton.OnPressedDown -= OnPressedDown;
                _colliderButton.OnPressedUp -= OnPressedUp;
            }
        }

        protected virtual void OnPressedUp(Vector2 arg1, float arg2)
        {
            _isDragging = false;
            OnUp();
        }

        protected virtual void OnPressedDown(Vector2 obj)
        {
            Vector3 clickPosition = _positionService.GetMouseWorldPosition();

            Vector3 position = transform.position;
            
            _startDragPosition = position;
            
            _offset = position - clickPosition;

            if (_coroutine != null)
            {
                _coroutineRunner.StopRoutine(_coroutine);
            }

            _isDragging = true;
        }
        
        private void OnUp()
        {
            _coroutine = _coroutineRunner.StartRoutine(MoveUpRoutine());
          
            OnEndedDrag?.Invoke(Vector3.Distance(_startDragPosition, transform.position));
        }

        //todo refactoring to unitask
        private IEnumerator MoveUpRoutine()
        {
            WaitForEndOfFrame period = new();
            Vector3 forward = new(0, 0.1f, 0);
            
            while (transform.position.y < _boarder.y)
            {
                transform.position += forward;
                yield return period;
            }
        }

        #endregion
    }
}