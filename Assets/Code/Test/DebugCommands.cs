﻿using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using IngameDebugConsole;
using UnityEngine;

namespace Code.Test
{
    public class DebugCommands : MonoBehaviour
    {
        public void GameStart()
        {
            // EditCommands();
        }

        private void EditCommands()
        {
           // DebugLogConsole.AddCommand<int>("add.sleep", "", AddSleepValue);
        }
    }
}