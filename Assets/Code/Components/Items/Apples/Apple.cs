using System;
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
        [SerializeField] private ColliderButton _colliderButton;
        [SerializeField] private ColliderDragAndDrop _dragAndDrop;
        public ColliderButton ColliderButton => _colliderButton;
        public AppleEvent Event { get; private set; } = new();

        [Header("Services")] 
        private LiveStateStorage _liveStateStorage;

        [Header("Static value")] 
        private AppleConfig _appleConfig;
        private TickCounter _tickCounter;

        [Header("Dinamic value")] 
        private bool _isFall;
        private bool _isBig;
        private int _currentStage;
        public bool IsActive { get; private set; }
        
        public void GameInit()
        {
            _appleConfig = Container.Instance.FindConfig<AppleConfig>();
            _liveStateStorage = Container.Instance.FindStorage<LiveStateStorage>();
            _tickCounter = new TickCounter();
        }


        public void Grow()
        {
            IsActive = true;
            _isFall = false;
            _isBig = false;
            _dragAndDrop.Off();
            _currentStage = 0;
            
            _appleAnimator.PlayEnter();
            _appleAnimator.SetAppleStage(_currentStage);

            _tickCounter.WaitedEvent += OnTickCounterWaited;
            _tickCounter.StartWait(_appleConfig.OneStageLiveTimeTick.GetRandomValue());
            Debugging.Instance.Log($"[Grow]", Debugging.Type.Apple);
        }

      
        public void ReadyForUse()
        {
            _dragAndDrop.Off();
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

            Debugging.Instance.Log($"[Use] стадия {_currentStage}", Debugging.Type.Apple);
        }

        private LiveStatePercentageValue[] GetLiveStateValues()
        {
            return _isBig
                ? _currentStage < _appleConfig.BigAppleValues.Length ? _appleConfig.BigAppleValues[_currentStage].Values : null
                : _currentStage < _appleConfig.BigAppleValues.Length ? _appleConfig.SmallAppleValues[_currentStage].Values : null;
        }

        public void Fall()
        {
            if (_isFall)
            {
                return;
            }
            _isFall = true;
            _dragAndDrop.On();
            Debugging.Instance.Log($"[Fall]", Debugging.Type.Apple);
        }


        public void Die()
        {
            if (!IsActive)
            {
                return;
            }
            _liveStateStorage.AddPercentageValue(_appleConfig.DieAppleEffect);
            Event.InvokeDieEvent();
            Debugging.Instance.Log($"[Die]", Debugging.Type.Apple);
            Reset();
        }

        private void Reset()
        {
            IsActive = false;
            _currentStage = 0;
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
                _currentStage++;
                Debugging.Instance.Log($"[OnTickCounterWaited] current stage ++ = {_currentStage}", Debugging.Type.Apple);
                if (_currentStage == 2 && !_isFall)
                {
                    Fall();
                    Event.InvokeStartIllEvent();
                }

                _appleAnimator.SetAppleStage(_currentStage);
            }

        }

    
    }
}