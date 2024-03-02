using UnityEngine;

namespace Code.Components.Objects
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

        protected override void OnPressDown(Vector2 obj)
        {
            SetPhysicsActive(false);
            base.OnPressDown(obj);
        }

        protected override void OnPressUp(Vector2 arg1, float arg2)
        {
            SetPhysicsActive(true);
            base.OnPressUp(arg1, arg2);
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