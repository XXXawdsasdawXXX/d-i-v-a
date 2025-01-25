using Code.Entities.Diva;
using Code.Entities.Items;
using Code.Infrastructure.DI;
using Code.Utils;
using UnityEngine;

namespace Code.BehaviorTree.Diva
{
    public class SubNode_ReactionToItems : BaseNode
    {
        [Header("Character")] 
        private readonly DivaItemsController _itemsController;

        [Header("Values")] 
        private ItemEntity _item;

        public SubNode_ReactionToItems()
        {
            _itemsController = Container.Instance.FindEntity<DivaEntity>()
                .FindCharacterComponent<DivaItemsController>();
        }

        protected override void Run()
        {
            if (IsCanRun())
            {
#if DEBUGGING
                Debugging.Log(this, $"[run]", Debugging.Type.BehaviorTree);
#endif
                _itemsController.StartReactionToObject(_item, OnEndReaction: () =>
                {
                    Return(true);
                    _item = null;
                });
            }
            else
            {
#if DEBUGGING
                Debugging.Log(this, $"[run] Return -> item is null", Debugging.Type.BehaviorTree);
#endif
                Return(false);
            }
        }

        public void SetCurrentItem(ItemEntity item)
        {
            _item = item;
#if DEBUGGING
            Debugging.Log(this, "[SetCurrentItem]", Debugging.Type.BehaviorTree);
#endif
        }

        protected override bool IsCanRun()
        {
            return _item != null;
        }

        protected override void OnReturn(bool success)
        {
#if DEBUGGING
            Debugging.Log(this, $"[on return] {success}", Debugging.Type.BehaviorTree);
#endif
            _item = null;

            base.OnReturn(success);
        }

        protected override void OnBreak()
        {
            _item = null;

#if DEBUGGING
            Debugging.Log(this, "[on break]", Debugging.Type.BehaviorTree);
#endif
        }
    }
}