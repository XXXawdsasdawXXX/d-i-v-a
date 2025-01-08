using System;
using UnityEngine;

namespace Code.Data
{
    [Serializable]
    public class CooldownData
    {
        public int Sleep = 10;
        
        [Header("Reaction minutes")]
        public int MaxAudioClipReactionMin = 5;
        public int MouseReactionMin = 5;
        public int BadInteractionReactionMin = 5;
        public int InputWordsReactionMin = 0;
    }
}