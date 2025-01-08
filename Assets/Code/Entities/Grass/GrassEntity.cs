using Code.Entities.Common;
using Code.Utils;
using UnityEngine;

namespace Code.Entities.Grass
{
    public class GrassEntity : Entity
    {
        public bool IsActive { get; private set; }
        
        [SerializeField] private GrassAnimator _grassAnimator;

        public void Grow()
        {
            IsActive = true;
            _grassAnimator.PlayGrow();
#if DEBUGGING
            Debugging.Log(this, "[Grow]", Debugging.Type.Grass);
#endif
        }

        public void Die()
        {
            IsActive = false;
#if DEBUGGING
            Debugging.Log(this, "[Die]", Debugging.Type.Grass);
#endif
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