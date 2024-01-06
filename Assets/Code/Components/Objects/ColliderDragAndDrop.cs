using Code.Infrastructure.GameLoop;
using Code.Services;
using UnityEngine;

namespace Code.Components.Objects
{
    public class ColliderDragAndDrop : MonoBehaviour, IGameTickListener
    {
        [SerializeField] private ColliderButton _colliderButton;

        public void GameTick()
        {
            if (_colliderButton.IsPressed)
            {
                Vector3 pos = PositionService.GetMouseWorldPosition();
                transform.position = pos;
            }
        }
    }
}