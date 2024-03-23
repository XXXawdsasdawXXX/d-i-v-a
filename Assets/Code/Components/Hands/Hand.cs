using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components.Hands
{
    public class Hand : MonoBehaviour, IGameInitListener
    {
        [SerializeField] private HandAnimator _handAnimator;
        public void GameInit()
        {
            
        }

        public void Enter()
        {
            _handAnimator.PlayEnter();
        }

        public void Exit_Interaction()
        {
            _handAnimator.PlayExit();
        }

        public void Exit_Time()
        {
            _handAnimator.PlayExit();
        }

        public void Exit()
        {
            
        }

        public void SetMode()//todo enum
        {
            
        }

        public void TakeItem()
        {
            
        }

        public void DropItem()
        {
            
        }
    }
}