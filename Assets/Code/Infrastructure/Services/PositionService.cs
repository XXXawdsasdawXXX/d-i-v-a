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

        private readonly Vector2 _offset = new(75,  75);
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
                PointAnchor.UpperLeft => GetUpperLeftPosition(entityBounds.Size, entityBounds.Center),
                PointAnchor.UpperCenter => GetUpperCenterPosition(entityBounds.Size, entityBounds.Center),
                PointAnchor.UpperRight => GetUpperRightPosition(entityBounds.Size, entityBounds.Center),
                PointAnchor.MiddleLeft => GetMiddleLeftPosition(entityBounds.Size, entityBounds.Center),
                PointAnchor.MiddleRight => GetMiddleRightPosition(entityBounds.Size, entityBounds.Center),
                PointAnchor.LowerLeft => GetLowerLeftPosition(entityBounds.Size, entityBounds.Center),
                PointAnchor.LowerCenter => GetLowerCenterPosition(entityBounds.Size, entityBounds.Center),
                PointAnchor.LowerRight => GetLowerRightPosition(entityBounds.Size, entityBounds.Center),
                PointAnchor.Center => GetCenterPosition(entityBounds.Size, entityBounds.Center),
                _ => Vector3.zero
            };
        }

        #region Base methods

        private Vector2 GetScreenSize()
        {
            return _canvas.sizeDelta - _offset;
        }

        public Vector3 GetMouseWorldPosition()
        {
            Vector3 position = ScreenToWorld(Input.mousePosition);
            return position;
        }

        private Vector3 ScreenToWorld(Vector2 screenPoint)
        {
            Vector3 worldPoint = _perfectCamera != null && _perfectCamera.enabled
                ? _perfectCamera.RoundToPixel(_camera.ScreenToWorldPoint(screenPoint))
                : _camera.ScreenToWorldPoint(screenPoint);
            return new Vector3(worldPoint.x, worldPoint.y, 0);
        }

        #endregion

        //upper


        private Vector3 GetUpperLeftPosition(Vector2 size, Vector2 center) =>
            ScreenToWorld(new Vector2(size.x, GetScreenSize().y - size.y)) + center.AsVector3();

        private Vector3 GetUpperCenterPosition(Vector2 size, Vector2 center) =>
            ScreenToWorld(new Vector2(GetScreenSize().x / 2, GetScreenSize().y - size.y)) + center.AsVector3();

        private Vector3 GetUpperRightPosition(Vector2 size, Vector2 center) =>
            ScreenToWorld(new Vector2(GetScreenSize().x - size.x, GetScreenSize().y - size.y)) + center.AsVector3();

        //middle

        private Vector3 GetCenterPosition(Vector2 size, Vector2 center) =>
            ScreenToWorld(new Vector2(GetScreenSize().x / 2, GetScreenSize().y / 2)) + center.AsVector3();

        private Vector3 GetMiddleRightPosition(Vector2 size, Vector2 center) =>
            ScreenToWorld(new Vector2(GetScreenSize().x - size.x, GetScreenSize().y / 2)) + center.AsVector3();

        private Vector3 GetMiddleLeftPosition(Vector2 size, Vector2 center) =>
            ScreenToWorld(new Vector2(size.x, GetScreenSize().y / 2)) + center.AsVector3();

        //lower


        private Vector3 GetLowerCenterPosition(Vector2 size, Vector2 center) =>
            ScreenToWorld(new Vector2(GetScreenSize().x / 2, 0)) + center.AsVector3();

        private Vector3 GetLowerLeftPosition(Vector2 size, Vector2 center) =>
            ScreenToWorld(new Vector2(size.x, 0)) + center.AsVector3();

        private Vector3 GetLowerRightPosition(Vector2 size, Vector2 center) =>
            ScreenToWorld(new Vector2(GetScreenSize().x - size.x, 0)) + center.AsVector3();


        public Vector2 WorldToScreen(Vector3 transformPosition)
        {
            return _camera.WorldToScreenPoint(transformPosition);
        }
        
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
    }
}