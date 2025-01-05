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
        public Dictionary<ELiveStateKey, CharacterLiveState> LiveStates { get; private set; } = new();
        public event Action OnInit;

        public void GameInit()
        {
            _liveStateConfig = Container.Instance.FindConfig<LiveStateConfig>();
        }

        public bool IsEmptyState(ELiveStateKey key)
        {
            if (TryGetLiveState(key, out CharacterLiveState state))
            {
                return state.Current <= 0;
            }

            return false;
        }

        public bool TryGetLiveState(ELiveStateKey key, out CharacterLiveState liveState)
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
                    _addPercent(state, liveStateValue.Value);
                }
            }
        }


        public void AddPercentageValue(LiveStatePercentageValue value)
        {
            if (TryGetLiveState(value.Key, out CharacterLiveState state))
            {
                _addPercent(state, value.Value);
            }
        }


        public void AddPercentageValue(LiveStateRangePercentageValue value)
        {
            if (TryGetLiveState(value.Key, out CharacterLiveState state))
            {
                float randomValue = value.PercentageValue.GetRandomValue();
                _addPercent(state, randomValue);
            }
        }

        private void _addPercent(CharacterLiveState state, float value)
        {
            float max = state.Max;
          
            float result = max / 100 * _getCorrectValue(value);
            
            Debugging.Instance.Log(this, $"[_addPercent] " +
                                         $"{max} / 100 * `{value}` = {result} " +
                                         $"| state value = {state.Current}", Debugging.Type.LiveState);
      
            state.Add(result);
        }

        private float _getCorrectValue(float value)
        {
            if (Math.Abs(value) <= 1)
            {
                return value * 100;
            }

            return value;
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
            foreach (KeyValuePair<ELiveStateKey, CharacterLiveState> liveState in LiveStates)
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

        private Dictionary<ELiveStateKey, CharacterLiveState> InitNewStates()
        {
            LiveStateConfig characterConfig = Container.Instance.FindConfig<LiveStateConfig>();
            Dictionary<ELiveStateKey, CharacterLiveState> characterLiveStates = new Dictionary<ELiveStateKey, CharacterLiveState>();
            int liveStateCount = Enum.GetNames(typeof(ELiveStateKey)).Length;

            for (int i = 1; i < liveStateCount; i++)
            {
                CharacterLiveState state = CreateNewState(
                    stateKey: (ELiveStateKey)i,
                    currentIsMaxValue: true);
                characterLiveStates.Add((ELiveStateKey)i, state);
            }

            Debugging.Instance.Log(this, $"init new", Debugging.Type.LiveState);
            return characterLiveStates;
        }

        private Dictionary<ELiveStateKey, CharacterLiveState> LoadSavedStates(List<LiveStateSavedData> list)
        {
            LiveStateConfig characterConfig = Container.Instance.FindConfig<LiveStateConfig>();
            Dictionary<ELiveStateKey, CharacterLiveState> characterLiveStates = new Dictionary<ELiveStateKey, CharacterLiveState>();

            foreach (LiveStateSavedData stateSavedData in list)
            {
                CharacterLiveState state = CreateNewState(stateKey:
                    stateSavedData.Key,
                    currentIsMaxValue: false,
                    currentValue: stateSavedData.CurrentValue,
                    isHealing: stateSavedData.Key != ELiveStateKey.Sleep && stateSavedData.IsHealing);
                characterLiveStates.Add(stateSavedData.Key, state);
            }

            Debugging.Instance.Log(this, $"load states", Debugging.Type.LiveState);
            return characterLiveStates;
        }

        private CharacterLiveState CreateNewState(ELiveStateKey stateKey, bool currentIsMaxValue, float currentValue = 0,
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
            foreach (KeyValuePair<ELiveStateKey, CharacterLiveState> liveState in LiveStates)
            {
                Debugging.Instance.Log(this, $"{liveState.Key} = {liveState.Value.Current}", Debugging.Type.LiveState);
            }
        }

        #endregion
    }
}