using System;
using System.Collections.Generic;
using Code.Game.Services.Interactions;

namespace Code.Infrastructure.Save
{
    [Serializable]
    public class PlayerProgressData
    {
        public CustomActionsSavedData CustomActions;
        public CooldownSavedData Cooldowns;
        public List<LiveStateSavedData> LiveStatesData;
        public Dictionary<EInteractionType, int> Interactions;
        public DateTime GameExitTime;
        public DateTime GameEnterTime;

        public PlayerProgressData()
        {
            CustomActions = new CustomActionsSavedData();
            Cooldowns = new CooldownSavedData();
            LiveStatesData = new List<LiveStateSavedData>();
            Interactions = new Dictionary<EInteractionType, int>();
        }
    }
}