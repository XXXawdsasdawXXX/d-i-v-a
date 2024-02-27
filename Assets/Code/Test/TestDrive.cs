using System;
using System.Collections.Generic;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using UnityEngine;

namespace Code.Test
{
    public class TestDrive : MonoBehaviour, IService ,IGameInitListener,IGameTickListener
    {
        [SerializeField] private GameObject _testObject;
        [SerializeField] private int _currentAnchorIndex;
        private PositionService _positionService;

        public void GameInit()
        {
            _positionService = Container.Instance.FindService<PositionService>();
        }

        public void GameTick()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                MoveToNextPosition();
            }
        }

        public void MoveToNextPosition()
        {
            SetNextIndex();
            _testObject.transform.position = _positionService.GetPosition((PointAnchor)_currentAnchorIndex);
        }

        private void SetNextIndex()
        {
            var lenght = Enum.GetNames(typeof(PointAnchor)).Length;
            _currentAnchorIndex++;
            if (_currentAnchorIndex >= lenght)
            {
                _currentAnchorIndex = 0;
            }
            Debug.Log($"Set next anchor point {(PointAnchor)_currentAnchorIndex}");
        }
    }
}