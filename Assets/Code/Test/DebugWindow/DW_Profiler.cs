using System;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace Code.Test
{
    [Serializable]
    public class DW_Profiler
    {
        private const float UPDATE_RATE = 4;
        
        [SerializeField] private Text _textFPS;
        [SerializeField] private Text _textAllocatedRum;
        [SerializeField] private Text _textReservedRam;
        [SerializeField] private Text _textMonoRam;
        
        private float _deltaTime;

        public void Refresh()
        {
            _deltaTime += Time.unscaledDeltaTime;

            if (_deltaTime > 1f / UPDATE_RATE)
            {
                float unscaledDeltaTime = Time.unscaledDeltaTime;

                _textFPS.text = Mathf.RoundToInt(1f / unscaledDeltaTime).ToString();

                float allocatedRam = Profiler.GetTotalAllocatedMemoryLong() / 1048576f;
                _textAllocatedRum.text = Mathf.RoundToInt(allocatedRam).ToString();

                float reservedRam = Profiler.GetTotalReservedMemoryLong() / 1048576f;
                _textReservedRam.text = Mathf.RoundToInt(reservedRam).ToString();

                float monoRam = Profiler.GetMonoUsedSizeLong() / 1048576f;
                _textMonoRam.text = Mathf.RoundToInt(monoRam).ToString();

                _deltaTime = 0;
            }
        }
    }
}