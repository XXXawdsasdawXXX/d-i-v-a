using Code.Data.Enums;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Borders
{
    public class PositionInitializer : MonoBehaviour, IGameStartListener
    {
        [SerializeField] private PointAnchor _pointAnchor;
        [SerializeField] private Vector2 _offset;

        public void GameStart()
        {
            transform.position = PositionService.GetPosition(_pointAnchor) + _offset.AsVector3();
        }
    }
}