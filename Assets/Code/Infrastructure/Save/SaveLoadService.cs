using System.Collections.Generic;
using Code.Data.Interfaces;
using Code.Data.SavedData;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.Save
{
    public class SaveLoadService: IService, IGameLoadListener, IGameExitListener
    {
        private const string PROGRESS_KEY = "Progress";

        private  PlayerProgressData _playerProgress;

        private List<IProgressWriter> _progressWriters = new();
        private List<IProgressReader> _progressReader = new();
        
        
        public void GameLoad()
        {
         
            _progressReader = Container.Instance.GetProgressReaders();
            foreach (var progressReader in _progressReader)
            {
                if (progressReader is IProgressWriter writer)
                {
                    _progressWriters.Add(writer);
                }
            }
            
            LoadProgress();
            
            Debugging.Instance.Log($"Game load",Debugging.Type.SaveLoad);
        }

        public void GameExit()
        {
            SaveProgress();
            Debugging.Instance.Log($"Game save",Debugging.Type.SaveLoad);
        }

        private void SaveProgress()
        {
            foreach (var progressWriter in  _progressWriters)
            {
                progressWriter.UpdateProgress(_playerProgress);
            }

            PlayerPrefs.SetString(PROGRESS_KEY, _playerProgress.ToJson());
                 
            var data = PlayerPrefs.GetString(PROGRESS_KEY);
            Debugging.Instance.Log($"Save progress -> " +
                                   $"{_playerProgress != null} " +
                                   $"{_playerProgress?.LiveStatesData != null}" +
                                   $"{_playerProgress?.LiveStatesData?.Count}\n" +
                                   $"{data} ",Debugging.Type.SaveLoad);
        }

        private void LoadProgress()
        {
            var data = PlayerPrefs.GetString(PROGRESS_KEY);
            _playerProgress =  PlayerPrefs.GetString(PROGRESS_KEY)?.ToDeserialized<PlayerProgressData>();
            
            Debugging.Instance.Log($"Load progress -> " +
                                   $"{_playerProgress != null} " +
                                   $"{_playerProgress?.LiveStatesData != null}" +
                                   $"{_playerProgress?.LiveStatesData?.Count}\n" +
                                   $"{data} ",Debugging.Type.SaveLoad);
            
            _playerProgress ??= new PlayerProgressData();
            foreach (var progressReader in _progressReader)
            {
                progressReader.LoadProgress(_playerProgress);
            }
        }
        


    }

    public interface IProgressWriter : IProgressReader
    {
        void UpdateProgress(PlayerProgressData playerProgress);
    }

    public interface IProgressReader
    {
        void LoadProgress(PlayerProgressData playerProgress);
    }
}