using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Character
{
    public class CharacterAudioListener : MonoBehaviour , IGameTickListener, IGameStartListener, IGameExitListener
    {
        [Header("Components")] 
        private MicrophoneAnalyzer _microphoneAnalyzer;

        [SerializeField] private CharacterAnimator characterAnimator;

        [Header("Params")] 
        [SerializeField] private float _reactionCooldown;

        private float _currentCooldown;

        public void GameStart()
        {
            _microphoneAnalyzer = Container.Instance.FindService<MicrophoneAnalyzer>();
            SubscribeToEvents();
            Debugging.Instance.Log("CharacterAudioListener: Game Start", Debugging.Type.Micro);
        }


        public void GameTick()
        {
            if (_currentCooldown > 0)
            {
                _currentCooldown -= Time.deltaTime;
            }
            Debugging.Instance.Log($"CharacterAudioListener: Game Tick {_currentCooldown}", Debugging.Type.Micro);
        }

        public void GameExit()
        {
            Debugging.Instance.Log("CharacterAudioListener: Game Exit", Debugging.Type.Micro);
            UnSubscribeToEvents();
            
        }



        private void SubscribeToEvents()
        {
            _microphoneAnalyzer.MaximumDecibelRecordedEvent += OnMaximumDecibelRecordedEvent;
            _microphoneAnalyzer.MinimumDecibelRecordedEvent += OnMinimumDecibelRecordedEvent;
        }

        private void UnSubscribeToEvents()
        {
            _microphoneAnalyzer.MaximumDecibelRecordedEvent -= OnMaximumDecibelRecordedEvent;
            _microphoneAnalyzer.MinimumDecibelRecordedEvent -= OnMinimumDecibelRecordedEvent;
        }

        private void OnMinimumDecibelRecordedEvent()
        {
            if (_currentCooldown > 0)
            {
                return;
            }

            _currentCooldown = _reactionCooldown;
            characterAnimator.PlayReactionVoice();
        }

        private void OnMaximumDecibelRecordedEvent()
        {
        }
    }
}