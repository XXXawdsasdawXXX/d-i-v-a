using System;
using Code.Data.Configs;
using Code.Data.Value.RangeFloat;
using Code.Infrastructure.DI;
using UnityEngine;

namespace Code.Services
{
    public class MicrophoneAnalyzer : MonoBehaviour
    {
        private const int SAMPLE_WINDOW = 128;
        
        [Header("Stats")]
        [SerializeField] private string _device;
        [SerializeField] private float _micLoudness;
        [SerializeField] private float _micDecibels;

        private MicrophoneAnalyzerData _analyzerData;
        private AudioClip _clipRecord;
        private AudioClip _recordedClip;
    
        private bool _isInitialized;

        public event Action MaximumDecibelRecordedEvent;
        public event Action MinimumDecibelRecordedEvent;
     
    
        private void Start()
        {
            _analyzerData = Container.Instance.FindConfig<AudioConfig>().MicrophoneAnalyzerData;
            InitMic();
        }
    
        private void Update()
        {
            if (!_isInitialized)
            {
                return;
            }
        
            _micLoudness = MicrophoneLevelMax();
            _micDecibels = MicrophoneLevelMaxDecibels();

            if (_analyzerData.MinActionDecibels.Contains(_micDecibels))
            {
                MinimumDecibelRecordedEvent?.Invoke();
            }

            if (_analyzerData.MaxActionDecibels.Contains(_micDecibels))
            {
                MaximumDecibelRecordedEvent?.Invoke();
            }
        }
    
        private void OnDestroy()
        {
            StopMicrophone();
        }
    
        //mic initialization
        private void InitMic()
        {
            _device = Microphone.devices[0];
            _clipRecord = Microphone.Start(_device, true, 999, 44100);
            _isInitialized = true;
        }

        private void StopMicrophone()
        {
            Microphone.End(_device);
            _isInitialized = false;
        }
    
        private float MicrophoneLevelMax()
        {
            float levelMax = 0;
            var waveData = new float[SAMPLE_WINDOW];
            var micPosition = Microphone.GetPosition(null) - (SAMPLE_WINDOW + 1); // null means the first microphone

            if (micPosition < 0)
            {
                return 0;
            }
        
            _clipRecord.GetData(waveData, micPosition);
        
            for (int i = 0; i < SAMPLE_WINDOW; i++)
            {
                float wavePeak = waveData[i] * waveData[i];
                if (levelMax < wavePeak)
                {
                    levelMax = wavePeak;
                }
            }
            return levelMax;
        }

        private  float MicrophoneLevelMaxDecibels()
        {
            var db = 20 * Mathf.Log10(Mathf.Abs(_micLoudness));
            return db;
        }

   
    }
}