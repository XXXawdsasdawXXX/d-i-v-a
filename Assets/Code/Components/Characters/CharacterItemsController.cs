using System;
using Code.Components.Items;
using Code.Components.Items.Apples;
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
        
        private void UseApple(Apple apple, Action OnEndReaction = null)
        {
            if (!apple.IsActive)
            {
                return;
            }
            
            apple.ReadyForUse(_modeAdapter.GetWorldEatPoint());
      
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