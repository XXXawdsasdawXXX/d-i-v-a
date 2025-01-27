using Code.Infrastructure.GameLoop;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Test
{
    public class DebugWindow : MonoBehaviour, IStartListener, IUpdateListener, IExitListener
    {
        [SerializeField] private GameObject _body;

        [Header("debug panels")] 
        [SerializeField] private DW_Param _param;
        [SerializeField] private DW_Tick _tick;
        [SerializeField] private DW_TimeScale _timeScale;
        [SerializeField] private DW_Version _version;
        [SerializeField] private DW_Profiler _profiler;
        
        private bool _isOpened;
        
        public async UniTask GameStart()
        {
            await _param.Initialize();
            await _tick.Initialize();
            await _timeScale.Initialize();
            await _version.Initialize();
            
            _isOpened = _body.activeSelf;
        }

        public void GameUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _isOpened = !_isOpened;
                _body.SetActive(_isOpened);
            }

            if (_isOpened)
            {
                _param.Refresh();
                _tick.Refresh();
                _profiler.Refresh();
            }
        }

        public void GameExit()
        {
            _param.Dispose();
            _timeScale.Dispose();
        }
    }
}