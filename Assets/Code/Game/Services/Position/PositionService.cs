using System;
using System.Linq;
using Code.Data;
using Code.Game.Views;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.ServiceLocator;
using Code.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Game.Services.Position
{
    public class PositionService : MonoBehaviour, IService, IInitializeListener
    {
        [SerializeField] private RectTransform _canvas;

        private readonly Vector2 _offset = new(75, 95);
        private CameraView _camera;

        public UniTask GameInitialize()
        {
            _camera = Container.Instance.GetView<CameraView>();
            
            return UniTask.CompletedTask;
        }

        public Vector3 GetPosition(EPointAnchor pointAnchor, EntityBounds entityBounds = null)
        {
            entityBounds ??= new EntityBounds();

            return pointAnchor switch
            {
                EPointAnchor.UpperLeft => _getUpperLeftPosition(entityBounds.Size, entityBounds.Center),
                EPointAnchor.UpperCenter => _getUpperCenterPosition(entityBounds.Size, entityBounds.Center),
                EPointAnchor.UpperRight => _getUpperRightPosition(entityBounds.Size, entityBounds.Center),
                EPointAnchor.MiddleLeft => _getMiddleLeftPosition(entityBounds.Size, entityBounds.Center),
                EPointAnchor.MiddleRight => _getMiddleRightPosition(entityBounds.Size, entityBounds.Center),
                EPointAnchor.LowerLeft => _getLowerLeftPosition(entityBounds.Size, entityBounds.Center),
                EPointAnchor.LowerCenter => _getLowerCenterPosition(entityBounds.Size, entityBounds.Center),
                EPointAnchor.LowerRight => _getLowerRightPosition(entityBounds.Size, entityBounds.Center),
                EPointAnchor.Center => _getCenterPosition(entityBounds.Size, entityBounds.Center),
                _ => Vector3.zero
            };
        }
        
        public Vector2 WorldToScreen(Vector3 transformPosition) =>
            _camera.WorldToScreen(transformPosition);

        public bool TryGetRandomDistantPosition(Vector3 targetPosition, float minDistance, out Vector3 resultPosition)
        {
            resultPosition = Vector3.zero;
            
            EPointAnchor[] posTypes = Enum.GetValues(typeof(EPointAnchor)).Cast<EPointAnchor>().ToArray();
            Extensions.ShuffleArray(posTypes);
            
            foreach (EPointAnchor pointAnchor in posTypes)
            {
                resultPosition = GetPosition(pointAnchor);
                if (Vector3.Distance(targetPosition, resultPosition) >= minDistance)
                {
                    return true;
                }
            }
            
            return false;
        }

        public Vector3 GetMouseWorldPosition()
        {
            Vector3 position = _screenToWorld(Input.mousePosition);
            return position;
        }
        
        private Vector2 _getScreenSize()
        {
            return _canvas.sizeDelta - _offset;
        }
        
        private Vector3 _screenToWorld(Vector2 screenPoint)
        {
            /*Vector3 worldPoint = _perfectCamera != null && _perfectCamera.enabled
                ? _perfectCamera.RoundToPixel(_camera.ScreenToWorldPoint(screenPoint))
                : _camera.ScreenToWorldPoint(screenPoint);*/
            return _camera.WorldToScreen(screenPoint);
        }

        //upper
        
        private Vector3 _getUpperLeftPosition(Vector2 size, Vector2 center) =>
            _screenToWorld(new Vector2(size.x, _getScreenSize().y - size.y)) + center.AsVector3();

        private Vector3 _getUpperCenterPosition(Vector2 size, Vector2 center) =>
            _screenToWorld(new Vector2(_getScreenSize().x / 2, _getScreenSize().y - size.y)) + center.AsVector3();

        private Vector3 _getUpperRightPosition(Vector2 size, Vector2 center) =>
            _screenToWorld(new Vector2(_getScreenSize().x - size.x, _getScreenSize().y - size.y)) + center.AsVector3();

        //middle

        private Vector3 _getCenterPosition(Vector2 size, Vector2 center) =>
            _screenToWorld(new Vector2(_getScreenSize().x / 2, _getScreenSize().y / 2)) + center.AsVector3();

        private Vector3 _getMiddleRightPosition(Vector2 size, Vector2 center) =>
            _screenToWorld(new Vector2(_getScreenSize().x - size.x, _getScreenSize().y / 2)) + center.AsVector3();

        private Vector3 _getMiddleLeftPosition(Vector2 size, Vector2 center) =>
            _screenToWorld(new Vector2(size.x, _getScreenSize().y / 2)) + center.AsVector3();

        //lower
        
        private Vector3 _getLowerCenterPosition(Vector2 size, Vector2 center) =>
            _screenToWorld(new Vector2(_getScreenSize().x / 2, _offset.y)) + center.AsVector3();

        private Vector3 _getLowerLeftPosition(Vector2 size, Vector2 center) =>
            _screenToWorld(new Vector2(size.x, _offset.y)) + center.AsVector3();

        private Vector3 _getLowerRightPosition(Vector2 size, Vector2 center) =>
            _screenToWorld(new Vector2(_getScreenSize().x - size.x, _offset.y)) + center.AsVector3();
        
    }
}