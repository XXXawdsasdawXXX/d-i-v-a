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
        private bool _isDisable ;
        private readonly DIVA _diva;
        private ParticleSystemFacade[] _particlesSystems;
        private readonly LoopbackAudioService _loopbackAudioService;
        private readonly CharacterModeAdapter _characterModeAdapter;

        public CustomAction_Electricity()
        {
            var particleDictionary = Container.Instance.FindService<ParticlesDictionary>();
            if (!particleDictionary.TryGetParticle(ParticleType.Electricity, out _particlesSystems))
            {
                _isDisable = true;
                Debugging.Instance.ErrorLog($"Партикл по типу {ParticleType.Electricity} не добавлен в библиотеку партиклов");
            }

            _diva = Container.Instance.FindEntity<DIVA>();
            _characterModeAdapter = _diva.FindCharacterComponent<CharacterModeAdapter>();
            _loopbackAudioService = Container.Instance.FindService<LoopbackAudioService>();
        }
        
        
        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.Electricity;
        }
      
        public void GameStart()
        {
            if(_isDisable)return;
            StartAction();
        }



        public void GameTick()
        {
            if(_isDisable)return;
            foreach (var particle in _particlesSystems)
            {
                if (!particle.IsPlay)
                {
                    particle.On();
                }

                var value = _loopbackAudioService.PostScaledEnergy * 0.02f;
                particle.SetTrailWidthOverTrail(value/* < 0.01f ? 0.01f : value*/);
           
                particle.transform.position = _characterModeAdapter.GetWorldEatPoint();
            
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