using System.Collections.Generic;
using System.Linq;
using Code.Entities.Common;
using UnityEditor;
using UnityEngine;

namespace Code.Entities.Hand
{
    public class HandEntity : Entity
    {
        [SerializeField] private HandComponent[] _handComponents;

        public T FindHandComponent<T>() where T : HandComponent
        {
            foreach (HandComponent component in _handComponents)
            {
                if (component is T handComponent)
                {
                    return handComponent;
                }
            }

            return null;
        }
        
#if UNITY_EDITOR
        
        /// <summary>
        /// Editor
        /// </summary>
        public void FindAllComponents()
        {
            List<HandComponent> handComponents = GetComponents<HandComponent>().ToList();
            foreach (HandComponent componentsInChild in GetComponentsInChildren<HandComponent>())
            {
                if (!handComponents.Contains(componentsInChild))
                {
                    handComponents.Add(componentsInChild);
                }
            }

            _handComponents = handComponents.ToArray();


            List<CommonComponent> commonComponents = GetComponents<CommonComponent>().ToList();
            foreach (CommonComponent componentsInChild in GetComponentsInChildren<CommonComponent>())
            {
                if (!commonComponents.Contains(componentsInChild))
                {
                    commonComponents.Add(componentsInChild);
                }
            }

            _commonComponents = commonComponents.ToArray();

            EditorUtility.SetDirty(gameObject);
        }

#endif
    }
}