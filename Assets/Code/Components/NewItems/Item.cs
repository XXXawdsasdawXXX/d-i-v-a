using System;
using Code.Components.Common;
using Code.Components.Entities;
using Code.Infrastructure.Services;
using Code.Services.Pools;
using Code.Utils;
using UnityEngine;

namespace Code.Components.NewItems
{
    [RequireComponent(typeof(RuntimeEntityHandler))]
    public class Item: Entity,IPoolEntity
    {
        [SerializeField] private ItemAnimation _itemAnimation;
        [SerializeField] private PhysicsDragAndDrop _dragAndDrop;
        [SerializeField] private ColliderButton _colliderButton;
        [SerializeField] private AnchorMover _anchorMover;
        private  TickCounter _liveTime;
        
        public ItemData Data { get; private set; }
        public bool IsActive { get; private set; }

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
        
        public void SetData(ItemData data, Transform anchor = null)
        {
            Debugging.Instance.Log($"{data.Type} {data.SpawnChance}", Debugging.Type.Items);
            Data = data;
            _anchorMover.SetAnchor(anchor);
            _itemAnimation.SetController(Data.AnimatorController );

            if (anchor == null)
            {
                _dragAndDrop.On();
                _anchorMover.Off();   
            }
            else
            {
                _dragAndDrop.Off();
                _anchorMover.On();
            }

            var ticks = Data.LiveTimeTicks.GetRandomValue();
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
            _anchorMover.Off();
        }


        #region Events
        
        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _colliderButton.OnPressedUp += OnPressedUp;
                _liveTime.OnWaitIsOver += OnEndLiveTime;
            }
            else
            {
                _colliderButton.OnPressedUp -= OnPressedUp;
                _liveTime.OnWaitIsOver -= OnEndLiveTime;
            }
        }

        /// <summary>
        /// ColliderButton event callback
        /// </summary>
        /// <param name="_"></param>
        /// <param name="__"></param>
        private void OnPressedUp(Vector2 _, float __)
        {
            OnPressed?.Invoke();
            _dragAndDrop.On();
            _anchorMover.Off();
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