using System;
using Code.Data.Enums;

namespace Code.Data.StaticData
{
    [Serializable]
    public class InteractionData
    {
        public InteractionType СlassificationType;
        public int MaxPerDay;
    }
    
    
    [Serializable]
    public class InteractionDynamicData
    {
        public InteractionType СlassificationType;
        public int Value;
    }
}