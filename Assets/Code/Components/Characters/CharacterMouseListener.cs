using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Characters
{
    public class CharacterMouseListener : MonoBehaviour, IGameStartListener,IGameTickListener
    {
        [SerializeField] private CharacterAnimator _characterAnimator;
        [SerializeField] private float _centralNormalValue = 0.3f;

        public Vector2 _normal;
        public Vector2 _offset;


        public void GameStart()
        {
        // _characterAnimator.StartPlayReactionMouse();   
        }

        public void GameTick()
        {
            var normal = (PositionService.GetMouseWorldPosition() - (transform.position + _offset.AsVector3())).normalized;
            _normal = normal;
            //_normal = new Vector2(roundedX,roundedY);
            var roundedX = Mathf.Abs(normal.x) < _centralNormalValue ? 0 : (normal.x < 0 ? -1 : 1);
            var roundedY = Mathf.Abs(normal.y) < _centralNormalValue ? 0 : (normal.y < 0 ? -1 : 1);
            _characterAnimator.SetMouseNormal(roundedX,roundedY);
        }
    }
}