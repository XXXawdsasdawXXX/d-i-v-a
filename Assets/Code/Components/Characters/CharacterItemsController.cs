using System;
using Code.Components.Apples;
using Code.Components.Items;
using UnityEngine;

namespace Code.Components.Characters
{
    public class CharacterItemsController : CharacterComponent
    {
        [SerializeField] private CharacterAnimator _characterAnimator;
        [SerializeField] private CharacterModeAdapter _modeAdapter;
        
        private Item _selectedItem;

        public void StartReactionToObject(Item item, Action OnEndReaction = null)
        {
            if (item is Apple apple)
            {
                UseApple(apple, OnEndReaction);
            }
        }

        public void StopReactionToObject()
        {
            if (_selectedItem == null)
            {
                 return;   
            }
            if (_selectedItem is Apple)
            { 
                _characterAnimator.StopPlayEat();
            }
        }


        private void UseApple(Apple apple, Action OnEndReaction = null)
        {
            if (!apple.IsActive)
            {
                return;
            }
            
            apple.ReadyForUse();
            apple.transform.position = _modeAdapter.GetWorldEatPoint();
            _characterAnimator.StartPlayEat(OnReadyEat: () =>
            {
                apple.Use(OnEnd: () =>
                {
                    _characterAnimator.StopPlayEat();
                    OnEndReaction?.Invoke();
                });
            });
         
        }
    }
}