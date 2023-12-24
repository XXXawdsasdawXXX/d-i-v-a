using System;
using System.Collections.Generic;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Value;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using UnityEngine;

namespace Code.Components.Character
{
    public class CharacterLiveStateController : MonoBehaviour, IGameStartListener, IGameExitListener
    {
       [SerializeField] private List<CharacterLiveState> _liveStates = new();
        private TimeObserver _timeObserver;


        public void GameStart()
        {
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            _liveStates = InitNewStates();
            
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
                _timeObserver.TickEvent += OnTimeObserverTick;
            }
            else
            {
                _timeObserver.TickEvent -= OnTimeObserverTick;
                
            }
        }

        private void OnTimeObserverTick()
        {
            foreach (var liveState in _liveStates)
            {
                liveState.TimeUpdate();
            }
        }


        private List<CharacterLiveState> InitNewStates()
        {
            List<CharacterLiveState> list = new List<CharacterLiveState>();
            var liveStateCount = Enum.GetNames(typeof(LiveStateKey)).Length;
            var characterConfig = Container.Instance.FindConfig<CharacterConfig>();

            for (int i = 1; i < liveStateCount; i++)
            {
                var stateKey = (LiveStateKey)i;
                var staticParam = characterConfig.GetStaticParam(stateKey);
                var characterLiveState = new CharacterLiveState(
                    key: stateKey,
                    current: staticParam.MaxValue,
                    max: staticParam.MaxValue,
                    decreasingValue: staticParam.DecreasingValue);

                list.Add(characterLiveState);
            }

            return list;
        }

        private List<CharacterLiveState> LoadStates(List<LiveStateSavedData> liveStateSavedData)
        {
            List<CharacterLiveState> list = new List<CharacterLiveState>();
            var characterConfig = Container.Instance.FindConfig<CharacterConfig>(); ///nu takoe
            foreach (var stateSavedData in liveStateSavedData)
            {
                var staticParam = characterConfig.GetStaticParam(stateSavedData.Key);
                var characterLiveState = new CharacterLiveState(
                    key: stateSavedData.Key,
                    current: stateSavedData.CurrentValue,
                    max: staticParam.MaxValue,
                    decreasingValue: staticParam.DecreasingValue);

                list.Add(characterLiveState);
            }

            return list;
        }


        /*public void LoadProgress(Progress progress)
        {
            if (progress == null)
            {
                Debugging.Instance.Log($"CharacterLiveState: ");
            }
            else
            {
                
            }
        }

        public void UpdateProgress(Progress progress)
        {
            progress.LiveStatesData = LiveStates.Select(s => s.GetCurrentData()).ToList();
        }*/
    }
}