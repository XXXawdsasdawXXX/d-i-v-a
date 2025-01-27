using System.Collections.Generic;
using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace Code.Infrastructure.Save
{
    [Preserve]
    public class SaveLoadService : IService, ILoadListener, IExitListener
    {
        private const string PROGRESS_KEY = "Progress";
        
        private readonly List<IProgressWriter> _progressWriters = new();
        private List<IProgressReader> _progressReader = new();

        private PlayerProgressData _playerProgress;

        public UniTask GameLoad()
        {
            _progressReader = Container.Instance.GetProgressReaders();
      
            foreach (IProgressReader progressReader in _progressReader)
            {
                if (progressReader is IProgressWriter writer)
                {
                    _progressWriters.Add(writer);
                }
            }

            LoadProgress();

#if DEBUGGING
            Debugging.Log(this, "[Game load]", Debugging.Type.SaveLoad);
#endif
            
            return UniTask.CompletedTask;
        }

        public void GameExit()
        {
            _saveProgress();
            
#if DEBUGGING
            Debugging.Log(this, "[Game save]", Debugging.Type.SaveLoad);
#endif
        }

        private void _saveProgress()
        {
            foreach (IProgressWriter progressWriter in _progressWriters)
            {
                progressWriter.SaveProgress(_playerProgress);
            }

            PlayerPrefs.SetString(PROGRESS_KEY, _playerProgress.ToJson());

            string data = PlayerPrefs.GetString(PROGRESS_KEY);
#if DEBUGGING
            Debugging.Log($"Save progress -> " +
                          $"{_playerProgress != null} " +
                          $"{_playerProgress?.LiveStatesData != null}" +
                          $"{_playerProgress?.LiveStatesData?.Count}\n" +
                          $"{data} ", Debugging.Type.SaveLoad);
#endif
        }

        private void LoadProgress()
        {
            string data = PlayerPrefs.GetString(PROGRESS_KEY);
            _playerProgress = PlayerPrefs.GetString(PROGRESS_KEY)?.ToDeserialized<PlayerProgressData>();
#if DEBUGGING
            Debugging.Log($"Load progress -> " +
                          $"{_playerProgress != null} " +
                          $"{_playerProgress?.LiveStatesData != null}" +
                          $"{_playerProgress?.LiveStatesData?.Count}\n" +
                          $"{data} ", Debugging.Type.SaveLoad);
#endif
            _playerProgress ??= new PlayerProgressData();
            foreach (IProgressReader progressReader in _progressReader)
            {
                progressReader.LoadProgress(_playerProgress);
            }
        }
    }
}