using Code.Game.Entities.Diva;
using Code.Game.Entities.Items;
using Code.Infrastructure.ServiceLocator;
using Code.Utils;
using UnityEngine;

namespace Code.Game.BehaviorTree.Diva
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
                Log.Info(this, $"[run]", Log.Type.BehaviorTree);
                _itemsController.StartReactionToObject(_item, OnEndReaction: () =>
                {
                    Return(true);
                    _item = null;
                });
            }
            else
            {
                Log.Info(this, $"[run] Return -> item is null", Log.Type.BehaviorTree);
                Return(false);
            }
        }

        public void SetCurrentItem(ItemEntity item)
        {
            _item = item;

            Log.Info(this, "[SetCurrentItem]", Log.Type.BehaviorTree);
        }

        protected override bool IsCanRun()
        {
            return _item != null;
        }

        protected override void OnReturn(bool success)
        {
            Log.Info(this, $"[on return] {success}", Log.Type.BehaviorTree);

            _item = null;

            base.OnReturn(success);
        }

        protected override void OnBreak()
        {
            _item = null;

            Log.Info(this, "[on break]", Log.Type.BehaviorTree);
        }
    }
}