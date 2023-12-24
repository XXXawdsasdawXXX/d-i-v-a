using System;
using System.Collections.Generic;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Value;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Save;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Character
{
    public class CharacterLiveStateController : MonoBehaviour, IGameStartListener, IGameExitListener, IProgressWriter
    {
        private Dictionary<LiveStateKey, CharacterLiveState> _liveStates = new();
        private TimeObserver _timeObserver;


        public void GameStart()
        {
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            SubscribeToEvents(true);
        }

        public void GameExit()
        {
            SubscribeToEvents(false);
        }

        private void OnTimeObserverTick()
        {
            foreach (var liveState in _liveStates)
            {
                liveState.Value.TimeUpdate();
            }
        }


        public void LoadProgress(PlayerProgress progress)
        {
            _liveStates = progress?.LiveStatesData == null || progress.LiveStatesData.Count == 0
                ? InitNewStates()
                : LoadSavedStates(progress.LiveStatesData);
        }

        public void UpdateProgress(PlayerProgress progress)
        {
            foreach (var liveState in _liveStates)
            {
                if (progress.LiveStatesData.ContainsKey(liveState.Key))
                {
                    progress.LiveStatesData[liveState.Key] = liveState.Value.Current;
                }
                else
                {
                    progress.LiveStatesData.Add(liveState.Key, liveState.Value._current);
                }
            }
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

        private Dictionary<LiveStateKey, CharacterLiveState> InitNewStates()
        {
            var characterConfig = Container.Instance.FindConfig<CharacterConfig>(); ///nu takoe
            var characterLiveStates = new Dictionary<LiveStateKey, CharacterLiveState>();
            var liveStateCount = Enum.GetNames(typeof(LiveStateKey)).Length;

            for (int i = 1; i < liveStateCount; i++)
            {
                var stateKey = (LiveStateKey)i;
                var staticParam = characterConfig.GetStaticParam(stateKey);
                var characterLiveState = new CharacterLiveState(
                    current: staticParam.MaxValue,
                    max: staticParam.MaxValue,
                    decreasingValue: staticParam.DecreasingValue);

                characterLiveStates.Add(stateKey, characterLiveState);
            }

            Debugging.Instance.Log($"Live state -> init new", Debugging.Type.LiveState);
            return characterLiveStates;
        }

        private Dictionary<LiveStateKey, CharacterLiveState> LoadSavedStates(
            Dictionary<LiveStateKey, float> liveStateSavedData)
        {
            var characterConfig = Container.Instance.FindConfig<CharacterConfig>(); ///nu takoe
            var characterLiveStates = new Dictionary<LiveStateKey, CharacterLiveState>();

            foreach (var stateSavedData in liveStateSavedData)
            {
                var staticParam = characterConfig.GetStaticParam(stateSavedData.Key);
                var characterLiveState = new CharacterLiveState(
                    current: stateSavedData.Value,
                    max: staticParam.MaxValue,
                    decreasingValue: staticParam.DecreasingValue);

                characterLiveStates.Add(stateSavedData.Key, characterLiveState);
            }

            Debugging.Instance.Log($"Live state -> load saved", Debugging.Type.LiveState);
            return characterLiveStates;
        }

        public void LogStates()
        {
            foreach (var liveState in _liveStates)
            {
                Debugging.Instance.Log($"{liveState.Key} = {liveState.Value.Current}", Debugging.Type.LiveState);
            }
        }
    }
}