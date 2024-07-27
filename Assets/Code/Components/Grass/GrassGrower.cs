using Code.Components.Characters;
using Code.Components.Entities;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Value.RangeInt;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Grass
{
    public class GrassGrower : MonoBehaviour, IGameInitListener
    {
        private Entities.Grass _grass;
        
        private CharacterAnimator _characterAnimator;
        private DIVA _diva;

        private TickCounter _tickCounter;
        private RangedInt _tickDelay;

        public void GameInit()
        {
            _grass = Container.Instance.FindEntity<Entities.Grass>();
            _diva = Container.Instance.FindEntity<DIVA>();
            _characterAnimator = _diva.FindCharacterComponent<CharacterAnimator>();
            
            _tickCounter = new TickCounter(isLoop: false);
            _tickDelay = Container.Instance.FindConfig<TimeConfig>().Delay.GrassGrow;
            
            SubscribeToEvents(true);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _characterAnimator.ModeEnteredEvent += OnCharacterSwitchAnimationMode;
                _tickCounter.WaitedEvent += OnWaitedTickCounter;
            }
            else
            {
                _characterAnimator.ModeEnteredEvent -= OnCharacterSwitchAnimationMode;
                _tickCounter.WaitedEvent -= OnWaitedTickCounter;
            }
        }

        private void OnCharacterSwitchAnimationMode(CharacterAnimationMode mode)
        {
            Debugging.Instance.Log($"Controller -> on swithc animation mode:  {mode} {_grass.IsActive}",Debugging.Type.Grass);
            if (mode == CharacterAnimationMode.Seat && _tickCounter.IsExpectedStart)
            {
                Debugging.Instance.Log($"Controller -> on swithc animation mode: start wait",Debugging.Type.Grass);
               _tickCounter.StartWait(_tickDelay.GetRandomValue());
            }
            else if (_grass.IsActive)
            {
                 Debugging.Instance.Log($"Controller -> on swithc animation mode: start sie",Debugging.Type.Grass);
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
            Debugging.Instance.Log($"Controller -> OnWaitedTickCounter",Debugging.Type.Grass);
            _grass.transform.position = _diva.transform.position;
            _grass.Grow();
        }
    }
}