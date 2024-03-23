using System;
using System.Collections.Generic;
using System.Linq;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Data.StaticData;
using Code.Infrastructure.Save;
using Code.Utils;

namespace Code.Services
{
    public class InteractionStorage : IStorage, IProgressWriter
    {
        private Dictionary<InteractionType, int> _interactions;
        private InteractionType _currentDominationType;

        public event Action<InteractionType> SwitchDominationTypeEvent;
        
        public int GetSum()
        {
            return _interactions.Sum(interaction => interaction.Value);
        }
        public void Add(InteractionDynamicData interactionData)
        {
           Add(interactionData.СlassificationType,interactionData.Value);
        }
        
        public void Add(InteractionType type, int value = 1)
        {
            if (_interactions.ContainsKey(type))
            {
                _interactions[type] += value;
                if (_currentDominationType != GetDominantInteractionType())
                {
                    _currentDominationType = GetDominantInteractionType();
                    SwitchDominationTypeEvent?.Invoke(_currentDominationType);
                }
            }
            else
            {
                _interactions.Add(type,value);
            }
            
            Debugging.Instance.Log($"[storage] add {type} {_interactions[type]}",Debugging.Type.Interaction);
        }
        
        public void LoadProgress(PlayerProgressData playerProgress)
        {
            _interactions = playerProgress.Interactions;
            _currentDominationType = GetDominantInteractionType();
        }
        public void UpdateProgress(PlayerProgressData playerProgress)
        {
            playerProgress.Interactions = _interactions;
        }

        public InteractionType GetDominantInteractionType()
        {
            if (_interactions.Count == 0)
            {
                return InteractionType.None;
            }

            var maxPair = _interactions.OrderByDescending(pair => pair.Value).FirstOrDefault();
            return maxPair.Key;
        }
    }
}