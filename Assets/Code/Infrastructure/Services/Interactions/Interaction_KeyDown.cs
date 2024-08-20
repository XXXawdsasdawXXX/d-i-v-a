using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Infrastructure.Services.Interactions
{
    public class Interaction_KeyDown : InteractionObserver, IGameInitListener, IGameTickListener
    {
        public enum InputWorlds
        {
            None,
            Hello,
            Hi,
            Привет,
            Yo,
            Love,
        }

        public void GameInit()
        {
        }

        public void GameTick()
        {
            foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(kcode))
                {
                    Debug.Log("Клавиша нажата: " + kcode);
                }
            }
        }
    }
}