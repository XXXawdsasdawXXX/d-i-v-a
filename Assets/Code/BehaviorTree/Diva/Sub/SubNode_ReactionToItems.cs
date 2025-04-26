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
                Log.Info(this, $"[run]", Log.Type.BehaviorTree);
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
                Log.Info(this, $"[run] Return -> item is null", Log.Type.BehaviorTree);
#endif
                Return(false);
            }
        }

        public void SetCurrentItem(ItemEntity item)
        {
            _item = item;
#if DEBUGGING
            Log.Info(this, "[SetCurrentItem]", Log.Type.BehaviorTree);
#endif
        }

        protected override bool IsCanRun()
        {
            return _item != null;
        }

        protected override void OnReturn(bool success)
        {
#if DEBUGGING
            Log.Info(this, $"[on return] {success}", Log.Type.BehaviorTree);
#endif
            _item = null;

            base.OnReturn(success);
        }

        protected override void OnBreak()
        {
            _item = null;

#if DEBUGGING
            Log.Info(this, "[on break]", Log.Type.BehaviorTree);
#endif
        }
    }
}