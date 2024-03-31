using UnityEngine;

namespace Code.Test
{
    public class TestDrive : MonoBehaviour
    {
        void Update()
        {
            // Проверяем каждую клавишу в перечислении KeyCode
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
