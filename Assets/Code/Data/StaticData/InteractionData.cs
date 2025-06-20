﻿using System;
using Code.Game.Services.Interactions;

namespace Code.Data
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