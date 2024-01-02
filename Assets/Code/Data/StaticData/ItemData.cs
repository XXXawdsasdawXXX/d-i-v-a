using System;
using Code.Data.Enums;
using Code.Data.Value;

namespace Code.Data.StaticData
{
    [Serializable]
    public class ItemData
    {
        public ItemType Type;
        public float LiveTime;
        public LiveStateValue[] InfluentialValues;
    }
}