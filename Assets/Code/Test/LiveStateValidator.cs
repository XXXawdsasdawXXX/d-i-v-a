using Code.Components.Entities.Characters;
using Code.Data.Enums;
using Code.Data.Storages;
using Code.Data.Value;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using UnityEngine;

namespace Code.Test
{
    public class LiveStateValidator : MonoBehaviour, IGameInitListener
    {
        [SerializeField] private LiveStateKey _editableKey;
        [SerializeField] private float _editableValue;

        private LiveStateStorage _stateStorage;
        private CharacterLiveStatesAnalytic _stateAnalytic;

        public void GameInit()
        {
            _stateAnalytic = Container.Instance.FindEntity<DIVA>()
                .FindCharacterComponent<CharacterLiveStatesAnalytic>();
            _stateStorage = Container.Instance.FindStorage<LiveStateStorage>();
        }

        public void ChangeState()
        {
            if (_stateStorage.TryGetLiveState(_editableKey, out CharacterLiveState state))
            {
                state.Add(_editableValue);
            }
        }

        public void DebugLowerState()
        {
            Debugging.Instance.Log($"Lower key: {_stateAnalytic.CurrentLowerLiveStateKey}", Debugging.Type.LiveState);
        }
    }
}