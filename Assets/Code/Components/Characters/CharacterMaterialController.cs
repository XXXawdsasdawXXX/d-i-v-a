using Code.Components.Common;
using Code.Data.Interfaces;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services.LoopbackAudio.Audio;
using UnityEngine;

namespace Code.Components.Characters
{
    public class CharacterMaterialController : MaterialController, IWindowsSpecific,IGameInitListener ,IGameTickListener
    {
        private LoopbackAudioService _loopbackAudioService;

        public void GameInit()
        {
            _loopbackAudioService = Container.Instance.FindService<LoopbackAudioService>();
        }

        public void GameTick()
        {
            SetShineAngle();
            SetOutLineSpeed();
        }

        private void SetOutLineSpeed()
        {
            SetFloatValue(FloatValueType._TextureScrollXSpeed, _loopbackAudioService.PostScaledEnergy);
            SetFloatValue(FloatValueType._TextureScrollYSpeed, _loopbackAudioService.PostScaledEnergy);
        }

        private void SetShineAngle()
        {
            _material.SetFloat(FloatValueType._ShineRotate.ToString(), _loopbackAudioService.PostScaledMax);
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

        [ContextMenu("Reset")]
        public void Reset()
        {
           Clear();
        }
    }
}