using Code.Components.Characters;
using Code.Components.Common;
using Code.Components.Entities;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Value.RangeInt;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.CustomActions
{
    public class CustomAction_Grass : CustomAction, IGameInitListener, IGameStartListener, IGameExitListener
    {
        [Header("DIVA")]
        private DIVA _diva;
        private CharacterAnimator _characterAnimator;
        
        [Header("Grass Components")]
        private Grass _grass;
        private ColorChecker _grassChecker;
        
        [Header("Action Delay")]
        private TickCounter _tickCounter;
        private RangedInt _tickDelay;

        public override CustomCutsceneActionType GetActionType() => CustomCutsceneActionType.Grass;

        public void GameInit()
        {
            _diva = Container.Instance.FindEntity<DIVA>();
            _characterAnimator = _diva.FindCharacterComponent<CharacterAnimator>();
        
            _grass = Container.Instance.FindEntity<Grass>();
            _grassChecker = _grass.FindCommonComponent<ColorChecker>();
            
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

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _characterAnimator.OnModeEntered += OnCharacterSwitchAnimationMode;
                _tickCounter.WaitedEvent += OnWaitedTickCounter;
                _grassChecker.OnFoundedNewColor += OnFoundedNewColor;
            }
            else
            {
                _characterAnimator.OnModeEntered -= OnCharacterSwitchAnimationMode;
                _tickCounter.WaitedEvent -= OnWaitedTickCounter;
                _grassChecker.OnFoundedNewColor -= OnFoundedNewColor;
            }
        }

        private void OnFoundedNewColor(Color obj)
        {
            _tickCounter.StopWait();
            _grass.Die();
        }

        private void OnCharacterSwitchAnimationMode(CharacterAnimationMode mode)
        {
            Debugging.Instance.Log($"Controller -> on swithc animation mode:  {mode} {_grass.IsActive}",
                Debugging.Type.Grass);
            if (mode == CharacterAnimationMode.Seat && _tickCounter.IsExpectedStart)
            {
                Debugging.Instance.Log($"Controller -> on swithc animation mode: start wait", Debugging.Type.Grass);
                _tickCounter.StartWait(_tickDelay.GetRandomValue());
            }
            else if (_grass.IsActive)
            {
                Debugging.Instance.Log($"Controller -> on swithc animation mode: start sie", Debugging.Type.Grass);
                _tickCounter.StopWait();
                _grass.Die();
            }
        }

        private void OnWaitedTickCounter()
        {
            if (_grass.IsActive)
            {
                return;
            }

            Debugging.Instance.Log($"Controller -> OnWaitedTickCounter", Debugging.Type.Grass);
            _grass.transform.position = _diva.transform.position;
            _grass.Grow();
        }
    }
}