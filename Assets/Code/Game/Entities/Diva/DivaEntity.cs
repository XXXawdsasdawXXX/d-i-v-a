using System.Collections.Generic;
using System.Linq;
using Code.Game.Entities.Common;
using Code.Game.Entities.Diva.Reactions;
using UnityEditor;
using UnityEngine;

namespace Code.Game.Entities.Diva
{
    public class DivaEntity : Entity
    {
        [SerializeField] private DivaComponent[] _characterComponent;

        public T FindCharacterComponent<T>() where T : DivaComponent
        {
            foreach (DivaComponent component in _characterComponent)
            {
                if (component is T characterComponent)
                {
                    return characterComponent;
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
            List<DivaComponent> characterComponents = GetComponents<DivaComponent>().ToList();
            foreach (DivaComponent componentsInChild in GetComponentsInChildren<DivaComponent>())
            {
                if (!characterComponents.Contains(componentsInChild))
                {
                    characterComponents.Add(componentsInChild);
                }
            }

            _characterComponent = characterComponents.ToArray();

            List<CommonComponent> commonComponents = GetComponents<CommonComponent>().ToList();
            foreach (CommonComponent componentsInChild in GetComponentsInChildren<CommonComponent>())
            {
                if (!commonComponents.Contains(componentsInChild))
                {
                    commonComponents.Add(componentsInChild);
                }
            }

            _commonComponents = commonComponents.ToArray();

            List<Reaction> reactions = GetComponents<Reaction>().ToList();
            foreach (Reaction componentsInChild in GetComponentsInChildren<Reaction>())
            {
                if (!reactions.Contains(componentsInChild))
                {
                    reactions.Add(componentsInChild);
                }
            }
            
            EditorUtility.SetDirty(gameObject);
        }
        
#endif
    }
}