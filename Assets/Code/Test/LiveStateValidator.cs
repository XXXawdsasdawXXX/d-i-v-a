using Code.Data.Enums;
using Code.Data.Storages;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Unity.VisualScripting;
using UnityEngine;

namespace Code.Test
{
    public class LiveStateValidator : MonoBehaviour, IGameInitListener
    {
        [SerializeField] private LiveStateKey _editableKey;
        [SerializeField] private float _editableValue;
        private CharacterLiveStateStorage _stateStorage;


        public void GameInit()
        {
            _stateStorage = Container.Instance.FindStorage<CharacterLiveStateStorage>();
        }


        public void ChangeState()
        {
            if (_stateStorage.TryGetCharacterLiveState(_editableKey, out var state))
            {
                state.Add(_editableValue);
            }
        }
    }
}