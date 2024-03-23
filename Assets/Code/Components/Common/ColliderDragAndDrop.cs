using System.Collections;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Objects
{
    public class ColliderDragAndDrop : CommonComponent, IGameInitListener,IGameTickListener, IActivated
    {
        [Header("Params")]
        [SerializeField] protected bool _isActive;
        [SerializeField] private Vector2 _offset;
        [SerializeField] protected ColliderButton _colliderButton;

        [Header("Services")]
        private PositionService _positionService;
        private CoroutineRunner _coroutineRunner;

        [Header("Dinamic values")] 
        private Coroutine _coroutine;
        private bool _isDragging;


        public void GameInit()
        {
            _positionService = Container.Instance.FindService<PositionService>();
            _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();
            
            Init();
            
            SubscribeToEvents(true);
        }

        public void GameTick()
        {
            if (_isActive && _isDragging && _colliderButton.IsPressed)
            {
                Move();
            }
        }

        #region Unique methods

        public virtual void On()
        {
            _isActive = true;
        }

        public virtual void Off()
        {
            _isActive = false;
        }

        protected virtual void Init()
        {
            
        }

        protected virtual void Move()
        {
            Vector3 pos = _positionService.GetMouseWorldPosition();
            transform.position = pos + _offset.AsVector3();
        }

        #endregion

        #region Events
        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _colliderButton.DownEvent += OnPressDown;
                _colliderButton.UpEvent += OnPressUp;
            }
            else
            {
                _colliderButton.DownEvent -= OnPressDown;
                _colliderButton.UpEvent -= OnPressUp;
            }
        }

        protected virtual void OnPressUp(Vector2 arg1, float arg2)
        {
            _isDragging = false;
            MoveUp();
        }

        protected virtual void OnPressDown(Vector2 obj)
        {
            Vector3 clickPosition = _positionService.GetMouseWorldPosition();
            _offset = transform.position - clickPosition;
            
            if (_coroutine != null)
            {
                _coroutineRunner.StopRoutine(_coroutine);
            }
            _isDragging = true;
        }


        private void MoveUp()
        { 
            _coroutine = _coroutineRunner.StartRoutine(MoveUpRoutine());
        }
        
        private IEnumerator MoveUpRoutine()
        {
            var lowerPosition = _positionService.GetPosition(PointAnchor.LowerCenter);
            var period = new WaitForEndOfFrame();
            while (transform.position.y < lowerPosition.y)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, 0);
                yield return period;
            }
        }
        
        #endregion
    }
}