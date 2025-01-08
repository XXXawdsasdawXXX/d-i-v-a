using System;
using Code.Utils;
using UnityEngine;

namespace Code.Entities.Common
{
    public class CollisionObserver : CommonComponent
    {
        public event Action<GameObject> EnterEvent;
        public event Action<GameObject> ExitEvent;

        private void OnCollisionEnter2D(Collision2D col)
        {
            Debugging.Log($"[{col.gameObject.name}] Collision enter ", Debugging.Type.Collision);
            EnterEvent?.Invoke(col.gameObject);
        }

        private void OnCollisionExit2D(Collision2D col)
        {
            Debugging.Log($"[{col.gameObject.name}] Collision exit ", Debugging.Type.Collision);
            ExitEvent?.Invoke(col.gameObject);
        }


        private void OnTriggerEnter2D(Collider2D col)
        {
            Debugging.Log($"[{col.gameObject.name}] Trigger enter ", Debugging.Type.Collision);
            EnterEvent?.Invoke(col.gameObject);
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            Debugging.Log($"[{col.gameObject.name}] Trigger exit ", Debugging.Type.Collision);
            ExitEvent?.Invoke(col.gameObject);
        }
    }
}