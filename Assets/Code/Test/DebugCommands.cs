using Code.Data.Enums;
using Code.Data.Storages;
using Code.Data.Value;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using IngameDebugConsole;
using UnityEngine;

namespace Code.Test
{
    public class DebugCommands : MonoBehaviour,  IGameStartListener
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