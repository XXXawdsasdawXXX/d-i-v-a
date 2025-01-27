using System.Collections.Generic;
using Code.Data;
using Code.Infrastructure.Save;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.BehaviorTree
{
    public class BehaviourTreeLoader : MonoBehaviour, IService, IProgressWriter
    {
        public class Data
        {
            public int SleepRemainingTick;
        }

        private readonly List<IProgressWriterNode> _progressWriterNodes = new();
        private readonly Data _data = new();

        public UniTask LoadProgress(PlayerProgressData playerProgress)
        {
            _data.SleepRemainingTick = playerProgress.Cooldowns.SleepRemainingTick;
         
            foreach (IProgressWriterNode progressWriterNode in _progressWriterNodes)
            {
                progressWriterNode.LoadData(_data);
            }
            
            return UniTask.CompletedTask;
        }

        public void SaveProgress(PlayerProgressData playerProgress)
        {
            foreach (IProgressWriterNode progressWriterNode in _progressWriterNodes)
            {
                progressWriterNode.UpdateData(_data);
            }

            playerProgress.Cooldowns.SleepRemainingTick = _data.SleepRemainingTick;
        }

        public void AddProgressWriter(IProgressWriterNode node)
        {
            if (!_progressWriterNodes.Contains(node))
            {
                _progressWriterNodes.Add(node);
            }
        }
    }
}