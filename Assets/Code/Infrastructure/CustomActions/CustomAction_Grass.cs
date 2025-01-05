using Code.Components.Common;
using Code.Components.Entities;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Value.RangeInt;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using UnityEngine;

namespace Code.Infrastructure.CustomActions
{
    public class CustomAction_Grass : CustomAction, 
        IGameInitListener, 
        IGameStartListener, 
        IGameExitListener
    {
        [Header("d i v a")] 
        private Transform _divaTransform;
        private DivaAnimator _divaAnimator;
        private PhysicsDragAndDrop _divaDragAndDrop;

        [Header("Grass Components")] 
        private Grass _grass;
        private ColorChecker _grassColorChecker;

        [Header("Action Delay")] 
        private TickCounter _tickCounter;
        private ColliderButton _grassButton;
        
        [Header("Static value")] 
        private RangedInt _tickDelay;

        [Header("Dynamic value")] 
        private Coroutine _coroutine;

        public override ECustomCutsceneActionType GetActionType() => ECustomCutsceneActionType.Grass;

        public void GameInit()
        {
            Diva diva = Container.Instance.FindEntity<Diva>();
            _divaTransform = diva.transform;
            _divaAnimator = diva.FindCharacterComponent<DivaAnimator>();
            _divaDragAndDrop = diva.FindCommonComponent<PhysicsDragAndDrop>();

            _grass = Container.Instance.FindEntity<Grass>();
            _grassColorChecker = _grass.FindCommonComponent<ColorChecker>();
            _grassButton = _grass.FindCommonComponent<ColliderButton>();

            _tickCounter = new TickCounter(isLoop: false);
            _tickDelay = Container.Instance.FindConfig<TimeConfig>().Delay.GrassGrow;
        }

        public void GameStart()
        {
            SubscribeToEvents(true);
        }
        
        public void GameExit()
        {
            SubscribeToEvents(false);
        }

        private void Start()
        {
            if (_grass.IsActive)
            {
                return;
            }

            _grass.transform.position = _divaTransform.position;
            _grassColorChecker.RefreshLastColor();
            _grassColorChecker.SetEnable(true);
            _tickCounter.StopWait();
            _grass.Grow();
        }

        private void Stop()
        {
            if (!_grass.IsActive)
            {
                return;
            }

            _grassColorChecker.SetEnable(false);
            _grass.Die();
            _tickCounter.StartWait();
        }

        #region Events

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _divaAnimator.OnModeEntered += OnDivaSwitchAnimation;
                _tickCounter.OnWaitIsOver += OnCooldownTick;
                _grassColorChecker.OnFoundedNewColor += OnNewColorFounded;
                _grassButton.OnPressedUp += OnGrassPressedUp;
                _divaDragAndDrop.OnEndedDrag += OnDivaEndedDrag;
            }
            else
            {
                _divaAnimator.OnModeEntered -= OnDivaSwitchAnimation;
                _tickCounter.OnWaitIsOver -= OnCooldownTick;
                _grassColorChecker.OnFoundedNewColor -= OnNewColorFounded;
                _grassButton.OnPressedUp -= OnGrassPressedUp;
                _divaDragAndDrop.OnEndedDrag -= OnDivaEndedDrag;
            }
        }

        private void OnDivaSwitchAnimation(EDivaAnimationMode mode)
        {
            if (mode == EDivaAnimationMode.Seat && _tickCounter.IsExpectedStart)
            {
                _tickCounter.StartWait(_tickDelay.GetRandomValue());
            }
            else if (_grass.IsActive)
            {
                Stop();
            }
        }

        private void OnCooldownTick()
        {
            Start();
        }

        private void OnDivaEndedDrag(float distance)
        {
            //todo переименовать поле
            distance = Vector3.Distance(_grass.transform.position, _divaTransform.position);
           
            if (distance > 0.6f)
            {
               Stop();   
            }
        }

        private void OnNewColorFounded(Color obj)
        {
            Stop();
        }

        private void OnGrassPressedUp(Vector2 arg1, float arg2)
        {
            Stop();
        }
        
        #endregion
    }
}