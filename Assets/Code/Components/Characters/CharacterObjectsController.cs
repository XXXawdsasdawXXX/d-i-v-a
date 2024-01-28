using Code.Components.Apples;
using Code.Components.Objects;
using Code.Infrastructure.GameLoop;
using UnityEditor;
using UnityEngine;

namespace Code.Components.Characters
{
    public class CharacterObjectsController : MonoBehaviour, IGameInitListener, IGameExitListener
    {
        [SerializeField] private Character _character;
        [SerializeField] private CollisionObserver _collisionObserver;
        [SerializeField] private CharacterModeAdapter _modeAdapter;

        private Apple _selectedApple;
        public void GameInit()
        {
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
            if (_selectedApple != null && obj.TryGetComponent(out Apple item))
            { 
                _character.Animator.StopPlayEat();
            }
        }


        private void OnEnterEvent(GameObject obj)
        {
            if (obj.TryGetComponent(out Apple item))
            { 
               item.transform.position = _modeAdapter.GetWorldEatPoint();
               _character.Animator.StartPlayEat();
               item.Use(OnEnd: () => _character.Animator.StopPlayEat());
            }
        }
    }
}