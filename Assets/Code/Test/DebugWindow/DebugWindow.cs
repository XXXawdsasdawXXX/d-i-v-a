using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Test
{
    public class DebugWindow : MonoBehaviour, IGameStartListener, IGameUpdateListener
    {
        [SerializeField] private GameObject _body;
        [SerializeField] private GameObject _console;

        [Header("debug panels")] 
        [SerializeField] private DW_Param _param;

        private bool _isOpened;
        
        public async void GameStart()
        {
            await _param.Initialize();

            _isOpened = _body.activeSelf;
        }

        public void GameUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _isOpened = !_isOpened;
                _body.SetActive(_isOpened);
                _console.SetActive(_isOpened);
            }

            if (_isOpened)
            {
                _param.Refresh();
            }
        }
    }
}