﻿using System;
using Code.Components.Objects;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Apples
{
    public class AppleAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteAnimationMask _animationMask;

        private static readonly int Small = Animator.StringToHash("Small");
        private static readonly int Stage = Animator.StringToHash("Stage");
        private static readonly int Active = Animator.StringToHash("Active");
        private static readonly int Use = Animator.StringToHash("Use");


        private event Action ReactionEndEvent;
        public event Action ExitEndEvent;

        public void PlayEnter()
        {
            _animator.SetBool(Small, true);
            _animator.SetBool(Active, true);
            _animator.SetInteger(Stage, 0);

            Debugging.Instance.Log("Apple animation play enter", Debugging.Type.Apple);
        }

        public void SetBigApple()
        {
            _animator.SetBool(Small, false);
            Debugging.Instance.Log("Apple animation set big", Debugging.Type.Apple);
        }

        public void SetAppleStage(int stage)
        {
            _animator.SetInteger(Stage, stage);
            Debugging.Instance.Log($"Apple animation set apple stage {stage}", Debugging.Type.Apple);
        }

        public void PlayUse(Action onEnd = null)
        {
            _animationMask.Activate(OnShown: () =>
            {
                _animator.SetBool(Active, false);
                _animator.SetTrigger(Use);
                onEnd?.Invoke();
                Debugging.Instance.Log("Apple animation Invoke reaction end", Debugging.Type.Apple);
            });

            Debugging.Instance.Log("Apple animation play reaction", Debugging.Type.Apple);
        }


        private void InvokeExitEnd()
        {
            Debugging.Instance.Log("Apple animation Invoke exit end", Debugging.Type.Apple);
            _animator.SetBool(Active, false);
            ExitEndEvent?.Invoke();
        }
    }
}