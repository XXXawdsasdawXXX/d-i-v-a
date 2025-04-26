using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Code.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Entities.Common
{
    public class PositionInitializer : CommonComponent, IInitListener, IStartListener
    {
        [SerializeField] private EPointAnchor _pointAnchor;
        [SerializeField] private Vector2 _offset;
        [SerializeField] private EntityBounds _entityBounds;

        private PositionService _positionService;

        public UniTask GameInitialize()
        {
            _positionService = Container.Instance.GetService<PositionService>();
            
            return UniTask.CompletedTask;
        }

        public UniTask GameStart()
        {
            SetDefaultPosition();
            
            return UniTask.CompletedTask;
        }

        public void SetDefaultPosition()
        {
            transform.position = _positionService.GetPosition(_pointAnchor, _entityBounds);

            Log.Info(this,
                $"[{gameObject.name}] from {transform.position} to {_positionService.GetPosition(_pointAnchor, _entityBounds)}",
                Log.Type.Position);
        }
    }
}