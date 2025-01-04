using System;
using Code.Components.Common;
using Code.Components.Entities;
using Code.Infrastructure.Pools;
using Code.Infrastructure.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Items
{
    [RequireComponent(typeof(RuntimeEntityHandler))]
    public class Item: Entity,IPoolEntity
    {
        [SerializeField] private ItemAnimation _itemAnimation;
        [SerializeField] private PhysicsDragAndDrop _dragAndDrop;
        [SerializeField] private ColliderButton _colliderButton;
        private  TickCounter _liveTime;
        
        public ItemData Data { get; private set; }
        public bool IsActive { get; private set; }

        private bool _isDrop;
        public event Action OnPressed;
        public event Action OnUsed;
        public event Action OnDestroyed;


        #region Pool methods

        public void Init(params object[] parameters)
        {
            _liveTime = new();
        }

        public void Enable()
        {
            IsActive = true;
            _dragAndDrop.Off();
            
            SubscribeToEvents(true);
            
            gameObject.SetActive(true);
            _itemAnimation.PlayEnter();
        }

        public void Disable()
        {
            IsActive = false;
            SubscribeToEvents(false);
            gameObject.SetActive(false);
        }

        #endregion

        public bool IsCanUse()
        {
            return IsActive && _dragAndDrop.IsActive;
        }

        public void SetData(ItemData data)
        {
            Debugging.Instance.Log($"{data.Type} {data.SpawnChance}", Debugging.Type.Items);
            
            Data = data;
            _itemAnimation.SetController(Data.AnimatorController);
            
            int ticks = Data.LiveTimeTicks.GetRandomValue();
            if (ticks > 0)
            {
                _liveTime.StartWait(ticks);
            }
        }

        public void Destroy(Action onCompleted = null)
        {
            _itemAnimation.PlayDestroy(onPlayed: () =>
            {
                onCompleted?.Invoke();
                OnDestroyed?.Invoke();
            });
        }

        public void Use(Action onCompleted = null)
        {
            _itemAnimation.PlayUse(onPlayed: onCompleted);
        }

        public void Lock()
        {
            _dragAndDrop.Off();
            _liveTime.StopWait();
        }


        #region Events

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _colliderButton.OnPressedDown += OnPressedDown;
                _liveTime.OnWaitIsOver += OnEndLiveTime;
            }
            else
            {
                _colliderButton.OnPressedDown -= OnPressedDown;
                _liveTime.OnWaitIsOver -= OnEndLiveTime;
            }
        }

        /// <summary>
        /// ColliderButton event callback
        /// </summary>
        /// <param name="_"></param>
        /// <param name="__"></param>
        private void OnPressedDown(Vector2 _)
        {
            _dragAndDrop.On();
            OnPressed?.Invoke();
        }

        /// <summary>
        /// LiveTime(tickCounter) event callback
        /// </summary>
        private void OnEndLiveTime()
        {
            Destroy();
        }

        #endregion

        #region Editor

        /// <summary>
        /// Editor
        /// </summary>
        public void FindAllComponents()
        {
            _commonComponents = GetComponentsInChildren<CommonComponent>(true);
        }

        #endregion
    }
}