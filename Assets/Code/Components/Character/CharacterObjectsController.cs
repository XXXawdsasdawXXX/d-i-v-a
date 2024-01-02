using Code.Components.Objects;
using Code.Data.Storages;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components
{
    public class CharacterObjectsController : MonoBehaviour, IGameInitListener, IGameExitListener
    {
        [SerializeField] private CollisionObserver _collisionObserver;
        private CharacterLiveStateStorage _liveStateStorage;
        public void GameInit()
        {
            _liveStateStorage = Container.Instance.FindStorage<CharacterLiveStateStorage>();
            SubscribeToEvents(true);    
        }

        public void GameExit()
        {
            SubscribeToEvents(false);    
            
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _collisionObserver.EnterEvent += OnEnterEvent;
                _collisionObserver.ExitEvent += OnExitEvent;
            }
            else
            {
                
                _collisionObserver.EnterEvent -= OnEnterEvent;
                _collisionObserver.ExitEvent -= OnExitEvent;
            }
        }

        private void OnExitEvent(GameObject obj)
        {
            throw new System.NotImplementedException();
        }

        private void OnEnterEvent(GameObject obj)
        {
            if (obj.TryGetComponent(out Apple item) )
            {
               item.Use();
            }

        }
    }
}