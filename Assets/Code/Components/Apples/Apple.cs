using System;
using System.Collections;
using Code.Components.Objects;
using Code.Data.Configs;
using Code.Data.Storages;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Apples
{
    public class Apple : Entity, IGameInitListener
    {
        [Header("Components")] 
        [SerializeField] private AppleAnimator _appleAnimator;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private ColliderButton _colliderButton;
        [SerializeField] private ColliderDragAndDrop _dragAndDrop;

        public ColliderButton ColliderButton => _colliderButton;

        public AppleEvent Event { get; private set; } = new AppleEvent();
        public int CurrentStage { get; private set; }
        public int MaxStage => 5;

        private AppleConfig _appleConfig;
        private CharacterLiveStateStorage _liveStateStorage;

        private Coroutine _liveCoroutine;
        private bool _isFall;
        private bool _isBig;

        public void GameInit()
        {
            _appleConfig = Container.Instance.FindConfig<AppleConfig>();
            _liveStateStorage = Container.Instance.FindStorage<CharacterLiveStateStorage>();

            Debugging.Instance.Log($"Init apple {_appleConfig != null} {_liveStateStorage != null}",
                Debugging.Type.Apple);
        }

        public void Grow()
        {
            _isFall = false;
            _isBig = false;
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

            _dragAndDrop.Activate();
            _appleAnimator.PlayEnter();

            _liveCoroutine = StartCoroutine(StartLiveTimerRoutine());
        }

        public void Use(Action OnEnd = null)
        {
            if (_liveCoroutine != null)
            {
                StopCoroutine(_liveCoroutine);
            }

            _dragAndDrop.Deactivate();
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            _rigidbody2D.velocity = Vector2.zero;

            _appleAnimator.PlayUse(onEnd: () =>
            {
                _liveStateStorage.AddPercentageValues(_isBig
                    ? _appleConfig.BigAppleValues[CurrentStage].Values
                    : _appleConfig.SmallAppleValues[CurrentStage].Values);

                transform.position = Vector3.zero;
                Event?.InvokeEndLiveTimeEvent();
                OnEnd?.Invoke();
            });

            Debugging.Instance.Log($"Use apple {_appleConfig != null} {_liveStateStorage != null}  {CurrentStage}",
                Debugging.Type.Apple);
        }

        public void Fall()
        {
            _isFall = true;
            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        }

        private IEnumerator StartLiveTimerRoutine()
        {
            while (CurrentStage < MaxStage)
            {
                yield return  new WaitForSeconds(_appleConfig.LiveTimeSecond.GetRandomValue());

                if (!_isBig && !_isFall)
                {
                    _isBig = true;
                    _appleAnimator.SetBigApple();
                }
                else
                {
                    CurrentStage++;
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
}