using System;
using System.Collections;
using System.Linq;
using Code.Components.Objects;
using Code.Infrastructure.GameLoop;
using Unity.VisualScripting;
using UnityEngine;

namespace Code.Components.Items
{
    public abstract class Item : MonoBehaviour
    {
        [SerializeField] private CommonComponent[] _commonComponents;
        [SerializeField] private float _stoppedDistance = 1;
        [SerializeField] private float _speed = 1;

        [Header("Debug")]
        [SerializeField] private float _distance;
        public bool IsReady { get; private set; }

        public  Action ReadyForUseEvent;
        
        public Action<Item> UseEvent;
        public  Action DieEvent;
        
        
        public abstract void Use(Action OnEnd = null);

        public virtual void Reset()
        {
            IsReady = false;
        }
        public virtual void ReadyForUse(Vector3 position)
        {
            ReadyForUseEvent?.Invoke();
            StartCoroutine(MoveToPoint(position));
        }

        private IEnumerator MoveToPoint(Vector3 position)
        {
            var period = new WaitForEndOfFrame();
            while (Vector3.Distance(transform.position, position) > _stoppedDistance)
            {
                transform.position = Vector3.Lerp(transform.position, position, _speed * Time.deltaTime);
                _distance = Vector3.Distance(transform.position, position);
                yield return period;
            }

            IsReady = true;
        }
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