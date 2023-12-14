using System.Collections.Generic;
using UnityEngine;

namespace Code.Infrastructure.DI
{
    public class Container : MonoBehaviour
    {
        public static Container Instance;

        [SerializeField] private List<ScriptableObject> _configs;

        private void Awake()
        {
            Instance = this;
        }
        
        public T FindConfig<T>() where  T : ScriptableObject
        {
            foreach (var panel in _configs)
            {
                if (panel is T uiPanel)
                {
                    return uiPanel;
                }
            }
            return null;
        }
    }
}