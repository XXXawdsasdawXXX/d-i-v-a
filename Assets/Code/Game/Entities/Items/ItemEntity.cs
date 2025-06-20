﻿using System;
using Code.Game.Entities.Common;
using Code.Game.Services;
using Code.Infrastructure.Pools;
using Code.Utils;
using UnityEngine;

namespace Code.Game.Entities.Items
{
    [RequireComponent(typeof(RuntimeEntityHandler))]
    public class ItemEntity : Entity, IPoolEntity
    {
        public event Action OnPressed;
        public event Action OnUsed;
        public event Action OnDestroyed;
        
        public ItemData Data { get; private set; }
        public bool IsActive { get; private set; }

        [SerializeField] private ItemAnimation _itemAnimation;
        [SerializeField] private PhysicsDragAndDrop _dragAndDrop;
        [SerializeField] private ColliderButton _colliderButton;
        
        private TickCounter _liveTime;
        private bool _isDrop;
        

        public void Enable()
        {
            _liveTime ??= new TickCounter();
            
            IsActive = true;
          
            _dragAndDrop.Disable();

            _subscribeToEvents(true);

            gameObject.SetActive(true);
            
            _itemAnimation.PlayEnter();
        }

        public void Disable()
        {
            IsActive = false;
            
            _subscribeToEvents(false);
            
            gameObject.SetActive(false);
        }

        public bool IsCanUse()
        {
            return IsActive && _dragAndDrop.IsActive;
        }

        public void SetData(ItemData data)
        {
            Data = data;
    
            _itemAnimation.SetController(Data.AnimatorController);

            int ticks = Data.LiveTimeTicks.GetRandomValue();
            if (ticks > 0)
            {
                _liveTime.StartWait(ticks);
            }

            Log.Info(this, $"{data.Type} {data.SpawnChance}", Log.Type.Items);
        }

        public void Use(Action onCompleted = null)
        {
            _itemAnimation.PlayUse(onPlayed: onCompleted);
        }

        public void Lock()
        {
            _dragAndDrop.Disable();
           
            _liveTime.StopWait();
        }
        
        private void _subscribeToEvents(bool flag)
        {
            if (flag)
            {
                _colliderButton.OnPressedDown += _onPressedDown;
                _liveTime.OnWaitIsOver += _onEndLiveTime;
            }
            else
            {
                _colliderButton.OnPressedDown -= _onPressedDown;
                _liveTime.OnWaitIsOver -= _onEndLiveTime;
            }
        }

        private void _onPressedDown(Vector2 _)
        {
            _dragAndDrop.Active();
            
            OnPressed?.Invoke();
        }

        private void _onEndLiveTime()
        {
            _destroy();
        }

        private void _destroy(Action onCompleted = null)
        {
            _itemAnimation.PlayDestroy(onPlayed: () =>
            {
                onCompleted?.Invoke();
                OnDestroyed?.Invoke();
            });
        }
        
#if UNITY_EDITOR
        
        /// <summary>
        /// Editor
        /// </summary>
        public void FindAllComponents()
        {
            _commonComponents = GetComponentsInChildren<CommonComponent>(true);
        }
        
#endif
        
    }
}