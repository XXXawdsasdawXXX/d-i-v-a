using System;
using Code.Data.Value;
using Code.Data.Value.RangeInt;
using UnityEngine;

namespace Code.Entities.Items
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