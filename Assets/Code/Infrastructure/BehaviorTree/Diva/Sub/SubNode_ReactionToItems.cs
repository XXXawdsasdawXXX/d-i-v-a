using Code.Entities.Diva;
using Code.Entities.Items;
using Code.Infrastructure.DI;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.Diva
{
    public class SubNode_ReactionToItems : BaseNode
    {
        [Header("Character")] 
        private readonly DivaItemsController _itemsController;

        [Header("Values")] 
        private ItemEntity _item;

        public SubNode_ReactionToItems()
        {
            _itemsController = Container.Instance.FindEntity<DivaEntity>().FindCharacterComponent<DivaItemsController>();
        }

        protected override void Run()
        {
            if (IsCanRun())
            {
                Debugging.Log(this, $"[Run]", Debugging.Type.BehaviorTree);

                _itemsController.StartReactionToObject(_item, OnEndReaction: () =>
                {
                    Return(true);
                    _item = null;
                });
            }
            else
            {
                Debugging.Log(this, $"[Run] Return -> item is null", Debugging.Type.BehaviorTree);
                
                Return(false);
            }
        }

        protected override bool IsCanRun()
        {
            return _item != null;
        }

        protected override void OnReturn(bool success)
        {
            Debugging.Log(this, $"[on return] {success}", Debugging.Type.BehaviorTree);
            
            _item = null;
            
            base.OnReturn(success);
        }

        protected override void OnBreak()
        {
            _item = null;
            
            Debugging.Log(this, "[on break]", Debugging.Type.BehaviorTree);
        }

        public void SetCurrentItem(ItemEntity item)
        {
            _item = item;
            
            Debugging.Log(this, "[SetCurrentItem]", Debugging.Type.BehaviorTree);
        }
    }
}