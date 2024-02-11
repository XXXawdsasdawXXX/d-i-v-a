using System.Linq;
using Code.Components.Character.LiveState;
using Code.Components.Characters.Reactions;
using Code.Components.Objects;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components.Characters
{
    public class Character : Entity
    {
        [SerializeField] private CharacterComponent[] _characterComponent;
        [SerializeField] private CommonComponent[] _commonComponent;
        [SerializeField] private CharacterReaction[] _reactions;


        public T FindCommonComponent<T>() where T : CommonComponent
        {
            foreach (var component in _commonComponent)
            {
                if (component is T commonComponent)
                {
                    return commonComponent;
                }
            }

            return null;
        }

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

        public T FindReaction<T>() where T : CharacterReaction
        {
            foreach (var reaction in _reactions)
            {
                if (reaction is T characterReaction)
                {
                    return characterReaction;
                }
            }

            return null;
        }

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
            _commonComponent = commonComponents.ToArray();
            
            var reactions = GetComponents<CharacterReaction>().ToList();
            foreach (var componentsInChild in GetComponentsInChildren<CharacterReaction>())
            {
                if (!reactions.Contains(componentsInChild))
                {
                    reactions.Add(componentsInChild);
                }
            }
             _reactions = reactions.ToArray();
        }
    }
}