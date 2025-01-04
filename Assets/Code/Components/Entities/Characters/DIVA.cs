using System.Collections.Generic;
using System.Linq;
using Code.Components.Common;
using Code.Infrastructure.Reactions;
using UnityEngine;

namespace Code.Components.Entities.Characters
{
    public class DIVA : Entity
    {
        [SerializeField] private CharacterComponent[] _characterComponent;

        public T FindCharacterComponent<T>() where T : CharacterComponent
        {
            foreach (CharacterComponent component in _characterComponent)
            {
                if (component is T characterComponent)
                {
                    return characterComponent;
                }
            }

            return null;
        }

        #region Editor

        public void FindAllComponents()
        {
            List<CharacterComponent> characterComponents = GetComponents<CharacterComponent>().ToList();
            foreach (CharacterComponent componentsInChild in GetComponentsInChildren<CharacterComponent>())
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
        }

        #endregion
    }
}