using System;
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
        [Header("Components")] [SerializeField]
        private AppleAnimator _appleAnimator;

        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private ColliderButton _colliderButton;
        [SerializeField] private ColliderDragAndDrop _dragAndDrop;
        public ColliderButton ColliderButton => _colliderButton;
        public AppleEvent Event { get; private set; } = new();


        public int CurrentStage { get; private set; }
        public int MaxStage => 5;

        private AppleConfig _appleConfig;
        private LiveStateStorage _liveStateStorage;

        private TickCounter _tickCounter;
        private bool _isFall;
        private bool _isBig;


        public void GameInit()
        {
            _appleConfig = Container.Instance.FindConfig<AppleConfig>();
            _liveStateStorage = Container.Instance.FindStorage<LiveStateStorage>();

            Debugging.Instance.Log($"Init apple {_appleConfig != null} {_liveStateStorage != null}",
                Debugging.Type.Apple);

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
            Debugging.Instance.Log($"Grow", Debugging.Type.Apple);
        }

        public void ReadyForUse()
        {
            _dragAndDrop.Deactivate();
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            _rigidbody2D.velocity = Vector2.zero;
            Debugging.Instance.Log($"ReadyForUse", Debugging.Type.Apple);
        }

        public override void Use(Action OnEnd = null)
        {
            _appleAnimator.PlayUse(onEnd: () =>
            {
                _liveStateStorage.AddPercentageValues(_isBig
                    ? _appleConfig.BigAppleValues[CurrentStage].Values
                    : _appleConfig.SmallAppleValues[CurrentStage].Values);

                transform.position = Vector3.zero;
                Event?.InvokeUseEvent();
                OnEnd?.Invoke();
                Reset();
            });

            Debugging.Instance.Log($"Use apple {_appleConfig != null} {_liveStateStorage != null}  {CurrentStage}", Debugging.Type.Apple);
        }

        public void Fall()
        {
            _isFall = true;
            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            Debugging.Instance.Log($"Fall", Debugging.Type.Apple);
        }


        public void Die()
        {
            _liveStateStorage.AddPercentageValue(_appleConfig.DieAppleEffect);
            Event.InvokeDieEvent();
            Debugging.Instance.Log($"Die", Debugging.Type.Apple);
            Reset();
        }

        private void Reset()
        {
            CurrentStage = 0;
            _tickCounter.StopWait();
            _tickCounter.WaitedEvent -= OnTickCounterWaited;
            Debugging.Instance.Log($"Reset", Debugging.Type.Apple);
        }

        private void OnTickCounterWaited()
        {
            if (!_isBig && !_isFall)
            {
                _isBig = true;
                _appleAnimator.SetBigApple();
            Debugging.Instance.Log($"Set big apple", Debugging.Type.Apple);
            }
            else
            {
                CurrentStage++;
            Debugging.Instance.Log($"current stage ++ = {CurrentStage}", Debugging.Type.Apple);
                if (CurrentStage == 2 && !_isFall)
                {
                    Fall();
                    Event.InvokeStartIllEvent();
                }

                _appleAnimator.SetAppleStage(CurrentStage);
            }

            Event.InvokeGrowEvent();
            
        }
    }
}