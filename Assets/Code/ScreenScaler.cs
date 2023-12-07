using System;
using UnityEngine;

namespace Code
{
    public class ScreenScaler : MonoBehaviour
    {
        private void Start()
        {
            Resolution displayResolution = Screen.currentResolution;

            // Выводим информацию о разрешении в консоль (это можно убрать в релизе)
            Debug.Log("Display Resolution - Width: " + displayResolution.width + " Height: " + displayResolution.height);

            // Если вы хотите получить ширину и высоту системы дисплея
            Debug.Log("System Width: " + Display.main.systemWidth + " System Height: " + Display.main.systemHeight);

#if !UNITY_EDITOR
            Screen.SetResolution(Display.main.systemWidth,Display.main.systemHeight, fullscreen: false); // Замените значения на нужные вам
#endif
        
        }
    }
}