using System;
using Code.Components.Items;
using Code.Components.Items.Apples;
using Code.Data.Storages;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components.Characters
{
    public class CharacterItemsController : CharacterComponent, IGameInitListener
    {
        [Header("Components")]
        [SerializeField] private CharacterAnimator _characterAnimator;
        [SerializeField] private CharacterModeAdapter _modeAdapter;
        
        [Header("Storages")]
        private LiveStateStorage _storage;
        
        [Header("Dynamic data")]
        private Item _selectedItem;

        public void GameInit()
        {
            _storage = Container.Instance.FindStorage<LiveStateStorage>();
        }

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
                    _storage.AddPercentageValues(apple.GetPercentageValues());
                    OnEndReaction?.Invoke();
                });
            });
         
        }
    }
}