using System;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Objects
{
    public class ButtonSprite : MonoBehaviour, IGameTickListener
    {
        public bool IsPressed { get; private set; }
        public event Action<Vector2> MouseDownEvent;
        public event Action<Vector2, float> MouseUpEvent;
        public event Action<int> SeriesOfClicksEvent;

        private float _pressedTime;
        
        public void GameTick()
        {
            if (IsPressed)
            {
                _pressedTime += Time.deltaTime;
            }
        }
        
        void OnMouseDown()
        {
            IsPressed = true;
            MouseDownEvent?.Invoke(PositionService.GetMouseWorldPosition());
            
            Debugging.Instance.Log($"{gameObject.name}: Mouse down", Debugging.Type.ButtonSprite);
        }

        private void OnMouseUp()
        {
            IsPressed = false;
            MouseUpEvent?.Invoke(PositionService.GetMouseWorldPosition(),_pressedTime);
            _pressedTime = 0;
            
            Debugging.Instance.Log($"{gameObject.name}: Mouse up", Debugging.Type.ButtonSprite);
        }

        private void OnMouseEnter()
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
        }
    }
}