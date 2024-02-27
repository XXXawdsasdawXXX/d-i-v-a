using Code.Data.Interfaces;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using UnityEngine;

namespace Code.Components.Objects
{
    public class ColliderDragAndDrop : CommonComponent, IGameInitListener,IGameTickListener, IActivated
    {
        [SerializeField] private ColliderButton _colliderButton;

        private bool _isActive;
        private PositionService _positionService;

        public void GameInit()
        {
            _positionService = Container.Instance.FindService<PositionService>();
        }

        public void GameTick()
        {
            if (_isActive && _colliderButton.IsPressed)
            {
                Vector3 pos = _positionService.GetMouseWorldPosition();
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