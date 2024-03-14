using System.Collections.Generic;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Data.StaticData;
using Code.Infrastructure.Save;
using Code.Utils;

namespace Code.Services
{
    public class InteractionStorage :Storage, IProgressWriter
    {
        private Dictionary<InteractionType, int> _interactions;
        
        public void Add(InteractionDynamicData interactionData)
        {
           Add(interactionData.СlassificationType,interactionData.Value);
        }
        
        public void Add(InteractionType type, int value = 1)
        {
            if (_interactions.ContainsKey(type))
            {
                _interactions[type] += value;
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
        }
        public void UpdateProgress(PlayerProgressData playerProgress)
        {
            playerProgress.Interactions = _interactions;
        }
    }
}