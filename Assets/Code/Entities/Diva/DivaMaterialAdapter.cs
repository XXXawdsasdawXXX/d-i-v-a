using System.Collections;
using Code.Data;
using Code.Entities.Common;
using UnityEngine;

namespace Code.Entities.Diva
{
    public class DivaMaterialAdapter : MaterialAdapter, IWindowsSpecific
    {
        private Coroutine _animationCoroutine;
        
        #region Material methods

        [ContextMenu("Reset")]
        public void Reset()
        {
            Clear();
        }

        public void SetOutLineSpeedValue(float value)
        {
            //SetFloatValue(FloatValueType._TextureScrollXSpeed, _loopbackAudioService.PostScaledEnergy);
            //SetFloatValue(FloatValueType._TextureScrollYSpeed, _loopbackAudioService.PostScaledEnergy);
            
            SetFloatValue(FloatValueType._TextureScrollXSpeed, value);
            SetFloatValue(FloatValueType._TextureScrollYSpeed, value);
        }

        public  void SetShineAngleValue(float value)
        {
            //_material.SetFloat(FloatValueType._ShineRotate.ToString(), _loopbackAudioService.PostScaledMax);
            
            _material.SetFloat(FloatValueType._ShineRotate.ToString(), value);
        }

        [ContextMenu("EnableDynamicMagicMaterial")]
        public void EnableDynamicMagicMaterial()
        {
            Clear();
            SetState(StateType.FADE_ON, true);
            SetState(StateType.SHINE_ON, true);
        }

        [ContextMenu("EnableStaticLightMaterial")]
        public void EnableStaticLightMaterial()
        {
            Clear();
            SetState(StateType.FADE_ON, true);
            SetState(StateType.OUTBASE_ON, true);
        }

        [ContextMenu("PlayDoodle")]
        public void PlayDoodle()
        { 
            TryStopCoroutine();
            _animationCoroutine = StartCoroutine(Play(StateType.DOODLE_ON, duration: 2));
        }

        private IEnumerator Play(StateType stateType, float duration)
        {
            SetState(stateType, true);
            yield return new WaitForSeconds(duration);
            SetState(stateType, false);
        }

        private void TryStopCoroutine()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }
        }
        #endregion
    }
}