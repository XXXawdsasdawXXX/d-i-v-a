using Code.Data.Interfaces;
using Code.Infrastructure.GameLoop;
using Code.Services;
using UnityEngine;

namespace Code.Components.Objects
{
    public class ColliderDragAndDrop : CommonComponent, IGameTickListener, IActivated
    {
        [SerializeField] private ColliderButton _colliderButton;

        private bool _isActive;
        public void GameTick()
        {
            if (_isActive && _colliderButton.IsPressed)
            {
                Vector3 pos = PositionService.GetMouseWorldPosition();
                transform.position = pos;
            }
        }
        
        public void Activate()
        {
            _isActive = true;
        }

        public void Deactivate()
        {
            _isActive = false;
        }
    }
}