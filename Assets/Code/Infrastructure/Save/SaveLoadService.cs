using System.Collections.Generic;
using Code.Data.Interfaces;
using Code.Data.SavedData;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.Save
{
    public class SaveLoadService : IService, IGameLoadListener, IGameExitListener
    {
        private const string PROGRESS_KEY = "Progress";

        private PlayerProgressData _playerProgress;

        private List<IProgressWriter> _progressWriters = new();
        private List<IProgressReader> _progressReader = new();


        public void GameLoad()
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

            Debugging.Log($"Game load", Debugging.Type.SaveLoad);
        }

        public void GameExit()
        {
            SaveProgress();
            Debugging.Log($"Game save", Debugging.Type.SaveLoad);
        }

        private void SaveProgress()
        {
            foreach (IProgressWriter progressWriter in _progressWriters)
            {
                progressWriter.SaveProgress(_playerProgress);
            }

            PlayerPrefs.SetString(PROGRESS_KEY, _playerProgress.ToJson());

            string data = PlayerPrefs.GetString(PROGRESS_KEY);
            Debugging.Log($"Save progress -> " +
                          $"{_playerProgress != null} " +
                          $"{_playerProgress?.LiveStatesData != null}" +
                          $"{_playerProgress?.LiveStatesData?.Count}\n" +
                          $"{data} ", Debugging.Type.SaveLoad);
        }

        private void LoadProgress()
        {
            string data = PlayerPrefs.GetString(PROGRESS_KEY);
            _playerProgress = PlayerPrefs.GetString(PROGRESS_KEY)?.ToDeserialized<PlayerProgressData>();

            Debugging.Log($"Load progress -> " +
                          $"{_playerProgress != null} " +
                          $"{_playerProgress?.LiveStatesData != null}" +
                          $"{_playerProgress?.LiveStatesData?.Count}\n" +
                          $"{data} ", Debugging.Type.SaveLoad);

            _playerProgress ??= new PlayerProgressData();
            foreach (IProgressReader progressReader in _progressReader)
            {
                progressReader.LoadProgress(_playerProgress);
            }
        }
    }

    public interface IProgressWriter : IProgressReader
    {
        void SaveProgress(PlayerProgressData playerProgress);
    }

    public interface IProgressReader
    {
        void LoadProgress(PlayerProgressData playerProgress);
    }
}