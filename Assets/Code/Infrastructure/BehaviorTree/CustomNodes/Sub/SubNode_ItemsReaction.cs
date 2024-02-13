﻿using Code.Components.Characters;
using Code.Components.Items;
using Code.Infrastructure.DI;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.CustomNodes.Sub
{
    public class SubNode_ItemsReaction: BaseNode
    {
        [Header("Character")]
        private readonly CharacterItemsController _itemsController;

        [Header("Values")] 
        private Item _item;
        
        public SubNode_ItemsReaction()
        {
            //character-------------------------------------------------------------------------------------------------
            _itemsController = Container.Instance.FindEntity<Character>()
                .FindCharacterComponent<CharacterItemsController>();
        }
        
        protected override void Run()
        {
            if (_item == null)
            {
                Debugging.Instance.Log($"Саб нода реакции на объект: пустой итем. попытка запустить оборвана", Debugging.Type.BehaviorTree);
                Return(false);
            }
            else
            {
                Debugging.Instance.Log($"Саб нода реакции на объект: запущена", Debugging.Type.BehaviorTree);
                _itemsController.StartReactionToObject(_item, OnEndReaction: () =>
                {
                    Return(true);
                    _item = null;
                });
            }
        }

        protected override void OnReturn(bool success)
        {
                Debugging.Instance.Log($"Саб нода реакции на объект: ретерн {success}", Debugging.Type.BehaviorTree);
            base.OnReturn(success);
            _item = null;
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