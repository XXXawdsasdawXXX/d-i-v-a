﻿using System;
using System.Collections.Generic;
using Code.Data.Enums;
using Code.Data.SavedData;

namespace Code.Infrastructure.Save
{
    [Serializable]
    public class PlayerProgressData
    {
        public List<LiveStateSavedData> LiveStatesData = new();

        public PlayerProgressData()
        {
            LiveStatesData = new List<LiveStateSavedData>();
        }
    }
}