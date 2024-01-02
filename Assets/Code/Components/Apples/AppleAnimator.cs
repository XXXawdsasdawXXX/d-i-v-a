using System;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Objects
{
    public class AppleAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        private event Action ReactionEndEvent;
        private  event Action ExitEndEvent;

        public void PlayEnter()
        {
            gameObject.SetActive(true);
            Debugging.Instance.Log("Play animation enter", Debugging.Type.Item);
        }

        public void PlayReaction(Action onEnd = null)
        {
            ReactionEndEvent = onEnd;
            Debugging.Instance.Log("Play animation reaction", Debugging.Type.Item);
        }

        public void PlayExit(Action onEnd = null)
        {
            gameObject.SetActive(false);
            ExitEndEvent = onEnd;
            Debugging.Instance.Log("Play animation exit", Debugging.Type.Item);
        }

        private void InvokeReactionEnd()
        {
            Debugging.Instance.Log("Invoke reaction end", Debugging.Type.Item);
            ReactionEndEvent?.Invoke();
        }

        private void InvokeExitEnd()
        {
            Debugging.Instance.Log("Invoke exit end", Debugging.Type.Item);
            ExitEndEvent?.Invoke();
        }
    }
}