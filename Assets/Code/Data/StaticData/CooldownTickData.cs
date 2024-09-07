using System;

namespace Code.Data.StaticData
{
    [Serializable]
    public class CooldownTickData
    {
        public int Sleep = 10;
        public int ReactionMaxAudioClip = 5;
        public int ReactionMouse = 5;
        public int BadInteractionReaction = 5;
    }
}