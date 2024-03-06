using Code.Components.Objects;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components.Characters
{
    public class CharacterMaterialController : MonoBehaviour, IGameTickListener
    {
        [SerializeField] private MaterialController _materialController;
        [SerializeField] private LoopbackAudio _loopbackAudio;


        public void GameTick()
        {
            _materialController.SetShineAngle(_loopbackAudio.PostScaledMax);
        }
    }
}