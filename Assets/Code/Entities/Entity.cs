using Code.Entities.Common;
using UnityEngine;

namespace Code.Entities
{
    public abstract class Entity : MonoBehaviour
    {
        [SerializeField] protected CommonComponent[] _commonComponents;

        public T FindCommonComponent<T>() where T : CommonComponent
        {
            foreach (CommonComponent component in _commonComponents)
            {
                if (component is T commonComponent)
                {
                    return commonComponent;
                }
            }

            return null;
        }
    }
}