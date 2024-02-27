using Code.Data.Enums;
using Code.Data.StaticData;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Objects
{
    public class PositionInitializer : CommonComponent, IGameInitListener,IGameStartListener
    {
        [SerializeField] private PointAnchor _pointAnchor;
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
            Debugging.Instance.Log($"[{gameObject.name}] from {transform.position} to { _positionService.GetPosition(_pointAnchor,_entityBounds)}",Debugging.Type.Position);
            transform.position = _positionService.GetPosition(_pointAnchor,_entityBounds);
        }
        
        
    }
}