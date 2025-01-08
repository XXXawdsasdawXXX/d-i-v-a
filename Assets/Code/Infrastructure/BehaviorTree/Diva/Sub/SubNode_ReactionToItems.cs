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
            //character-------------------------------------------------------------------------------------------------
            _itemsController = Container.Instance.FindEntity<Entities.Diva.DivaEntity>()
                .FindCharacterComponent<DivaItemsController>();
        }

        protected override void Run()
        {
            if (IsCanRun())
            {
                Debugging.Log($"Саб нода реакции на объект: запущена", Debugging.Type.BehaviorTree);
                _itemsController.StartReactionToObject(_item, OnEndReaction: () =>
                {
                    Return(true);
                    _item = null;
                });
            }
            else
            {
                Debugging.Log($"Саб нода реакции на объект: пустой итем. попытка запустить оборвана",
                    Debugging.Type.BehaviorTree);
                Return(false);
            }
        }

        protected override bool IsCanRun()
        {
            return _item != null;
        }

        protected override void OnReturn(bool success)
        {
            Debugging.Log($"Саб нода реакции на объект: ретерн {success}", Debugging.Type.BehaviorTree);
            _item = null;
            base.OnReturn(success);
        }

        protected override void OnBreak()
        {
            Debugging.Log($"Саб нода реакции на объект: брейк", Debugging.Type.BehaviorTree);
            _item = null;
        }

        public void SetCurrentItem(ItemEntity component)
        {
            Debugging.Log($"Саб нода реакции на объект: установлен итем", Debugging.Type.BehaviorTree);
            _item = component;
        }
    }
}