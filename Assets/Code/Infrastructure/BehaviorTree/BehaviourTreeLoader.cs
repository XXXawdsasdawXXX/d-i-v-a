using System.Collections;
using System.Collections.Generic;
using Code.Data.Interfaces;
using Code.Data.SavedData;
using Code.Infrastructure.Save;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree
{
    public class BehaviourTreeLoader : MonoBehaviour, IService, IProgressWriter
    {
        public class Data
        {
            public int SleepRemainingTick;
        }

        private readonly List<IProgressWriterNode> _progressWriterNodes = new();
        private readonly Data _data = new();

        public void AddProgressWriter(IProgressWriterNode node)
        {
            if (!_progressWriterNodes.Contains(node))
            {
                _progressWriterNodes.Add(node);
            }
        }

        public void LoadProgress(PlayerProgressData playerProgress)
        {
            _data.SleepRemainingTick = playerProgress.Cooldowns.SleepRemainingTick;
         
            StartCoroutine(_loadBehaviorTreeRoutine());
        }

        public void UpdateProgress(PlayerProgressData playerProgress)
        {
            foreach (IProgressWriterNode progressWriterNode in _progressWriterNodes)
            {
                progressWriterNode.UpdateData(_data);
            }

            playerProgress.Cooldowns.SleepRemainingTick = _data.SleepRemainingTick;
        }

        private IEnumerator _loadBehaviorTreeRoutine()
        {
            yield return new WaitUntil(_isInitBehaviorProgressWriter);
         
            foreach (IProgressWriterNode progressWriterNode in _progressWriterNodes)
            {
                progressWriterNode.LoadData(_data);
            }
        }

        private bool _isInitBehaviorProgressWriter()
        {
            return _progressWriterNodes.Count > 0;
        }
    }
}