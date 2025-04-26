using System;
using System.Collections.Generic;
using System.Linq;
using Code.Infrastructure.Save;
using Code.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;

namespace Code.Data
{
    [Preserve]
    public class InteractionStorage : IStorage, IProgressWriter
    {
        public event Action<EInteractionType> OnSwitchDominationType;
        public event Action<EInteractionType, int> OnAdded;
        
        private Dictionary<EInteractionType, int> _interactions;
        private EInteractionType _currentDominationType;

        public UniTask LoadProgress(PlayerProgressData playerProgress)
        {
            _interactions = playerProgress.Interactions;
            
            _currentDominationType = GetDominantInteractionType();
            
            OnSwitchDominationType?.Invoke(_currentDominationType);
            
            return UniTask.CompletedTask;
        }

        public void SaveProgress(PlayerProgressData playerProgress)
        {
            playerProgress.Interactions = _interactions;
        }

        public int GetSum()
        {
            return _interactions.Sum(interaction => interaction.Value);
        }

        public void Add(EInteractionType type, int value = 1)
        {
            if (_interactions.ContainsKey(type))
            {
                _interactions[type] += value;
                if (_currentDominationType != GetDominantInteractionType())
                {
                    _currentDominationType = GetDominantInteractionType();
                    OnSwitchDominationType?.Invoke(_currentDominationType);
                }
            }
            else
            {
                _interactions.Add(type, value);
            }

            OnAdded?.Invoke(type, value);
            
            Log.Info(this, $"[Add] {type} {_interactions[type]}", Log.Type.Interaction);
        }

        public EInteractionType GetDominantInteractionType()
        {
            if (_interactions.Count == 0)
            {
                return EInteractionType.None;
            }

            KeyValuePair<EInteractionType, int> maxPair = _interactions.OrderByDescending(pair => pair.Value).FirstOrDefault();
       
            return maxPair.Key;
        }
    }
}