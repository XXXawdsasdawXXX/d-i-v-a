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
#if DEBUGGING
            Log.Info($"[{col.gameObject.name}] Collision enter ", Log.Type.Collision);
#endif
            EnterEvent?.Invoke(col.gameObject);
        }

        private void OnCollisionExit2D(Collision2D col)
        {
#if DEBUGGING
            Log.Info($"[{col.gameObject.name}] Collision exit ", Log.Type.Collision);
#endif
            ExitEvent?.Invoke(col.gameObject);
        }


        private void OnTriggerEnter2D(Collider2D col)
        {
#if DEBUGGING
            Log.Info($"[{col.gameObject.name}] Trigger enter ", Log.Type.Collision);
#endif
            EnterEvent?.Invoke(col.gameObject);
        }

        private void OnTriggerExit2D(Collider2D col)
        {
#if DEBUGGING
            Log.Info($"[{col.gameObject.name}] Trigger exit ", Log.Type.Collision);
#endif
            ExitEvent?.Invoke(col.gameObject);
        }
    }
}