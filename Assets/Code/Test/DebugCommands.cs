using Code.Data.Enums;
using Code.Data.Storages;
using Code.Data.Value;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using IngameDebugConsole;
using UnityEngine;

namespace Code.Test
{
    public class DebugCommands : MonoBehaviour, IGameInitListener, IGameStartListener
    {
        private LiveStateStorage _liveStateStorage;

        public void GameInit()
        {
            _liveStateStorage = Container.Instance.FindStorage<LiveStateStorage>();
        }

        public void GameStart()
        {
            EditCommands();
        }
        
        private void EditCommands()
        {
            DebugLogConsole.AddCommand<int>("add.sleep", "", AddSleepValue);
            DebugLogConsole.AddCommand<int>("add.trust", "", AddTrustValue);
            DebugLogConsole.AddCommand<int>("add.eat", "", AddEatValue);
            

            //DebugLogConsole.RemoveCommand("prefs.clear");
            DebugLogConsole.RemoveCommand("prefs.delete");
            DebugLogConsole.RemoveCommand("prefs.float");
            DebugLogConsole.RemoveCommand("prefs.int");
            DebugLogConsole.RemoveCommand("prefs.string");
            DebugLogConsole.RemoveCommand("scene.load");
            DebugLogConsole.RemoveCommand("scene.loadasync");
            DebugLogConsole.RemoveCommand("scene.unload");
            //DebugLogConsole.RemoveCommand("scene.restart");
            DebugLogConsole.RemoveCommand("time.scale");
            DebugLogConsole.RemoveCommand("logs.save");
            //DebugLogConsole.RemoveCommand("sysinfo");
        }

        private void AddEatValue(int obj)
        {
            _liveStateStorage.AddPercentageValue(new LiveStatePercentageValue()
            {
                Key = LiveStateKey.Hunger,
                Value = obj
            });
        }

        private void AddTrustValue(int obj)
        {
            _liveStateStorage.AddPercentageValue(new LiveStatePercentageValue()
            {
                Key = LiveStateKey.Trust,
                Value = obj
            });
        }

        private void AddSleepValue(int obj)
        {  
            _liveStateStorage.AddPercentageValue(new LiveStatePercentageValue()
            {
                Key = LiveStateKey.Sleep,
                Value = obj
            });
        }
    }
}