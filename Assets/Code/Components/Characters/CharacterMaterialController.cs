using System.Collections.Generic;
using Code.Components.Objects;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components.Characters
{
    public class CharacterMaterialController : MaterialController, IGameTickListener
    {
        [SerializeField] private LoopbackAudio _loopbackAudio;

        private List<FloatValueType> _floatObservers = new List<FloatValueType>();

        
        
        public void GameTick()
        {
            SetShineAngle();
            SetOutlLineSpeed();
        }

        private void SetOutlLineSpeed()
        {
            SetFloatValue(FloatValueType._TextureScrollXSpeed, _loopbackAudio.PostScaledEnergy);
            SetFloatValue(FloatValueType._TextureScrollYSpeed, _loopbackAudio.PostScaledEnergy);
        }

        private void SetShineAngle()
        {
            _material.SetFloat(FloatValueType._ShineRotate.ToString(), _loopbackAudio.PostScaledMax);
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