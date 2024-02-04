using Code.Data.Enums;
using Code.Data.Storages;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Test
{
    public class LiveStateValidator : MonoBehaviour, IGameInitListener
    {
        [SerializeField] private LiveStateKey _editableKey;
        [SerializeField] private float _editableValue;

        private LiveStateStorage _stateStorage;
        
        public void GameInit()
        {
            _stateStorage = Container.Instance.FindStorage<LiveStateStorage>();
        }
        
        public void ChangeState()
        {
            if (_stateStorage.TryGetLiveState(_editableKey, out var state))
            {
                state.Add(_editableValue);
            }
        }
    }
}