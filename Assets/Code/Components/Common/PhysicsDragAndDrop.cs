using System;
using UnityEngine;

namespace Code.Components.Common
{
    public class PhysicsDragAndDrop : ColliderDragAndDrop
    {
        [SerializeField] private Rigidbody2D _rigidbody2D;

        protected override void Init()
        {
            if (_isActive)
            {
                On();
            }
            else
            {
                Off();
            }

            SetPhysicsActive(false);
            base.Init();
        }

        public override void On(Action onTurnedOn = null)
        {
            SetPhysicsActive(true);
            base.On(onTurnedOn);
            
            _isDragging = _colliderButton.IsPressed;
        }

        public override void Off(Action onTurnedOff = null)
        {
            SetPhysicsActive(false);
            base.Off(onTurnedOff);
            _isDragging = false;
        }

        public void SetKinematicMode()
        {
            SetPhysicsActive(false);
        }

        public void SetDynamicMode()
        {
            SetPhysicsActive(true);
        }

        protected override void OnPressedDown(Vector2 obj)
        {
            if (!_isActive)
            {
                return;
            }

            SetPhysicsActive(false);
            base.OnPressedDown(obj);
        }

        protected override void OnPressedUp(Vector2 arg1, float arg2)
        {
            if (!_isActive)
            {
                return;
            }

            SetPhysicsActive(true);
            base.OnPressedUp(arg1, arg2);
        }

        private void SetPhysicsActive(bool isActive)
        {
            if (_rigidbody2D == null)
            {
                return;
            }

            _rigidbody2D.bodyType = isActive ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;
            _rigidbody2D.velocity = Vector2.zero;
        }
    }
}