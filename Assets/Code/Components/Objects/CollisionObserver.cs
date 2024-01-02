using System;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Objects
{
    public class CollisionObserver : MonoBehaviour
    {
        public event Action<GameObject> EnterEvent;
        public event Action<GameObject> ExitEvent;

        private void OnCollisionEnter2D(Collision2D col)
        {
            Debugging.Instance.Log($"Collision enter {col.gameObject.name}");
            EnterEvent?.Invoke(col.gameObject);
        }

        private void OnCollisionExit2D(Collision2D col)
        {
            Debugging.Instance.Log($"Collision exit {col.gameObject.name}");
            ExitEvent?.Invoke(col.gameObject);
        }


        private void OnTriggerEnter2D(Collider2D col)
        {
            Debugging.Instance.Log($"Trigger enter {col.gameObject.name}");
            EnterEvent?.Invoke(col.gameObject);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Debugging.Instance.Log($"Trigger exit {other.gameObject.name}");
            ExitEvent?.Invoke(other.gameObject);
        }
    }
}