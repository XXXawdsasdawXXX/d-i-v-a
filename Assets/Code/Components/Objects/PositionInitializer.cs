using Code.Data.Enums;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Borders
{
    public class PositionInitializer : MonoBehaviour, IGameInitListener, IGameStartListener
    {
        [SerializeField] private PointAnchor _pointAnchor;
        [SerializeField] private Vector2 _offset;
        public void GameInit()
        {
            Debugging.Instance.Log($"PositionInitializer Init -> {_pointAnchor}",Debugging.Type.Position);
            transform.position = PositionService.GetPosition(_pointAnchor) + _offset.AsVector3();
        }


        public void GameStart()
        {
            Debugging.Instance.Log($"PositionInitializer Start -> {_pointAnchor}",Debugging.Type.Position);
            transform.position = PositionService.GetPosition(_pointAnchor) + _offset.AsVector3();
        }
    }
}