using System;
using Code.Data.Enums;

namespace Code.Data.StaticData
{
    [Serializable]
    public class InteractionData
    {
        public EInteractionType СlassificationType;
        public int MaxPerDay;
    }


    [Serializable]
    public class InteractionDynamicData
    {
        public EInteractionType СlassificationType;
        public int Value;
    }
}