using System;
using System.Collections.Generic;
using Code.Data.Value;
using Code.Utils;
using UnityEditor;
using UnityEngine;

namespace Code.Infrastructure.Save
{
    public class SaveLoadService
    {
        private const string progressKey = "Progress";

        private readonly Progress _progress;

        private List<IProgressWriter> _progressWriters;
        private List<IProgressWriter> _progressReader;
        
        public void SaveProgress()
        {
            foreach (var progressWriter in  _progressWriters)
            {
                progressWriter.UpdateProgress(_progress);
            }

            PlayerPrefs.SetString(progressKey, _progress.ToJson());
        }

        public Progress LoadProgress() =>
            PlayerPrefs.GetString(progressKey)?.ToDeserialized<Progress>();
    
    }

    public interface IProgressWriter : IProgressReader
    {
        void UpdateProgress(Progress progress);
    }

    public interface IProgressReader
    {
        void LoadProgress(Progress progress);
    }

    [Serializable]
    public class Progress
    {
        public List<LiveStateSavedData> LiveStatesData;
    }
}