using System;
using System.Collections.Generic;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Data.Value;
using Code.Infrastructure.DI;
using Code.Infrastructure.Save;
using Code.Utils;

namespace Code.Data.Storages
{
    public class CharacterLiveStateStorage : IStorage, IProgressWriter
    {
        public Dictionary<LiveStateKey, CharacterLiveState> LiveStates { get; private set; } = new();

        public void AddValues(LiveStateValue[] value)
        {
            foreach (var liveStateValue in value)
            {
                if (TryGetCharacterLiveState(liveStateValue.Key, out var state))
                {
                    state.Add(liveStateValue.Value);
                }
            }
        }
        public bool TryGetCharacterLiveState(LiveStateKey key, out CharacterLiveState liveState)
        {
            if (LiveStates.ContainsKey(key))
            {
                liveState = LiveStates[key];
                return true;
            }

            liveState = null;
            return false;
        }

        #region Initialize

        public void LoadProgress(PlayerProgress progress)
        {
            LiveStates = progress?.LiveStatesData == null || progress.LiveStatesData.Count == 0
                ? InitNewStates()
                : LoadSavedStates(progress.LiveStatesData);
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
            var characterConfig = Container.Instance.FindConfig<CharacterConfig>();
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

        public void AddPercentageValues(LiveStateValue[] values)
        {
            foreach (var liveStateValue in values)
            {
                if (TryGetCharacterLiveState(liveStateValue.Key, out var state))
                {
                    state.Add(state.Max / 100 * liveStateValue.Value);
                }
            }
        }
    }
}