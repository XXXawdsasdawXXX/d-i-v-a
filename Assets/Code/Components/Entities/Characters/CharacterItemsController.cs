using System;
using Code.Components.Items;
using Code.Components.Items.Apples;
using Code.Data.Value;
using UnityEngine;

namespace Code.Components.Entities.Characters
{
    public class CharacterItemsController : CharacterComponent
    {
        [Header("Components")]
        [SerializeField] private CharacterAnimator _characterAnimator;
        [SerializeField] private CharacterModeAdapter _modeAdapter;
        
        
        [Header("Dynamic data")]
        private Item _selectedItem;
        
        public event Action<LiveStatePercentageValue[]> OnUseItem;

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
                    OnUseItem?.Invoke(apple.GetPercentageValues());
                    OnEndReaction?.Invoke();
                });
            });
         
        }
    }
}