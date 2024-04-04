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
        [Header("Components")] 
        [SerializeField] private AppleAnimator _appleAnimator;
        [SerializeField] private PhysicsDragAndDrop _dragAndDrop;
        [SerializeField] private ColliderButton _colliderButton;

        [Header("Services")] 
        private LiveStateStorage _liveStateStorage;
        private CoroutineRunner _coroutineRunner;

        [Header("Static value")] 
        private AppleConfig _appleConfig;
        private TickCounter _tickCounter;

        [Header("Dynamic value")] 
        private bool _isFall;
        private int _currentStage;
        public bool IsActive { get; private set; }
        
        public void GameInit()
        {
            _appleConfig = Container.Instance.FindConfig<AppleConfig>();
            _liveStateStorage = Container.Instance.FindStorage<LiveStateStorage>();
            _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();
            _tickCounter = new TickCounter();
        }
        
        public void Grow()
        {
            IsActive = true;
            _isFall = false;
            _currentStage = 0;

            _appleAnimator.PlayEnter();
            _appleAnimator.SetAppleStage(_currentStage);

            _tickCounter.WaitedEvent += OnTickCounterWaited;
            _tickCounter.StartWait(_appleConfig.OneStageLiveTimeTick.GetRandomValue());
            Debugging.Instance.Log($"[Grow]", Debugging.Type.Apple);
        }

      
        public override void ReadyForUse(Vector3 position)
        {
            Debugging.Instance.Log($"[Ready For Use] ожидание отпускания кнопки яблока", Debugging.Type.Apple);
            _coroutineRunner.StartCoroutine(ReadyForUseRoutine(position));
        }

        private IEnumerator ReadyForUseRoutine(Vector3 position)
        {
            yield return new WaitUntil(() => !_colliderButton.IsPressed);
            Debugging.Instance.Log($"[Ready For Use] драг энд дроп отключен", Debugging.Type.Apple);
            _dragAndDrop.Off();
            base.ReadyForUse(position);
        }
        
        public override void Use(Action OnEnd = null)
        {
            StartCoroutine(PlayUse(OnEnd));
            Debugging.Instance.Log($"[Use] стадия {_currentStage}", Debugging.Type.Apple);
        }

        private IEnumerator PlayUse(Action OnEnd = null)
        {
            yield return new WaitUntil(() => IsReady);
            _appleAnimator.PlayUse(onEnd: () =>
            {
                Debugging.Instance.Log($"[Use] анимация закончена, начисляются значения", Debugging.Type.Apple);
                _liveStateStorage.AddPercentageValues(GetLiveStateValues());
                OnEnd?.Invoke();
                UseEvent?.Invoke(this);
                Reset();
            });
        }

        private LiveStatePercentageValue[] GetLiveStateValues()
        {
            return  _currentStage < _appleConfig.AppleValues.Length ? _appleConfig.AppleValues[_currentStage].Values : null;
        }
        
        public void Die()
        {
            if (!IsActive)
            {
                return;
            }
            DieEvent?.Invoke();
            _liveStateStorage.AddPercentageValue(_appleConfig.DieAppleEffect);
            Debugging.Instance.Log($"[Die]", Debugging.Type.Apple);
            Reset();
        }

        public override void Reset()
        {
            IsActive = false;
            _currentStage = 0;
            _tickCounter.StopWait();
            _tickCounter.WaitedEvent -= OnTickCounterWaited;
            Debugging.Instance.Log($"[Reset]", Debugging.Type.Apple);
            base.Reset();
        }

        private void OnTickCounterWaited()
        {
            if(_isUsing)
            {
                return;
            }
            _currentStage++;
            Debugging.Instance.Log($"[OnTickCounterWaited] current stage ++ = {_currentStage}", Debugging.Type.Apple);
            _appleAnimator.SetAppleStage(_currentStage);
        }
    }
}
