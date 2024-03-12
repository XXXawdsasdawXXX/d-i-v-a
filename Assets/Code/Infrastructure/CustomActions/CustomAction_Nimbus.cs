using Code.Components.Characters;
using Code.Data.Enums;
using Code.Data.Facades;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.CustomActions
{
    public class CustomAction_Nimbus : CustomAction, IGameTickListener, IGameStartListener
    {
        private readonly bool _isDisable;
        private readonly DIVA _diva;
        private readonly ParticleSystemFacade[] _particlesSystems;
        private readonly LoopbackAudioService _loopbackAudioService;
        private readonly CharacterModeAdapter _characterModeAdapter;

        public CustomAction_Nimbus()
        {
            var particleDictionary = Container.Instance.FindService<ParticlesDictionary>();
            if (!particleDictionary.TryGetParticle(ParticleType.Nimbus, out _particlesSystems))
            {
                _isDisable = true;
                return;
            }

            _diva = Container.Instance.FindEntity<DIVA>();
            _characterModeAdapter = _diva.FindCharacterComponent<CharacterModeAdapter>();
            _loopbackAudioService = Container.Instance.FindService<LoopbackAudioService>();
        }


        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.Nimbus;
        }

        public void GameStart()
        {
            if (_isDisable) return;
            StartAction();
        }


        public void GameTick()
        {
            if (_isDisable) return;
            foreach (var particle in _particlesSystems)
            {
                if (!particle.IsPlay)
                {
                    particle.On();
                }

                particle.SetTrailWidthOverTrail(_loopbackAudioService.PostScaledMax * 0.07f);
                particle.transform.position = _characterModeAdapter.GetWorldEatPoint() + Vector3.up * 0.5f;
            }
        }


        public override void StartAction()
        {
            if (_isDisable) return;
            Debugging.Instance.Log($"Старт события {GetActionType()} particles count = {_particlesSystems.Length}",
                Debugging.Type.CustomAction);
            foreach (var particle in _particlesSystems)
            {
                particle.On();
            }
        }

        public override void StopAction()
        {
        }
    }
}