using System;
using System.Collections;
using System.Linq;
using Code.Components.Common;
using Code.Infrastructure.GameLoop;
using Code.Utils;
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

        [Header("Dynamic value")]
        [SerializeField] protected bool _isUsing;
        [field :SerializeField] public bool IsReady { get; private set; }

        //Events
        public  Action ReadyForUseEvent;        
        public Action<Item> UseEvent;
        public  Action DieEvent;
        
        
        public abstract void Use(Action OnEnd = null);

        public virtual void Reset()
        {
            IsReady = false;
            _isUsing = false;
        }

        public virtual void ReadyForUse(Vector3 position)
        {
            _isUsing = true;
            ReadyForUseEvent?.Invoke();
            StartCoroutine(MoveToPoint(position));
        }

        private IEnumerator MoveToPoint(Vector3 position)
        {
            var period = new WaitForEndOfFrame();
            while (Vector3.Distance(transform.position, position) > _stoppedDistance)
            {
                Debugging.Instance.Log($"[Move to point] " +
                $"осталось двигаться {Vector3.Distance(transform.position, position) - _stoppedDistance}", Debugging.Type.Items);
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