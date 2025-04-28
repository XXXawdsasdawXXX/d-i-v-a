using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Game.Entities.Common
{
    public class PhysicsDragAndDrop : ColliderDragAndDrop
    {
        [SerializeField] private Rigidbody2D _rigidbody2D;

        protected override UniTask InitializeDragAndDrop()
        {
            if (_isActive)
            {
                Active();
            }
            else
            {
                Disable();
            }

            _setPhysicsActive(false);
            
            return base.InitializeDragAndDrop();
        }

        public override void Active(Action OnTurnedOn = null)
        {
            _setPhysicsActive(true);
         
            base.Active(OnTurnedOn);
            
            _isDragging = _colliderButton.IsPressed;
        }

        public override void Disable(Action onTurnedOff = null)
        {
            _setPhysicsActive(false);
        
            base.Disable(onTurnedOff);
            
            _isDragging = false;
        }

        public void SetKinematicMode()
        {
            _setPhysicsActive(false);
        }

        public void SetDynamicMode()
        {
            _setPhysicsActive(true);
        }

        protected override void OnPressedDown(Vector2 obj)
        {
            if (!_isActive)
            {
                return;
            }

            _setPhysicsActive(false);
            
            base.OnPressedDown(obj);
        }

        protected override void OnPressedUp(Vector2 arg1, float arg2)
        {
            if (!_isActive)
            {
                return;
            }

            _setPhysicsActive(true);
           
            base.OnPressedUp(arg1, arg2);
        }

        private void _setPhysicsActive(bool isActive)
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