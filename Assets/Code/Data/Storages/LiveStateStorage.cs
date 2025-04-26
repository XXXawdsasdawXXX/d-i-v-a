using System;
using System.Collections.Generic;
using System.Linq;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Save;
using Code.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;

namespace Code.Data
{
    [Preserve]
    public class LiveStateStorage : IStorage, IInitListener, IProgressWriter
    {
        public Dictionary<ELiveStateKey, CharacterLiveState> LiveStates { get; private set; } = new();

        private LiveStateConfig _liveStateConfig;
        
        public UniTask GameInitialize()
        {
            _liveStateConfig = Container.Instance.FindConfig<LiveStateConfig>();
            
            return UniTask.CompletedTask;
        }
        
        public UniTask LoadProgress(PlayerProgressData playerProgress)
        {
            LiveStates = playerProgress?.LiveStatesData == null || playerProgress.LiveStatesData.Count == 0
                ? _initNewStates()
                : LoadSavedStates(playerProgress.LiveStatesData);
            
            return UniTask.CompletedTask;
        }

        public void SaveProgress(PlayerProgressData playerProgress)
        {
            foreach (KeyValuePair<ELiveStateKey, CharacterLiveState> liveState in LiveStates)
            {
                LiveStateSavedData savedState =
                    playerProgress.LiveStatesData.FirstOrDefault(l => l.Key == liveState.Key);

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
        
        public void AddPercentageValues(LiveStatePercentageValue[] values)
        {
            if (values == null)
            {
#if DEBUGGING
                Log.Info(this, "[AddPercentageValues] can not add null values", Log.Type.LiveState);
#endif
                return;
            }

            foreach (LiveStatePercentageValue liveStateValue in values)
            {
                if (TryGetLiveState(liveStateValue.Key, out CharacterLiveState state))
                {
#if DEBUGGING
                    Log.Info(this, $"[AddPercentageValues] add {liveStateValue.Key} {liveStateValue.Value}",
                        Log.Type.LiveState);
#endif
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
#if DEBUGGING

            Log.Info(this, $"[_addPercent] {max} / 100 * `{value}` = {result} | state value = {state.Current}",
                Log.Type.LiveState);
#endif

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

    
        private Dictionary<ELiveStateKey, CharacterLiveState> _initNewStates()
        {
            Dictionary<ELiveStateKey, CharacterLiveState> characterLiveStates = new();

            int liveStateCount = Enum.GetNames(typeof(ELiveStateKey)).Length;

            for (int i = 1; i < liveStateCount; i++)
            {
                CharacterLiveState state = CreateNewState(
                    stateKey: (ELiveStateKey)i,
                    currentIsMaxValue: true);
                characterLiveStates.Add((ELiveStateKey)i, state);
            }

#if DEBUGGING
            Log.Info(this, "[_init new states]", Log.Type.LiveState);
#endif

            return characterLiveStates;
        }

        private Dictionary<ELiveStateKey, CharacterLiveState> LoadSavedStates(List<LiveStateSavedData> list)
        {
            Dictionary<ELiveStateKey, CharacterLiveState> characterLiveStates = new();

            foreach (LiveStateSavedData stateSavedData in list)
            {
                CharacterLiveState state = CreateNewState(stateKey:
                    stateSavedData.Key,
                    currentIsMaxValue: false,
                    currentValue: stateSavedData.CurrentValue,
                    isHealing: stateSavedData.Key != ELiveStateKey.Sleep && stateSavedData.IsHealing);
                characterLiveStates.Add(stateSavedData.Key, state);
            }

#if DEBUGGING
            Log.Info(this, $"[_load states]", Log.Type.LiveState);
#endif

            return characterLiveStates;
        }

        private CharacterLiveState CreateNewState(ELiveStateKey stateKey, bool currentIsMaxValue,
            float currentValue = 0,
            bool isHealing = false)
        {
            LiveStateStaticParam staticParam = _liveStateConfig.GetStaticParam(stateKey);

            CharacterLiveState characterLiveState = new(
                current: currentIsMaxValue ? staticParam.MaxValue : currentValue,
                max: staticParam.MaxValue,
                decreasingValue: staticParam.DecreasingValue,
                healValue: staticParam.HealValue,
                isHealing: isHealing);

            return characterLiveState;
        }

    }
}