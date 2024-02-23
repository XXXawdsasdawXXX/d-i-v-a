using System;
using System.Collections.Generic;
using System.Linq;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Data.SavedData;
using Code.Data.Value;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Save;
using Code.Utils;

namespace Code.Data.Storages
{
    public class LiveStateStorage : IStorage, IGameInitListener, IProgressWriter
    {
        private LiveStateConfig _liveStateConfig;
        public Dictionary<LiveStateKey, CharacterLiveState> LiveStates { get; private set; } = new();

        public void GameInit()
        {
            _liveStateConfig = Container.Instance.FindConfig<LiveStateConfig>();
        }


        public bool IsEmptyState(LiveStateKey key)
        {
            if (TryGetLiveState(key, out var state))
            {
                return state.Current <= 0;
            }

            return false;
        }
        
        public bool TryGetLiveState(LiveStateKey key, out CharacterLiveState liveState)
        {
            if (LiveStates.ContainsKey(key))
            {
                liveState = LiveStates[key];
                return true;
            }

            liveState = null;
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values">value from 0 to 1</param>
        public void AddPercentageValues(LiveStatePercentageValue[] values)
        {
            foreach (var liveStateValue in values)
            {
                if (TryGetLiveState(liveStateValue.Key, out var state))
                {
                    Add(state, liveStateValue.Value);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values">value from -1 to 1</param>
        public void AddPercentageValue(LiveStatePercentageValue percentageValue)
        {
            if (TryGetLiveState(percentageValue.Key, out var state))
            {
                Add(state, percentageValue.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values">value from -1 to 1</param>
        public void AddPercentageValue(LiveStateRangePercentageValue percentageValue)
        {
            if (TryGetLiveState(percentageValue.Key, out var state))
            {
                var randomValue = percentageValue.PercentageValue.GetRandomValue();
                Add(state, randomValue);
            }
        }

        private void Add(CharacterLiveState state, float randomValue)
        {
            var max = state.Max;
            var result = max / 100 * GetCorrectValue(randomValue);
            Debugging.Instance.Log($"{max} / 100 * {randomValue} = {result}", Debugging.Type.LiveState);
            state.Add(result);
        }

        private float GetCorrectValue(float normalizedValue)
        {
            if (Math.Abs(normalizedValue) <= 1)
            {
                return normalizedValue * 100;
            }

            return normalizedValue;
        }

        #region Initialize

        public void LoadProgress(PlayerProgressData playerProgress)
        {
            LiveStates = playerProgress?.LiveStatesData == null || playerProgress.LiveStatesData.Count == 0
                ? InitNewStates()
                : LoadSavedStates(playerProgress.LiveStatesData);

            Debugging.Instance.Log($"load init count {LiveStates.Count} {playerProgress.LiveStatesData.Count}",
                Debugging.Type.LiveState);
        }

        public void UpdateProgress(PlayerProgressData playerProgress)
        {
            foreach (var liveState in LiveStates)
            {
                var savedState = playerProgress.LiveStatesData.FirstOrDefault(l => l.Key == liveState.Key);
                
                if (savedState != null)
                {
                    savedState.CurrentValue = liveState.Value.Current;
                    savedState.IsHealing = liveState.Value.IsHealing;
                }
                else
                {
                    playerProgress.LiveStatesData.Add(new LiveStateSavedData()
                    {
                        Key = liveState.Key,
                        CurrentValue = liveState.Value.Current,
                        IsHealing = liveState.Value.IsHealing
                    });
                }
            }
        }

        private Dictionary<LiveStateKey, CharacterLiveState> InitNewStates()
        {
            var characterConfig = Container.Instance.FindConfig<LiveStateConfig>();
            var characterLiveStates = new Dictionary<LiveStateKey, CharacterLiveState>();
            var liveStateCount = Enum.GetNames(typeof(LiveStateKey)).Length;

            for (int i = 1; i < liveStateCount; i++)
            {
                var state = CreateNewState(
                    stateKey: (LiveStateKey)i,
                    currentIsMaxValue: true);
                characterLiveStates.Add((LiveStateKey)i, state);
            }

            Debugging.Instance.Log($"init new", Debugging.Type.LiveState);
            return characterLiveStates;
        }

        private Dictionary<LiveStateKey, CharacterLiveState> LoadSavedStates(List<LiveStateSavedData> list)
        {
            var characterConfig = Container.Instance.FindConfig<LiveStateConfig>();
            var characterLiveStates = new Dictionary<LiveStateKey, CharacterLiveState>();

            foreach (var stateSavedData in list)
            {
                var state = CreateNewState(stateKey:
                    stateSavedData.Key,
                    currentIsMaxValue: false,
                    currentValue: stateSavedData.CurrentValue,
                    isHealing: stateSavedData.IsHealing);
                characterLiveStates.Add(stateSavedData.Key, state);
            }

            Debugging.Instance.Log($"load states", Debugging.Type.LiveState);
            return characterLiveStates;
        }

        private CharacterLiveState CreateNewState(LiveStateKey stateKey, bool currentIsMaxValue, float currentValue = 0, bool isHealing = false)
        {
            var staticParam = _liveStateConfig.GetStaticParam(stateKey);
            var characterLiveState = new CharacterLiveState(
                current: currentIsMaxValue ? staticParam.MaxValue : currentValue,
                max: staticParam.MaxValue,
                decreasingValue: staticParam.DecreasingValue,
                healValue: staticParam.HealValue,
                isHealing : isHealing);

            return characterLiveState;
        }

        #endregion

        #region Editor

        public void LogStates()
        {
            foreach (var liveState in LiveStates)
            {
                Debugging.Instance.Log($"{liveState.Key} = {liveState.Value.Current}", Debugging.Type.LiveState);
            }
        }

        #endregion
    }
}