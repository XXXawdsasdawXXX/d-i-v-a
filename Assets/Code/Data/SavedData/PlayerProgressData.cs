using System;
using System.Collections.Generic;
using Code.Data.Enums;
using Code.Data.SavedData;

namespace Code.Infrastructure.Save
{
    [Serializable]
    public class PlayerProgressData
    {
        public CustomActionsSavedData CustomActions;
        public CooldownSavedData Cooldowns;
        public List<LiveStateSavedData> LiveStatesData;
        public Dictionary<InteractionType, int> Interactions;
        public DateTime GameExitTime;
        public DateTime GameEnterTime;
        
        public PlayerProgressData()
        {
            CustomActions = new CustomActionsSavedData();
            Cooldowns = new CooldownSavedData();
            LiveStatesData = new List<LiveStateSavedData>();
            Interactions = new Dictionary<InteractionType, int>();
        }
    }
}