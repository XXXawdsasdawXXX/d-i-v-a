using Code.Components.Characters;
using Code.Components.Common;
using Code.Components.Grass;
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
        private ColorChecker _colorChecker;
        
        [Header("Action Delay")]
        private TickCounter _tickCounter;
        private RangedInt _tickDelay;

        public override CustomCutsceneActionType GetActionType() => CustomCutsceneActionType.Grass;

        public void GameInit()
        {
            _diva = Container.Instance.FindEntity<DIVA>();
            _characterAnimator = _diva.FindCharacterComponent<CharacterAnimator>();
        
            _grass = Container.Instance.FindEntity<Grass>();
            _colorChecker = _grass.FindCommonComponent<ColorChecker>();
            
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
                _tickCounter.WaitedEvent += OnTick;
                _colorChecker.OnFoundedNewColor += OnFoundedNewColor;
            }
            else
            {
                _characterAnimator.OnModeEntered -= OnCharacterSwitchAnimationMode;
                _tickCounter.WaitedEvent -= OnTick;
                _colorChecker.OnFoundedNewColor -= OnFoundedNewColor;
            }
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
               
                Stop();
            }
        }

        private void OnFoundedNewColor(Color obj)
        {
             Stop();
        }

        private void OnTick()
        {
            if (_grass.IsActive)
            {
                return;
            }

            Debugging.Instance.Log($"Controller -> OnWaitedTickCounter", Debugging.Type.Grass);
            Start();
        }

        private void Start()
        { 
            _grass.transform.position = _diva.transform.position;
            _colorChecker.RefreshLastColor();
            _colorChecker.SetEnable(true);
            _grass.Grow();
        }

        private void Stop()
        {
            _tickCounter.StopWait();
            _colorChecker.SetEnable(false);
            _grass.Die();
        }
    }
}