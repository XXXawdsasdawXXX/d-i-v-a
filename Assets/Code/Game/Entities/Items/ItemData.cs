using System;
using Code.Data;
using UnityEngine;

namespace Code.Game.Entities.Items
{
    [Serializable]
    public class ItemData
    {
        public ItemType Type;
        public int SpawnChance; 
        public RuntimeAnimatorController AnimatorController;
        [MinMaxRangeInt(0, 100)] public RangedInt LiveTimeTicks; 
        public LiveStatePercentageValues BonusValues;
    }
}