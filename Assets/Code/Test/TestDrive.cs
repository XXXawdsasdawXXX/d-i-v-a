using System;
using System.Collections;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Utils;
using Kirurobo;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Test
{
    public class TestDrive : MonoBehaviour, IService, IGameInitListener, IGameTickListener
    {
        [SerializeField] private bool _isTest;
        [SerializeField] private Image _image;
        [SerializeField] private UniWindowController _uniWindow;
        [SerializeField] private GameObject _testObject;
        [SerializeField] private int _currentAnchorIndex;
        private PositionService _positionService;

        private bool _isCheck;
        private DesktopColorAnalyzer _desktopColorAnalyzer;

        private Coroutine _coroutine;
        public void GameInit()
        {
            _positionService = Container.Instance.FindService<PositionService>();
            _desktopColorAnalyzer = Container.Instance.FindService<DesktopColorAnalyzer>();
           // StartCoroutine(SetColor());
        }

        private IEnumerator SetColor()
        {
            yield return new WaitForEndOfFrame();
            if (_uniWindow.TryGetColor(Input.mousePosition, out var color))
            {
                if (_desktopColorAnalyzer.CompareColors(_image.color, color))
                {
                    
                }
                else
                {
                    _image.color = color;
                      Debugging.Instance.Log("Other color");    
                }
            }

            _coroutine = null;
        }

        public void GameTick()
        {
            if (_positionService == null)
            {
                return;
            }

            /*if (Input.GetMouseButtonDown(0) && _coroutine == null)
            {
                _coroutine = StartCoroutine(SetColor());
            }*/

            _image.color = _uniWindow.pickedColor;
            /*if(_uniWindow.TryGetColor(_positionService.WorldToScreen(_testObject.transform.position), out var color))
            {
                _image.color = color;
            }
            */
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