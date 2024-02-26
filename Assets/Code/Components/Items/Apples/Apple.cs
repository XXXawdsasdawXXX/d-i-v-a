﻿using System;
using System.Collections;
using Code.Components.Items;
using Code.Components.Objects;
using Code.Data.Configs;
using Code.Data.Storages;
using Code.Data.Value;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Apples
{
    public class Apple : Item, IGameInitListener
    {
        [Header("Components")] 
        [SerializeField] private AppleAnimator _appleAnimator;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private ColliderButton _colliderButton;
        [SerializeField] private ColliderDragAndDrop _dragAndDrop;
        public ColliderButton ColliderButton => _colliderButton;
        public AppleEvent Event { get; private set; } = new();

        [Header("Services")] 
        private LiveStateStorage _liveStateStorage;

        [Header("Value")] 
        private AppleConfig _appleConfig;
        private TickCounter _tickCounter;
        private bool _isFall;
        private bool _isBig;
        public int MaxStage => 5;
        public int CurrentStage { get; private set; }


        public void GameInit()
        {
            _appleConfig = Container.Instance.FindConfig<AppleConfig>();
            _liveStateStorage = Container.Instance.FindStorage<LiveStateStorage>();
            
            _tickCounter = new TickCounter();
        }


        public void Grow()
        {
            _isFall = false;
            _isBig = false;
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            _rigidbody2D.velocity = Vector2.zero;
            CurrentStage = 0;
            
            _dragAndDrop.Activate();
            _appleAnimator.PlayEnter();
            _appleAnimator.SetAppleStage(CurrentStage);

            _tickCounter.WaitedEvent += OnTickCounterWaited;
            _tickCounter.StartWait(_appleConfig.OneStageLiveTimeTick.GetRandomValue());
            Debugging.Instance.Log($"[Grow]", Debugging.Type.Apple);
        }

        public void ReadyForUse()
        {
            _dragAndDrop.Deactivate();
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            _rigidbody2D.velocity = Vector2.zero;
            Debugging.Instance.Log($"[ReadyForUse]", Debugging.Type.Apple);
        }

        public override void Use(Action OnEnd = null)
        {
            _appleAnimator.PlayUse(onEnd: () =>
            {
                Debugging.Instance.Log($"[Use] анимация закончена, начисляются значения", Debugging.Type.Apple);
                _liveStateStorage.AddPercentageValues(GetLiveStateValues());
                Event?.InvokeUseEvent();
                OnEnd?.Invoke();
                Reset();
            });

            Debugging.Instance.Log($"[Use] стадия {CurrentStage}", Debugging.Type.Apple);
        }

        private LiveStatePercentageValue[] GetLiveStateValues()
        {
            return _isBig
                ? CurrentStage < _appleConfig.BigAppleValues.Length ? _appleConfig.BigAppleValues[CurrentStage].Values : null
                : CurrentStage < _appleConfig.BigAppleValues.Length ? _appleConfig.SmallAppleValues[CurrentStage].Values : null;
        }

        public void Fall()
        {
            _isFall = true;
            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            Debugging.Instance.Log($"[Fall]", Debugging.Type.Apple);
        }


        public void Die()
        {
            _liveStateStorage.AddPercentageValue(_appleConfig.DieAppleEffect);
            Event.InvokeDieEvent();
            Debugging.Instance.Log($"[Die]", Debugging.Type.Apple);
            Reset();
        }

        private void Reset()
        {
            CurrentStage = 0;
            _tickCounter.StopWait();
            _tickCounter.WaitedEvent -= OnTickCounterWaited;
            Debugging.Instance.Log($"[Reset]", Debugging.Type.Apple);
        }

        private void OnTickCounterWaited()
        {
            if (!_isBig && !_isFall)
            {
                _isBig = true;
                _appleAnimator.SetBigApple();
                Event.InvokeSetBigAppleEvent();
                Debugging.Instance.Log($"[OnTickCounterWaited] Set big apple", Debugging.Type.Apple);
            }
            else
            {
                CurrentStage++;
                Debugging.Instance.Log($"[OnTickCounterWaited] current stage ++ = {CurrentStage}", Debugging.Type.Apple);
                if (CurrentStage == 2 && !_isFall)
                {
                    Fall();
                    Event.InvokeStartIllEvent();
                }

                _appleAnimator.SetAppleStage(CurrentStage);
            }

        }
    }
}