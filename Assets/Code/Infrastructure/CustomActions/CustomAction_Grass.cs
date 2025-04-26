using System;
using Code.Data;
using Code.Entities.Common;
using Code.Entities.Diva;
using Code.Entities.Grass;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace Code.Infrastructure.CustomActions
{
    [Preserve]
    public class CustomAction_Grass : CustomAction, IInitListener, ISubscriber
    {
        [Header("d i v a")] private Transform _divaTransform;
        private DivaAnimator _divaAnimator;
        private PhysicsDragAndDrop _divaDragAndDrop;

        [Header("Grass Components")] private GrassEntity _grass;
        private ColorChecker _grassColorChecker;

        [Header("Action Delay")] private TickCounter _tickCounter;
        private ColliderButton _grassButton;

        [Header("Static value")] private RangedInt _tickDelay;

        [Header("Dynamic value")] private Coroutine _coroutine;

        public override ECustomCutsceneActionType GetActionType() => ECustomCutsceneActionType.Grass;

        public UniTask GameInitialize()
        {
            DivaEntity diva = Container.Instance.FindEntity<DivaEntity>();
            _divaTransform = diva.transform;
            _divaAnimator = diva.FindCharacterComponent<DivaAnimator>();
            _divaDragAndDrop = diva.FindCommonComponent<PhysicsDragAndDrop>();

            _grass = Container.Instance.FindEntity<GrassEntity>();
            _grassColorChecker = _grass.FindCommonComponent<ColorChecker>();
            _grassButton = _grass.FindCommonComponent<ColliderButton>();

            _tickCounter = new TickCounter(isLoop: false);
            _tickDelay = Container.Instance.FindConfig<TimeConfig>().Delay.GrassGrow;

            return UniTask.CompletedTask;
        }

        public void Subscribe()
        {
            _divaAnimator.OnModeEntered += _onDivaSwitchAnimation;
            _tickCounter.OnWaitIsOver += _onCooldownTick;
            _grassColorChecker.OnFoundedNewColor += OnNewColorFounded;
            _grassButton.OnPressedUp += OnGrassPressedUp;
            _divaDragAndDrop.OnEndedDrag += _onDivaEndedDrag;
        }

        public void Unsubscribe()
        {
            _divaAnimator.OnModeEntered -= _onDivaSwitchAnimation;
            _tickCounter.OnWaitIsOver -= _onCooldownTick;
            _grassColorChecker.OnFoundedNewColor -= OnNewColorFounded;
            _grassButton.OnPressedUp -= OnGrassPressedUp;
            _divaDragAndDrop.OnEndedDrag -= _onDivaEndedDrag;
        }
        
        private void _start()
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

        private void _stop()
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
        
        private void _onDivaSwitchAnimation(EDivaAnimationMode mode)
        {
            if (mode == EDivaAnimationMode.Seat && _tickCounter.IsExpectedStart)
            {
                _tickCounter.StartWait(_tickDelay.GetRandomValue());
            }
            else if (_grass.IsActive)
            {
                _stop();
            }
        }

        private void _onCooldownTick()
        {
            _start();
        }

        private void _onDivaEndedDrag(float distance)
        {
            if (distance <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(distance));
            }
            
            distance = Vector3.Distance(_grass.transform.position, _divaTransform.position);

            if (distance > 0.6f)
            {
                _stop();
            }
        }

        private void OnNewColorFounded(Color obj)
        {
            _stop();
        }

        private void OnGrassPressedUp(Vector2 arg1, float arg2)
        {
            _stop();
        }

        #endregion
    }
}