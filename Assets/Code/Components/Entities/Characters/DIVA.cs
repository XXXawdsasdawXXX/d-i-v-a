using System.Linq;
using Code.Components.Common;
using Code.Components.Entities.Characters.Reactions;
using UnityEngine;

namespace Code.Components.Entities.Characters
{
    public class DIVA : Entity
    {
        [SerializeField] private CharacterComponent[] _characterComponent;

        public T FindCharacterComponent<T>() where T : CharacterComponent
        {
            foreach (var component in _characterComponent)
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
            var characterComponents = GetComponents<CharacterComponent>().ToList();
            foreach (var componentsInChild in GetComponentsInChildren<CharacterComponent>())
            {
                if (!characterComponents.Contains(componentsInChild))
                {
                    characterComponents.Add(componentsInChild);
                }
            }

            _characterComponent = characterComponents.ToArray();

            var commonComponents = GetComponents<CommonComponent>().ToList();
            foreach (var componentsInChild in GetComponentsInChildren<CommonComponent>())
            {
                if (!commonComponents.Contains(componentsInChild))
                {
                    commonComponents.Add(componentsInChild);
                }
            }

            _commonComponents = commonComponents.ToArray();

            var reactions = GetComponents<Reaction>().ToList();
            foreach (var componentsInChild in GetComponentsInChildren<Reaction>())
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