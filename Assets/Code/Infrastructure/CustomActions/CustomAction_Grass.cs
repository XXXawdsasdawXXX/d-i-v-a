using Code.Components.Common;
using Code.Components.Entities.Characters;
using Code.Components.Entities.Grass;
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
        private ColorChecker _grassColorChecker;
        
        [Header("Action Delay")]
        private TickCounter _tickCounter;
        private RangedInt _tickDelay;
        private ColliderButton _grassButton;

        public override CustomCutsceneActionType GetActionType() => CustomCutsceneActionType.Grass;

        public void GameInit()
        {
            _diva = Container.Instance.FindEntity<DIVA>();
            _characterAnimator = _diva.FindCharacterComponent<CharacterAnimator>();
        
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

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _characterAnimator.OnModeEntered += OnCharacterSwitchAnimation;
                _tickCounter.WaitedEvent += OnCooldownTick;
                _grassColorChecker.OnFoundedNewColor += OnNewColorFounded;
                _grassButton.OnPressedUp += OnGrassPressedUp;
            }
            else
            {
                _characterAnimator.OnModeEntered -= OnCharacterSwitchAnimation;
                _tickCounter.WaitedEvent -= OnCooldownTick;
                _grassColorChecker.OnFoundedNewColor -= OnNewColorFounded;
            }
        }

        private void OnCharacterSwitchAnimation(CharacterAnimationMode mode)
        {
            if (mode == CharacterAnimationMode.Seat && _tickCounter.IsExpectedStart)
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

        private void OnNewColorFounded(Color obj)
        {
            Stop();
        }

        private void OnGrassPressedUp(Vector2 arg1, float arg2)
        {
            Stop();
        }

        private void Start()
        { 
            if (_grass.IsActive)
            {
                return;
            }
            _grass.transform.position = _diva.transform.position;
            _grassColorChecker.RefreshLastColor();
            _grassColorChecker.SetEnable(true);
            _grass.Grow();
        }

        private void Stop()
        {
            if (!_grass.IsActive)
            {
                return;
            }
            _tickCounter.StopWait();
            _grassColorChecker.SetEnable(false);
            _grass.Die();
        }
    }
}