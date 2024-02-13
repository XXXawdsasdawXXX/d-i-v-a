using Code.Components.Character.LiveState;
using Code.Components.Characters;
using Code.Components.Items;
using Code.Components.Objects;
using Code.Data.Enums;
using Code.Data.Value.RangeFloat;
using Code.Infrastructure.BehaviorTree.BaseNodes;
using Code.Infrastructure.BehaviorTree.CustomNodes.Sub;
using Code.Infrastructure.DI;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.CustomNodes
{
    public class BehaviorNode_Stand : BaseNode, IBehaviourCallback
    {
        [Header("Character")] 
        private readonly CharacterAnimator _characterAnimator;
        private readonly CharacterLiveStatesAnalytic _statesAnalytic;
        private readonly CollisionObserver _collisionObserver;

        [Header("Node")] 
        private BaseNode _node_Current;
        private readonly BaseNode_RandomSequence _node_randomSequence;
        private readonly SubNode_ItemsReaction _node_ItemReaction;

        public BehaviorNode_Stand()
        {
            //character-------------------------------------------------------------------------------------------------
            var character = Container.Instance.FindEntity<Character>();
            _statesAnalytic = character.FindCharacterComponent<CharacterLiveStatesAnalytic>();
            _characterAnimator = character.FindCharacterComponent<CharacterAnimator>();
            _collisionObserver = character.FindCommonComponent<CollisionObserver>();
            //node------------------------------------------------------------------------------------------------------
            _node_randomSequence = new BaseNode_RandomSequence(new BaseNode[]
            {
                new SubNode_WaitForSeconds(new RangedFloat()
                {
                    MinValue = 60 * /*1*/0.15f,
                    MaxValue = 60 * /*5*/ 0.5f
                }),
                new SubNode_LookToMouse()
            });
            _node_ItemReaction = new SubNode_ItemsReaction();
        }

        protected override void Run()
        {
            if (IsCanStand())
            {
                Debugging.Instance.Log($"Нода стояния: выбрано", Debugging.Type.BehaviorTree);

                SubscribeToEvents(true);
                _characterAnimator.EnterToMode(CharacterAnimationMode.Stand);
                RunNode(_node_randomSequence);
            }
            else
            {
                Debugging.Instance.Log(
                    $"Нода стояния: отказ. текущий минимальный стрейт {_statesAnalytic.CurrentLowerLiveStateKey}",
                    Debugging.Type.BehaviorTree);

                Return(false);
            }
        }

        void IBehaviourCallback.InvokeCallback(BaseNode node, bool success)
        {
            Debugging.Instance.Log($"Нода стояния: колбэк ", Debugging.Type.BehaviorTree);
            if (_statesAnalytic.CurrentLowerLiveStateKey == LiveStateKey.None)
            {
                /*Debugging.Instance.Log($"Нода стояния: колбэк -> запуск рандомной сиквенции",
                    Debugging.Type.BehaviorTree);*/
                /*if (_node_Current == null)
                {
                    RunNode(_node_randomSequence);
                }
                else
                {
                    return;
                }*/
            }
            else
            {
                Debugging.Instance.Log($"Нода стояния: колбэк -> ретерн тру", Debugging.Type.BehaviorTree);
                //Return(true);
            }
        }

        protected override void OnBreak()
        {
            Debugging.Instance.Log($"Нода стояния: брейк ", Debugging.Type.BehaviorTree);
            SubscribeToEvents(false);
            _node_Current?.Break();
            _node_Current = null;
            base.OnBreak();
        }

        protected override void OnReturn(bool success)
        {
            Debugging.Instance.Log($"Нода стояния: ретерн {success} ", Debugging.Type.BehaviorTree);
            SubscribeToEvents(false);
            _node_Current?.Break();
            _node_Current = null;
            base.OnReturn(success);
        }

        private void RunNode(BaseNode node)
        {
            /*if (_node_Current != null && _node_Current == node)
            {
                Debugging.Instance.Log($"Нода стояния: попыталась запустить уже запущенную саб ноду ",
                    Debugging.Type.BehaviorTree);
                return;
            }*/
            _node_Current?.Break();
            _node_Current = node;
            _node_Current.Run(this);
        }

        #region Events

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _collisionObserver.EnterEvent += StartReactionToObject;
            }
            else
            {
                _collisionObserver.EnterEvent -= StartReactionToObject;
            }
        }

        private void StartReactionToObject(GameObject obj)
        {
            if (obj.TryGetComponent(out Item item))
            {
                Debugging.Instance.Log($"Нода стояния: начинает реакцию на итем ", Debugging.Type.BehaviorTree);
                _node_ItemReaction.SetCurrentItem(item);
               RunNode(_node_ItemReaction);
            }
        }

        #endregion

        #region Conditions

        private bool IsCanStand()
        {
            return true;
        }

        #endregion
    }
}