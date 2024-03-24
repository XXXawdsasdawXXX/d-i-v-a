﻿using Code.Components.Apples;
using Code.Components.Hands;
using Code.Components.Objects;
using Code.Data.Configs;
using Code.Infrastructure.BehaviorTree.CustomNodes.Character;
using Code.Infrastructure.BehaviorTree.CustomNodes.Character.Sub;
using Code.Infrastructure.DI;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.CustomNodes.Hand.Behavior
{
    public class BehaviourNode_ShowApple : BaseNode
    {
        [Header("Entities")] 
        private readonly Apple _apple;

        [Header("Hand")] //☺
        private readonly ColliderButton _handButton;
        private readonly HandMovement _handMovement;
        private readonly HandAnimator _handAnimator;
        private readonly ItemHolder _itemHolder;

        [Header("Services")] 
        private readonly TickCounter _tickCounter_cooldown;
        private readonly TickCounter _tickCounter_liveTime;
        private readonly CharacterCondition _characterCondition;

        [Header("Static values")] 
        private readonly HandConfig _handConfig;
        private readonly InteractionStorage _interactionsStorage;
        private readonly WhiteBoard_Hand _whiteBoard;
        private readonly AppleConfig _appleConfig;

        [Header("Dynamic values")] 
        private bool _isExpectedStart = false;
        private bool _isHidden = true;


        public BehaviourNode_ShowApple()
        {
            //enitities
            _apple = Container.Instance.FindItem<Apple>();
          
            //hand
            var hand = Container.Instance.FindEntity<Components.Entities.Hand>();
            _handButton = hand.FindCommonComponent<ColliderButton>();
            _handAnimator = hand.FindHandComponent<HandAnimator>();
            _handMovement = hand.FindHandComponent<HandMovement>();
            _itemHolder = hand.FindCommonComponent<ItemHolder>();

            //static data
            _handConfig = Container.Instance.FindConfig<HandConfig>();
            _interactionsStorage = Container.Instance.FindStorage<InteractionStorage>();
            _whiteBoard = Container.Instance.FindStorage<WhiteBoard_Hand>();
            _appleConfig = Container.Instance.FindConfig<AppleConfig>();

            //services 
            _tickCounter_cooldown = new TickCounter(isLoop: false);
            _tickCounter_liveTime = new TickCounter(isLoop: false);
            _characterCondition = Container.Instance.FindService<CharacterCondition>();
            
            SubscribeToGlobalEvents(true);
            StartWaitCooldown();
        }

        ~BehaviourNode_ShowApple()
        {
            SubscribeToGlobalEvents(false);
        }

        #region Live cycle
        protected override void Run()
        {
            if (!IsCanShowApple())
            {
                Return(false);
                return;
            }

            var dropChance = _handConfig.GetAppleDropChance(_interactionsStorage.GetSum());
            var random = Random.Range(0, 101);
            if (random <= dropChance)
            {
                ShowHandWithApple();
                return;
            }

            Debugging.Instance.Log($"[showapple_run] не покажет яблоко {random} > {dropChance}. interaction count = {_interactionsStorage.GetSum()}", Debugging.Type.Hand);
            _isExpectedStart = true;
            Return(false);
        }

        #endregion

        #region Conditions

        private bool IsCanShowApple()
        {
            return _isExpectedStart 
                   && _tickCounter_cooldown.IsExpectedStart 
                   &&!_characterCondition.IsCanSleep(sleepStatePercent: 0.6f);
        }

        #endregion
        
        #region Unique methods
        private void ShowHandWithApple()
        {
            _whiteBoard.SetData(WhiteBoard_Hand.Type.IsShowApple, true);
            _isExpectedStart = false;
            
            GrowApple();

            _handAnimator.PlayEnter();
            _handMovement.On();
            
            StartWaitLiveTime();
            SubscribeToLocalEvents(true);
            Debugging.Instance.Log($"[showapple_ShowWhitApple] показал яблоко", Debugging.Type.Hand);
        }
        
        private void HideHand()
        {
            _whiteBoard.SetData(WhiteBoard_Hand.Type.IsShowApple, false);
        
            TryDropApple();
            
            _handAnimator.PlayExit();
            _handMovement.Off();
            
            StartWaitCooldown();
            SubscribeToLocalEvents(false);
            Return(true);
            Debugging.Instance.Log($"[showapple_HideHand] спрятал руку", Debugging.Type.Hand);
        }

        private void StartWaitLiveTime()
        {
            int liveTimeTicks = _handConfig.GetLiveTimeTicks();
            _tickCounter_liveTime.StartWait(liveTimeTicks);
            Debugging.Instance.Log($"[showapple_StartWaitCooldown] стал ждать конца {liveTimeTicks} жизненных тиков", Debugging.Type.Hand);
        }
        private void StartWaitCooldown()
        {
            var cooldownTicks = _appleConfig.SpawnCooldownTick.GetRandomValue();
            _tickCounter_cooldown.StartWait(cooldownTicks);
            Debugging.Instance.Log($"[showapple_StartWaitCooldown] стал ждать кулдаун {cooldownTicks} тиков", Debugging.Type.Hand);
        }

        private void GrowApple()
        {
            _itemHolder.SetItem(_apple);
            _apple.Grow();
        }

        private void TryDropApple()
        {
            if (_itemHolder.SelectedItem == null || !_apple.IsActive)
            {
                return;
            }
            _itemHolder.DropItem();
            _apple.Fall();
        }

#endregion

        #region Events
        
        //Local
        private void SubscribeToLocalEvents(bool flag)
        {
            if (flag)
            {
                _apple.DieEvent += HideHand;
                _apple.ReadyForUseEvent += HideHand;
                _handButton.UpEvent += HandButtonOnUpEvent;
            }
            else
            {
                _apple.DieEvent -= HideHand;
                _apple.ReadyForUseEvent -= HideHand;
                _handButton.UpEvent -= HandButtonOnUpEvent;
            }
        }

        private void HandButtonOnUpEvent(Vector2 _, float __)
        {
            HideHand();
        }

        //Global
        private void SubscribeToGlobalEvents(bool flag)
        {
            if (flag)
            {
                _tickCounter_cooldown.WaitedEvent += TickCounter_cooldownOnWaitedEvent;
                _tickCounter_liveTime.WaitedEvent += TickCounter_liveTimeOnWaitedEvent;
            }
            else
            {
                _tickCounter_cooldown.WaitedEvent -= TickCounter_cooldownOnWaitedEvent;
                _tickCounter_liveTime.WaitedEvent -= TickCounter_liveTimeOnWaitedEvent;
            }
        }

        private void TickCounter_liveTimeOnWaitedEvent()
        {
            if (_whiteBoard.TryGetData(WhiteBoard_Hand.Type.IsShowApple, out bool isShow) && isShow)
            {
                HideHand();
            }
        }

        private void TickCounter_cooldownOnWaitedEvent()
        {
            if (!_isExpectedStart)
            {
                _isExpectedStart = true;
            }
        }

        #endregion
    }
}