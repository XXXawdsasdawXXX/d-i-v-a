using Code.Game.Services;
using Code.Infrastructure.ServiceLocator;
using Code.Utils;
using UnityEngine;

namespace Code.Game.Views
{
    public class DisplayColorView : MonoBehaviour, IView
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