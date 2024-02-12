﻿using Code.Components.Apples;
using Code.Components.Objects;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components.Characters
{
    public class CharacterObjectsController : CharacterComponent, IGameInitListener, IGameExitListener
    {
        [SerializeField] private CharacterAnimator _characterAnimator;
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
                _collisionObserver.EnterEvent += StartReactionToObject;
                _collisionObserver.ExitEvent += StopReactionToObject;
                
            }
            else
            {
                _collisionObserver.EnterEvent -= StartReactionToObject;
                _collisionObserver.ExitEvent -= StopReactionToObject;
            }
        }

        public void StopReactionToObject(GameObject obj)
        {
            if (_selectedApple != null && obj.TryGetComponent(out Apple item))
            { 
                _characterAnimator.StopPlayEat();
            }
        }


        public void StartReactionToObject(GameObject obj)
        {
            if (obj.TryGetComponent(out Apple apple))
            {
                UseApple(apple);
            }
        }

        private void UseApple(Apple apple)
        {
            apple.transform.position = _modeAdapter.GetWorldEatPoint();
            _characterAnimator.StartPlayEat();
            apple.Use(OnEnd: () => _characterAnimator.StopPlayEat());
        }
    }
}