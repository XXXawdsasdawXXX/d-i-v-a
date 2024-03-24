using System;
using System.Linq;
using Code.Components.Hands;
using Code.Components.Objects;
using UnityEngine;

namespace Code.Components.Items
{
    public abstract class Item : MonoBehaviour
    {
        [SerializeField] private CommonComponent[] _commonComponents;

        public Action<Item> UseEvent;
        public abstract void Use(Action OnEnd = null);

        public T FindCommonComponent<T>() where T : CommonComponent
        {
            foreach (var component in _commonComponents)
            {
                if (component is T commonComponent)
                {
                    return commonComponent;
                }
            }
            return null;
        }
        
        [ContextMenu("FindAllComponents")]
        public void FindAllComponents()
        {
            var commonComponents = GetComponents<CommonComponent>().ToList();
            foreach (var componentsInChild in GetComponentsInChildren<CommonComponent>())
            {
                if (!commonComponents.Contains(componentsInChild))
                {
                    commonComponents.Add(componentsInChild);
                }
            }
            _commonComponents = commonComponents.ToArray();
        }
    }
}