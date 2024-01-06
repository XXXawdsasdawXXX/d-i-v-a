using System.Collections;
using Code.Data.Configs;
using Code.Data.StaticData;
using Code.Data.Storages;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components.Objects
{
    public class Apple : MonoBehaviour, IGameInitListener
    {
        [Header("Components")] 
        [SerializeField] private AppleAnimator _appleAnimator;
        [SerializeField] private Rigidbody2D _rigidbody2D;

        private AppleConfig _appleConfig;
        private CharacterLiveStateStorage _liveStateStorage;
        
        public int MaxStage => 5;
        public int CurrentStage { get; private set; }
        
        private Coroutine _liveCoroutine;
        private bool _isFall;
        private bool _isBig;


        public void GameInit()
        {
            _appleConfig = Container.Instance.FindConfig<AppleConfig>();
            _liveStateStorage = Container.Instance.FindStorage<CharacterLiveStateStorage>();
        }

        public void Grow()
        {
            _isFall = false;
            _isBig = false;
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

            _appleAnimator.PlayEnter();

            _liveCoroutine = StartCoroutine(StartLiveTimerRoutine());
        }

        public void Use()
        {
            StopCoroutine(_liveCoroutine);
            
            _liveStateStorage.AddPercentageValues(_isBig
                ? _appleConfig.BigAppleValues[CurrentStage].Values
                : _appleConfig.SmallAppleValues[CurrentStage].Values);
            
        }

        public void Fall()
        {
            _isFall = true;
            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        }

        private IEnumerator StartLiveTimerRoutine()
        {
            var period = new WaitForSeconds(_appleConfig.LiveTime.GetRandomValue() / MaxStage);

            while (CurrentStage < MaxStage)
            {
                yield return period;
                
                if (!_isBig && !_isFall)
                {
                    _isBig = true;
                    _appleAnimator.SetBigApple();
                }
                else
                {
                    CurrentStage++;
                    _appleAnimator.SetAppleStage(CurrentStage);
                }
            }
        }
    }
}