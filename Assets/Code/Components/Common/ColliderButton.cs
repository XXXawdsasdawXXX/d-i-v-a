using System;
using Code.Data.Configs;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Objects
{
    public class ColliderButton : CommonComponent, IGameInitListener, IGameTickListener
    {
        public bool IsPressed { get; private set; }
        public event Action<Vector2> DownEvent;
        public event Action<Vector2, float> UpEvent;
        public event Action<int> SeriesOfClicksEvent;

        private float _pressedTime;


        private float _maxClickCooldown, _currentClickCooldown;
        private int _clickNumber;
        private PositionService _positionService;

        public void GameInit()
        {
            _maxClickCooldown = Container.Instance.FindConfig<TimeConfig>().ClickSeries;
            _positionService = Container.Instance.FindService<PositionService>();
        }

        public void GameTick()
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
            
            DownEvent?.Invoke(_positionService.GetMouseWorldPosition());
            SeriesOfClicksEvent?.Invoke(_clickNumber);
            
            Debugging.Instance.Log($"{gameObject.name}: Mouse down {_clickNumber}", Debugging.Type.ButtonSprite);
        }

        private void OnMouseUp()
        {
            IsPressed = false;
            UpEvent?.Invoke(_positionService.GetMouseWorldPosition(), _pressedTime);
            _pressedTime = 0;

            Debugging.Instance.Log($"{gameObject.name}: Mouse up", Debugging.Type.ButtonSprite);
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