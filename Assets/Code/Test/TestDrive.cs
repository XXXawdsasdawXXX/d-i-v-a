using System;
using Code.Enums;
using UnityEngine;

namespace Code
{
    public class TestDrive : MonoBehaviour
    {
        [SerializeField] private GameObject _testObject;
        [SerializeField] private int _currentAnchorIndex;

        private void Update()
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