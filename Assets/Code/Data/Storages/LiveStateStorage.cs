using System;
using System.Collections.Generic;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Interfaces;
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
        public void AddPercentageValues(LiveStateValue[] values)
        {
            foreach (var liveStateValue in values)
            {
                if (TryGetLiveState(liveStateValue.Key, out var state))
                {
                    state.Add(state.Max / 100 * liveStateValue.Value);
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values">value from 0 to 1</param>
        public void AddPercentageValue(LiveStateValue value)
        {
            if (TryGetLiveState(value.Key, out var state))
            {
                state.Add(state.Max / (100 * value.Value));
            }
        }

        #region Initialize

        public void LoadProgress(PlayerProgress progress)
        {
            LiveStates = progress?.LiveStatesData == null || progress.LiveStatesData.Count == 0
                ? InitNewStates()
                : LoadSavedStates(progress.LiveStatesData);
            
            Debugging.Instance.Log($"load init count {LiveStates.Count} { progress.LiveStatesData.Count }", Debugging.Type.LiveState);
        }

        public void UpdateProgress(PlayerProgress progress)
        {
            foreach (var liveState in LiveStates)
            {
                if (progress.LiveStatesData.ContainsKey(liveState.Key))
                {
                    progress.LiveStatesData[liveState.Key] = liveState.Value.Current;
                }
                else
                {
                    progress.LiveStatesData.Add(liveState.Key, liveState.Value.Current);
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
                var state =  CreateNewState(
                    stateKey: (LiveStateKey)i,
                    currentIsMaxValue:true);
                characterLiveStates.Add( (LiveStateKey)i,state);
            }

            Debugging.Instance.Log($"init new", Debugging.Type.LiveState);
            return characterLiveStates;
        }

        private Dictionary<LiveStateKey, CharacterLiveState> LoadSavedStates(Dictionary<LiveStateKey, float> liveStateSavedData)
        {
            var characterConfig = Container.Instance.FindConfig<LiveStateConfig>(); 
            var characterLiveStates = new Dictionary<LiveStateKey, CharacterLiveState>();

            foreach (var stateSavedData in liveStateSavedData)
            {
               var state =  CreateNewState(stateKey:
                    stateSavedData.Key,
                    currentIsMaxValue: false,
                    currentValue:stateSavedData.Value);
                characterLiveStates.Add(stateSavedData.Key,state);
            }

            Debugging.Instance.Log($"load saved", Debugging.Type.LiveState);
            return characterLiveStates;
        }

        private  CharacterLiveState CreateNewState(LiveStateKey stateKey, bool currentIsMaxValue, float currentValue = 0)
        {
            var staticParam = _liveStateConfig.GetStaticParam(stateKey);
            var characterLiveState = new CharacterLiveState(
                current: currentIsMaxValue ? staticParam.MaxValue: currentValue,
                max: staticParam.MaxValue,
                decreasingValue: staticParam.DecreasingValue,
                healValue: staticParam.HealValue);

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