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
            Log.Info(this, "[Grow]", Log.Type.Grass);
#endif
        }

        public void Die()
        {
            IsActive = false;
#if DEBUGGING
            Log.Info(this, "[Die]", Log.Type.Grass);
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