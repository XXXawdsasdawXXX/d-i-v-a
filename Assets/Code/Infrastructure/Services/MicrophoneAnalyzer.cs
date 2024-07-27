using System;
using Code.Data.Configs;
using Code.Data.Interfaces;
using Code.Data.StaticData;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.Services
{
    public class MicrophoneAnalyzer : IService, IGameInitListener, IGameTickListener, IGameExitListener
    {
        private const int SAMPLE_WINDOW = 128;

        [Header("Stats")] private string _device;
        private float _micLoudness;
        private float _micDecibels;

        private MicrophoneAnalyzerData _analyzerData;
        private AudioClip _clipRecord;
        private AudioClip _recordedClip;

        private bool _isInitialized;

        public event Action MaxDecibelRecordedEvent;
        public event Action MinDecibelRecordedEvent;

   
        public void GameInit()
        {
            if (Utils.Extensions.IsMacOs())
            {
                return;
            }

            _isInitialized = true;
            Debugging.Instance.Log($"MicrophoneAnalyzer: Construct", Debugging.Type.Micro);
            _analyzerData = Container.Instance.FindConfig<AudioConfig>().MicrophoneAnalyzerData;
            //todo проверка на ос
            InitMic();
            Debugging.Instance.Log($"MicrophoneAnalyzer: GameStart -> is init {_isInitialized}", Debugging.Type.Micro);
            
        }



        public void GameTick()
        {
            if (!_isInitialized)
            {
                return;
            }

            _micLoudness = MicrophoneLevelMax();
            _micDecibels = MicrophoneLevelMaxDecibels();

            if (_analyzerData.MinActionDecibels.Contains(_micDecibels))
            {
                MinDecibelRecordedEvent?.Invoke();
            }

            if (_analyzerData.MaxActionDecibels.Contains(_micDecibels))
            {
                MaxDecibelRecordedEvent?.Invoke();
            }
            //   Debugging.Instance.Log($"MicrophoneAnalyzer: GameTick {_micDecibels}", Debugging.Type.Micro);
        }

        public void GameExit()
        {
            StopMicrophone();
            Debugging.Instance.Log("MicrophoneAnalyzer: GameExit", Debugging.Type.Micro);
        }

        private void InitMic()
        {
            if (Microphone.devices == null || Microphone.devices.Length == 0 || string.IsNullOrEmpty(Microphone.devices[0]))
            {
                Debug.LogError("Microphone is not available or not connected.");
                return;
            }

            _device = Microphone.devices[0];
            _clipRecord = Microphone.Start(_device, true, 999, 44100);

            if (_clipRecord == null)
            {
                return;
            }
            
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

        private float MicrophoneLevelMaxDecibels()
        {
            var db = 20 * Mathf.Log10(Mathf.Abs(_micLoudness));
            return db;
        }
    }
}