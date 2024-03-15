using System.Collections;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Objects
{
    public class ColorChecker : CommonComponent, IGameInitListener, IGameStartListener
    {
        [SerializeField] private bool _isCheck;
        [SerializeField] private Color _lastColor = Color.red;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        private DesktopColorAnalyzer _desktopColorAnalyzer;
        private PositionService _positionService;

        public void GameInit()
        {
            _desktopColorAnalyzer = Container.Instance.FindService<DesktopColorAnalyzer>();
            _positionService = Container.Instance.FindService<PositionService>();
        }


        public void GameStart()
        {
           // _lastColor = _desktopColorAnalyzer.GetColor(GetScreenPosition());
            StartCoroutine(StartCheck());
        }


        public IEnumerator StartCheck()
        {
            if (_lastColor == Color.red)
            {
                yield return new WaitForEndOfFrame();
                _lastColor = _desktopColorAnalyzer.GetColor(GetScreenPosition());
            }
            while (_isCheck)
            {
                yield return new WaitForEndOfFrame();
                if (_desktopColorAnalyzer.IsCurrentColor(GetScreenPosition(), _lastColor, out var newColor))
                {
                    yield return new WaitForSeconds(0.1f);
                }
                else
                {
                    _lastColor = newColor;
                    _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                    _rigidbody2D.velocity = Vector2.zero;
                    Debugging.Instance.Log("COOOOLOOOOOR!!!");
                }
            }
        }

        private Vector2 GetScreenPosition()
        {
            return _positionService.WorldToScreen(transform.position);
        }
    }
}