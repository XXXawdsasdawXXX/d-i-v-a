using Code.Data;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.Services.Getters
{
    public class DisplayColorGetter : MonoBehaviour, IGetter
    {
        [SerializeField] private DisplayColor _displayColor;

        private void OnEnable()
        {
            if (Extensions.IsMacOs())
            {
                MonoBehaviour[] components = GetComponentsInChildren<MonoBehaviour>();
               
                foreach (MonoBehaviour behaviour in components)
                {
                    behaviour.enabled = false;
                }
            }
        }

        public void Get<T>(out T component) where T : class
        {
            if (typeof(T) == typeof(DisplayColor))
            {
                component = _displayColor as T;
            }
            else
            {
                component = default;
            }
        }
    }
}