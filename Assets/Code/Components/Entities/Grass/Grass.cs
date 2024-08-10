using Code.Components.Common;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Entities.Grass
{
    public class Grass : Entity
    {
        [SerializeField] private GrassAnimator _grassAnimator;
        public bool IsActive{ get; private set; }

        public void Grow()
        {
            IsActive = true;
            _grassAnimator.PlayGrow();
            Debugging.Instance.Log($"Grow",Debugging.Type.Grass);
        }

        public void Die()
        {
            IsActive = false;
            Debugging.Instance.Log($"Die",Debugging.Type.Grass);
            _grassAnimator.PlayDie();
        }

        #region Editor

        public void FindAllComponents()
        {
            _commonComponents = GetComponentsInChildren<CommonComponent>(true);
        }

        #endregion
    }
}