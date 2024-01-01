using Code.Infrastructure.GameLoop;
using Code.Services;
using UnityEngine;

namespace Code.Components.Objects
{
    public class DragAndDropSprite : MonoBehaviour, IGameTickListener
    {
        [SerializeField] private ButtonSprite _buttonSprite;

        public void GameTick()
        {
            if (_buttonSprite.IsPressed)
            {
                Vector3 pos = PositionService.GetMouseWorldPosition();
                transform.position = pos;
            }
        }
    }
}