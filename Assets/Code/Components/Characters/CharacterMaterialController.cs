using System.Collections.Generic;
using Code.Components.Objects;
using Code.Infrastructure.GameLoop;
using Code.Services.LoopbackAudio.Audio;
using UnityEngine;

namespace Code.Components.Characters
{
    public class CharacterMaterialController : MaterialController, IGameTickListener
    {
        [SerializeField] private LoopbackAudioService loopbackAudioService;

        private List<FloatValueType> _floatObservers = new List<FloatValueType>();
        
        
        public void GameTick()
        {
            if (loopbackAudioService == null)
            {
                return;
            }
            SetShineAngle();
            SetOutlLineSpeed();
        }

        private void SetOutlLineSpeed()
        {
            SetFloatValue(FloatValueType._TextureScrollXSpeed, loopbackAudioService.PostScaledEnergy);
            SetFloatValue(FloatValueType._TextureScrollYSpeed, loopbackAudioService.PostScaledEnergy);
        }

        private void SetShineAngle()
        {
            _material.SetFloat(FloatValueType._ShineRotate.ToString(), loopbackAudioService.PostScaledMax);
        }

        [ContextMenu("SetDynamicMagicMaterial")]
        public void SetDynamicMagicMaterial()
        {
           Clear();
           SetState(StateType.FADE_ON,true);
            SetState(StateType.SHINE_ON,true);
        }
        
        [ContextMenu("SetStaticLightMaterial")]
        public void SetStaticLightMaterial()
        {
            Clear();
            SetState(StateType.FADE_ON,true);
            SetState(StateType.OUTBASE_ON,true);
        }

        public void EnableOutlLine()
        {
            
        }
        
        [ContextMenu("Reset")]
        public void Reset()
        {
           Clear();
        }
    }
}