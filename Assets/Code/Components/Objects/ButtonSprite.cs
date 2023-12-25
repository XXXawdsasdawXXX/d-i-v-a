using System;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Objects
{
    public class ButtonSprite : MonoBehaviour
    {
        [SerializeField] private bool _isPressed;
        public event Action<Vector2> MouseDownEvent;
        public event Action<Vector2, float> MouseUpEvent;
        public event Action<int> SeriesOfClicksEvent;

        private float _pressedTime;

        private void Update()
        {
            if (_isPressed)
            {
                _pressedTime += Time.deltaTime;
                
                Vector3 pos = GetMouseWorldPosition();
                transform.position = pos;
            }
        }

        private Vector3 GetMouseWorldPosition()
        {
            var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            return position;
        }

        void OnMouseDown()
        {
            _isPressed = true;
            MouseDownEvent?.Invoke(GetMouseWorldPosition());
            
            Debugging.Instance.Log($"{gameObject.name}: Mouse down", Debugging.Type.ButtonSprite);
        }

        private void OnMouseUp()
        {
            _isPressed = false;
            MouseUpEvent?.Invoke(GetMouseWorldPosition(),_pressedTime);
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