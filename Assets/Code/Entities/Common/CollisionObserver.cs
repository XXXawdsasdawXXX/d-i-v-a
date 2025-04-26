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
            Log.Info($"[{col.gameObject.name}] Collision enter ", Log.Type.Collision);
            EnterEvent?.Invoke(col.gameObject);
        }

        private void OnCollisionExit2D(Collision2D col)
        {
            Log.Info($"[{col.gameObject.name}] Collision exit ", Log.Type.Collision);
            ExitEvent?.Invoke(col.gameObject);
        }


        private void OnTriggerEnter2D(Collider2D col)
        {
            Log.Info($"[{col.gameObject.name}] Trigger enter ", Log.Type.Collision);
            EnterEvent?.Invoke(col.gameObject);
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            Log.Info($"[{col.gameObject.name}] Trigger exit ", Log.Type.Collision);
            ExitEvent?.Invoke(col.gameObject);
        }
    }
}