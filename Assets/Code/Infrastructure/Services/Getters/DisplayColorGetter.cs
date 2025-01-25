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

        public object Get()
        {
            return _displayColor;
        }
    }
}