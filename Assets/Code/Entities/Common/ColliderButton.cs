using System;
using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Entities.Common
{
    public class ColliderButton : CommonComponent, IGameInitListener, IGameUpdateListener
    {
        [Header("Services")]
        private PositionService _positionService;

        [Header("Static values")] 
        private float _maxClickCooldown;
        [field: SerializeField] public bool IsPressed { get; private set; }

        [Header("Dynamic values")] 
        private float _pressedTime;
        private float _currentClickCooldown;
        private int _clickNumber;

        public event Action<Vector2> OnPressedDown;
        public event Action<Vector2, float> OnPressedUp;
        public event Action<int> SeriesOfClicksEvent;

        public void GameInit()
        {
            _maxClickCooldown = Container.Instance.FindConfig<TimeConfig>().ClickSeries;
            _positionService = Container.Instance.FindService<PositionService>();
        }

        public void GameUpdate()
        {
            if (IsPressed)
            {
                _pressedTime += Time.deltaTime;
            }

            if (_clickNumber > 0)
            {
                if (_currentClickCooldown < _maxClickCooldown)
                {
                    _currentClickCooldown += Time.deltaTime;
                }
                else
                {
                    _clickNumber = 0;
                }
            }
        }

        private void OnMouseDown()
        {
            IsPressed = true;
            _clickNumber++;
            _currentClickCooldown = 0;

            OnPressedDown?.Invoke(_positionService.GetMouseWorldPosition());
            SeriesOfClicksEvent?.Invoke(_clickNumber);

#if DEBUGGING
            Debugging.Log(this, $"{gameObject.name}: Mouse down {_clickNumber}.", Debugging.Type.ButtonSprite);
#endif
        }

        private void OnMouseUp()
        {
            IsPressed = false;
            OnPressedUp?.Invoke(_positionService.GetMouseWorldPosition(), _pressedTime);
            _pressedTime = 0;

#if DEBUGGING
            Debugging.Log(this, $"{gameObject.name}: Mouse up.", Debugging.Type.ButtonSprite);
#endif
        }

        /*private void OnMouseEnter()
        {
            
            Debugging.Instance.Log($"{gameObject.name}: Mouse enter", Debugging.Type.ButtonSprite);
        }

        private void OnMouseExit()
        {
            Debugging.Instance.Log($"{gameObject.name}: Mouse exit", Debugging.Type.ButtonSprite);
        }

        private void OnMouseOver()
        {
            Debugging.Instance.Log($"{gameObject.name}: Mouse over", Debugging.Type.ButtonSprite);
        }*/
    }
}