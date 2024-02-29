using System;
using System.Collections.Generic;
using Code.Data.SavedData;

namespace Code.Infrastructure.Save
{
    [Serializable]
    public class PlayerProgressData
    {
        public CooldownSavedData Cooldowns;
        public List<LiveStateSavedData> LiveStatesData;


        public PlayerProgressData()
        {
            Cooldowns = new CooldownSavedData();
            LiveStatesData = new List<LiveStateSavedData>();
        }
    }
}