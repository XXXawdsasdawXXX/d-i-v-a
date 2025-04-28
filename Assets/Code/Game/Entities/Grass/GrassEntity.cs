using Code.Game.Entities.Common;
using Code.Utils;
using UnityEngine;

namespace Code.Game.Entities.Grass
{
    public class GrassEntity : Entity
    {
        public bool IsActive { get; private set; }
        
        [SerializeField] private GrassAnimator _grassAnimator;

        public void Grow()
        {
            IsActive = true;
            _grassAnimator.PlayGrow();
            Log.Info(this, "[Grow]", Log.Type.Grass);
        }

        public void Die()
        {
            IsActive = false;
            Log.Info(this, "[Die]", Log.Type.Grass);
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