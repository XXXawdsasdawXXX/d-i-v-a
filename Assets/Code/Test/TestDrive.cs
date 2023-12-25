using System;
using System.Collections.Generic;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Infrastructure.GameLoop;
using Code.Services;
using UnityEngine;

namespace Code.Test
{
    public class TestDrive : MonoBehaviour, IService ,IGameTickListener
    {
        [SerializeField] private GameObject _testObject;
        [SerializeField] private int _currentAnchorIndex;

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
            _testObject.transform.position = PositionService.GetPosition((PointAnchor)_currentAnchorIndex);
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