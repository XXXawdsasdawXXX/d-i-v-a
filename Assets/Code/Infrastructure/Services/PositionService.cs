using System;
using System.Linq;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Data.StaticData;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Getters;
using Code.Utils;
using UnityEngine;
using UnityEngine.U2D;

namespace Code.Infrastructure.Services
{
    public class PositionService : MonoBehaviour, IService, IGameInitListener
    {
        [SerializeField] private RectTransform _canvas;

        private readonly Vector2 _offset = new(75, 95);
        private PixelPerfectCamera _perfectCamera;
        private Camera _camera;

        public void GameInit()
        {
            _camera = Container.Instance.FindGetter<CameraGetter>().Get() as Camera;
            _perfectCamera = Container.Instance.FindGetter<PixelPerfectGetter>().Get() as PixelPerfectCamera;
        }

        public Vector3 GetPosition(PointAnchor pointAnchor, EntityBounds entityBounds = null)
        {
            entityBounds ??= new EntityBounds();

            return pointAnchor switch
            {
                PointAnchor.UpperLeft => _getUpperLeftPosition(entityBounds.Size, entityBounds.Center),
                PointAnchor.UpperCenter => _getUpperCenterPosition(entityBounds.Size, entityBounds.Center),
                PointAnchor.UpperRight => _getUpperRightPosition(entityBounds.Size, entityBounds.Center),
                PointAnchor.MiddleLeft => _getMiddleLeftPosition(entityBounds.Size, entityBounds.Center),
                PointAnchor.MiddleRight => _getMiddleRightPosition(entityBounds.Size, entityBounds.Center),
                PointAnchor.LowerLeft => _getLowerLeftPosition(entityBounds.Size, entityBounds.Center),
                PointAnchor.LowerCenter => _getLowerCenterPosition(entityBounds.Size, entityBounds.Center),
                PointAnchor.LowerRight => _getLowerRightPosition(entityBounds.Size, entityBounds.Center),
                PointAnchor.Center => _getCenterPosition(entityBounds.Size, entityBounds.Center),
                _ => Vector3.zero
            };
        }
        
        public Vector2 WorldToScreen(Vector3 transformPosition) =>
            _camera.WorldToScreenPoint(transformPosition);

        public bool TryGetRandomDistantPosition(Vector3 targetPosition, float minDistance, out Vector3 resultPosition)
        {
            resultPosition = Vector3.zero;
            
            PointAnchor[] posTypes = Enum.GetValues(typeof(PointAnchor)).Cast<PointAnchor>().ToArray();
            Extensions.ShuffleArray(posTypes);
            
            foreach (PointAnchor pointAnchor in posTypes)
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
            Vector3 worldPoint = _perfectCamera != null && _perfectCamera.enabled
                ? _perfectCamera.RoundToPixel(_camera.ScreenToWorldPoint(screenPoint))
                : _camera.ScreenToWorldPoint(screenPoint);
            return new Vector3(worldPoint.x, worldPoint.y, 0);
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