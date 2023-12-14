using UnityEngine;

namespace Code.Components.Objects
{
    public class ButtonSprite : MonoBehaviour
    {
        [SerializeField] private bool _isPressed;

        private void Update()
        {
            if (_isPressed)
            {
                var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0;
                transform.position = pos;
            }
        }

        void OnMouseDown()
        {
            _isPressed = true;
            Debug.Log("Mouse down");
        }

        private void OnMouseUp()
        {
            _isPressed = false;
            Debug.Log("Mouse up");
        }

        private void OnMouseEnter()
        {
            Debug.Log("Mouse enter");
        }

        private void OnMouseExit()
        {
            Debug.Log("Mouse exit");
        }

        private void OnMouseOver()
        {
            Debug.Log("Mouse over");
        }
    }
}