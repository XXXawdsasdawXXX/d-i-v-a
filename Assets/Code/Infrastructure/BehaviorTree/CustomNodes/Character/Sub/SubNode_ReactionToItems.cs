using Code.Components.Characters;
using Code.Components.Items;
using Code.Infrastructure.DI;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.CustomNodes.Character.Sub
{
    public class SubNode_ReactionToItems: BaseNode
    {
        [Header("Character")]
        private readonly CharacterItemsController _itemsController;

        [Header("Values")] 
        private Item _item;
        
        public SubNode_ReactionToItems()
        {
            //character-------------------------------------------------------------------------------------------------
            _itemsController = Container.Instance.FindEntity<DIVA>()
                .FindCharacterComponent<CharacterItemsController>();
        }
        
        protected override void Run()
        {
            if (IsCanRun())
            {
                Debugging.Instance.Log($"Саб нода реакции на объект: запущена", Debugging.Type.BehaviorTree);
                _itemsController.StartReactionToObject(_item, OnEndReaction: () =>
                {
                    Return(true);
                    _item = null;
                });
            }
            else
            {
                Debugging.Instance.Log($"Саб нода реакции на объект: пустой итем. попытка запустить оборвана", Debugging.Type.BehaviorTree);
                Return(false);
            }
        }

        protected override bool IsCanRun()
        {
            return _item != null;
        }

        protected override void OnReturn(bool success)
        {
            Debugging.Instance.Log($"Саб нода реакции на объект: ретерн {success}", Debugging.Type.BehaviorTree);
            _item = null;
            base.OnReturn(success);
        }

        protected override void OnBreak()
        {
            Debugging.Instance.Log($"Саб нода реакции на объект: брейк", Debugging.Type.BehaviorTree);
            _item = null;
        }

        public void SetCurrentItem(Item component)
        {
            Debugging.Instance.Log($"Саб нода реакции на объект: установлен итем", Debugging.Type.BehaviorTree);
            _item = component;
        }
    }
}