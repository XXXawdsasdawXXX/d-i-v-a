using Code.Data.Enums;
using Code.Data.StaticData;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Common
{
    public class PositionInitializer : CommonComponent, IGameInitListener, IGameStartListener
    {
        [SerializeField] private EPointAnchor _pointAnchor;
        [SerializeField] private Vector2 _offset;
        [SerializeField] private EntityBounds _entityBounds;

        private PositionService _positionService;

        public void GameInit()
        {
            _positionService = Container.Instance.FindService<PositionService>();
        }

        public void GameStart()
        {
            SetDefaultPosition();
        }

        public void SetDefaultPosition()
        {
            Debugging.Log(
                $"[{gameObject.name}] from {transform.position} to {_positionService.GetPosition(_pointAnchor, _entityBounds)}",
                Debugging.Type.Position);
            transform.position = _positionService.GetPosition(_pointAnchor, _entityBounds);
        }
    }
}