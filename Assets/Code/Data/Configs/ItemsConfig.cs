﻿using Code.Components.NewItems;
using UnityEngine;

namespace Code.Data
{
    [CreateAssetMenu(fileName = "ItemsConfig", menuName = "Configs/Items Config")]
    public class ItemsConfig :ScriptableObject
    {
        public ItemData[] Items;
    }
}