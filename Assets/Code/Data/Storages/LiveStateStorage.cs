using System;
using System.Collections.Generic;
using System.Linq;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Data.SavedData;
using Code.Data.StaticData;
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
        public event Action OnInit;

        public void GameInit()
        {
            _liveStateConfig = Container.Instance.FindConfig<LiveStateConfig>();
        }

        public bool IsEmptyState(LiveStateKey key)
        {
            if (TryGetLiveState(key, out CharacterLiveState state))
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
            if (values == null)
            {
                Debugging.Instance.Log(this, $"[AddPercentageValues] can not add null values",
                    Debugging.Type.LiveState);
                return;
            }

            foreach (LiveStatePercentageValue liveStateValue in values)
            {
                if (TryGetLiveState(liveStateValue.Key, out CharacterLiveState state))
                {
                    Debugging.Instance.Log(this,
                        $"[AddPercentageValues] add {liveStateValue.Key} {liveStateValue.Value}",
                        Debugging.Type.LiveState);
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
            if (TryGetLiveState(percentageValue.Key, out CharacterLiveState state))
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
            if (TryGetLiveState(percentageValue.Key, out CharacterLiveState state))
            {
                float randomValue = percentageValue.PercentageValue.GetRandomValue();
                Add(state, randomValue);
            }
        }

        private void Add(CharacterLiveState state, float randomValue)
        {
            float max = state.Max;
            float result = max / 100 * GetCorrectValue(randomValue);
            Debugging.Instance.Log(this, $"[Add] {max} / 100 * `{randomValue}` = {result}", Debugging.Type.LiveState);
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

            OnInit?.Invoke();

            Debugging.Instance.Log(this, $"load init count {LiveStates.Count} {playerProgress.LiveStatesData.Count}",
                Debugging.Type.LiveState);
        }

        public void UpdateProgress(PlayerProgressData playerProgress)
        {
            foreach (KeyValuePair<LiveStateKey, CharacterLiveState> liveState in LiveStates)
            {
                LiveStateSavedData savedState = playerProgress.LiveStatesData.FirstOrDefault(l => l.Key == liveState.Key);

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
            LiveStateConfig characterConfig = Container.Instance.FindConfig<LiveStateConfig>();
            Dictionary<LiveStateKey, CharacterLiveState> characterLiveStates = new Dictionary<LiveStateKey, CharacterLiveState>();
            int liveStateCount = Enum.GetNames(typeof(LiveStateKey)).Length;

            for (int i = 1; i < liveStateCount; i++)
            {
                CharacterLiveState state = CreateNewState(
                    stateKey: (LiveStateKey)i,
                    currentIsMaxValue: true);
                characterLiveStates.Add((LiveStateKey)i, state);
            }

            Debugging.Instance.Log(this, $"init new", Debugging.Type.LiveState);
            return characterLiveStates;
        }

        private Dictionary<LiveStateKey, CharacterLiveState> LoadSavedStates(List<LiveStateSavedData> list)
        {
            LiveStateConfig characterConfig = Container.Instance.FindConfig<LiveStateConfig>();
            Dictionary<LiveStateKey, CharacterLiveState> characterLiveStates = new Dictionary<LiveStateKey, CharacterLiveState>();

            foreach (LiveStateSavedData stateSavedData in list)
            {
                CharacterLiveState state = CreateNewState(stateKey:
                    stateSavedData.Key,
                    currentIsMaxValue: false,
                    currentValue: stateSavedData.CurrentValue,
                    isHealing: stateSavedData.Key != LiveStateKey.Sleep && stateSavedData.IsHealing);
                characterLiveStates.Add(stateSavedData.Key, state);
            }

            Debugging.Instance.Log(this, $"load states", Debugging.Type.LiveState);
            return characterLiveStates;
        }

        private CharacterLiveState CreateNewState(LiveStateKey stateKey, bool currentIsMaxValue, float currentValue = 0,
            bool isHealing = false)
        {
            LiveStateStaticParam staticParam = _liveStateConfig.GetStaticParam(stateKey);
            CharacterLiveState characterLiveState = new CharacterLiveState(
                current: currentIsMaxValue ? staticParam.MaxValue : currentValue,
                max: staticParam.MaxValue,
                decreasingValue: staticParam.DecreasingValue,
                healValue: staticParam.HealValue,
                isHealing: isHealing);

            return characterLiveState;
        }

        #endregion

        #region Editor

        public void LogStates()
        {
            foreach (KeyValuePair<LiveStateKey, CharacterLiveState> liveState in LiveStates)
            {
                Debugging.Instance.Log(this, $"{liveState.Key} = {liveState.Value.Current}", Debugging.Type.LiveState);
            }
        }

        #endregion
    }
}