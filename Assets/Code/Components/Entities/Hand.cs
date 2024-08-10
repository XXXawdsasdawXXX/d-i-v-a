using System.Linq;
using Code.Components.Common;
using Code.Components.Hands;
using UnityEngine;

namespace Code.Components.Entities
{
    public class Hand : Entity
    {
        [SerializeField] private HandComponent[] _handComponents;
        
        public T FindHandComponent<T>() where T : HandComponent
        {
            foreach (var component in _handComponents)
            {
                if (component is T handComponent)
                {
                    return handComponent;
                }
            }

            return null;
        }

        #region Editor

        public void FindAllComponents()
        {
            var handComponents = GetComponents<HandComponent>().ToList();
            foreach (var componentsInChild in GetComponentsInChildren<HandComponent>())
            {
                if (!handComponents.Contains(componentsInChild))
                {
                    handComponents.Add(componentsInChild);
                }
            }
            _handComponents = handComponents.ToArray();
            
            
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

        #endregion
    }
}