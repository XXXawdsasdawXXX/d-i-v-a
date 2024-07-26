using Code.Infrastructure.GameLoop;
using Code.Services;
using UnityEngine;

public class Interaction_WriteWorld : InteractionObserver,IGameInitListener ,IGameTickListener
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