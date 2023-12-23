using System.Collections.Generic;
using Code.Components.Character.Params;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components.Character
{
    public class CharacterLiveStateController : MonoBehaviour, IGameTickListener
    {
        
        public List<CharacterLiveState> LiveStates { get; private set; } = new(0);

        public void GameTick()
        {
           
        }
    }

    public enum LiveStateKey
    {
        
    }
}