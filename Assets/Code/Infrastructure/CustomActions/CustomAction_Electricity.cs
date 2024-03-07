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
    public class CustomAction_Electricity: CustomAction, IGameTickListener, IGameStartListener
    {
        private readonly DIVA _diva;
        private ParticleSystemFacade[] _particlesSystems;
        private readonly LoopbackAudio _loopbackAudio;
        private readonly CharacterModeAdapter _characterModeAdapter;

        public CustomAction_Electricity()
        {
            var particleDictionary = Container.Instance.FindService<ParticlesDictionary>();
            if (!particleDictionary.TryGetParticle(ParticleType.Stun, out _particlesSystems))
            {
                Debugging.Instance.ErrorLog($"Партикл по типу {ParticleType.Stun} не добавлен в библиотеку партиклов");
            }

            _diva = Container.Instance.FindEntity<DIVA>();
            _characterModeAdapter = _diva.FindCharacterComponent<CharacterModeAdapter>();
            _loopbackAudio = Container.Instance.FindService<LoopbackAudio>();
        }
        
        
        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.Electricity;
        }
      
        public void GameStart()
        {
            StartAction();
        }



        public void GameTick()
        {
            
            foreach (var particle in _particlesSystems)
            {
                if (!particle.IsPlay)
                {
                    particle.On();
                }
                 particle.SetTrailWidthOverTrail(_loopbackAudio.PostScaledMax * 0.07f);
               // particle.SetTrailLifeTime(_loopbackAudio.PostScaledMax * 5);
                //particle.SetVelocityOverLifetime(_loopbackAudio.PostScaledEnergy * 0.7f);
            particle.transform.position = _characterModeAdapter.GetWorldEatPoint() + Vector3.up * 0.7f;
            
            }
        }


        public override void StartAction()
        {
            Debugging.Instance.Log($"Старт события {GetActionType()} particles count = {_particlesSystems.Length}",Debugging.Type.CustomAction);
            foreach (var particle in _particlesSystems)
            {
                particle.On();
                //particle.SetColor(Color.black);
            }
        }
        public override void StopAction()
        {
          
        }
        
        
      
    }
}