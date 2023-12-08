using System;
using Code.Services;
using UnityEngine;

namespace Code.Character
{
    public class CharacterListener : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private MicrophoneAnalyzer _microphoneAnalyzer;
        [SerializeField] private CharacterAnimation _characterAnimation;

        [Header("Params")]
        [SerializeField] private float _reactionCooldown;

        private float _currentCooldown;

        private void Start()
        {
            SubscribeToEvents();
        }

        private void Update()
        {
            if (_currentCooldown > 0)
            {
                _currentCooldown -= Time.deltaTime;
            }
        }

        private void OnDestroy()
        {
            UnSubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _microphoneAnalyzer.MaximumDecibelRecordedEvent += OnMaximumDecibelRecordedEvent;
            _microphoneAnalyzer.MinimumDecibelRecordedEvent += OnMinimumDecibelRecordedEvent;
        }

        private void UnSubscribeToEvents()
        {
            _microphoneAnalyzer.MaximumDecibelRecordedEvent += OnMaximumDecibelRecordedEvent;
            _microphoneAnalyzer.MinimumDecibelRecordedEvent += OnMinimumDecibelRecordedEvent;
        }
        
        private void OnMinimumDecibelRecordedEvent()
        {
            if (_currentCooldown > 0)
            {
                return;
            }

            _currentCooldown = _reactionCooldown;
            _characterAnimation.PlayReactionVoice();
        }

        private void OnMaximumDecibelRecordedEvent()
        {
            
        }
    }
}