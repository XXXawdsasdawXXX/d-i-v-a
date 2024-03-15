using Code.Components.Characters;
using Code.Data.Enums;
using Code.Data.Facades;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Services.LoopbackAudio.Audio;
using Code.Utils;

namespace Code.Infrastructure.CustomActions
{
    public class CustomAction_Electricity: CustomAction, IGameTickListener, IGameStartListener
    {
        private readonly bool _isNotUsed;
        private readonly DIVA _diva;
        private readonly ParticleSystemFacade[] _particlesSystems;
        private readonly LoopbackAudioService _loopbackAudioService;
        private readonly CharacterModeAdapter _characterModeAdapter;

        public CustomAction_Electricity()
        {
            var particleDictionary = Container.Instance.FindService<ParticlesStorage>();
            if (particleDictionary.TryGetParticle(ParticleType.Electricity, out _particlesSystems))
            {
                _diva = Container.Instance.FindEntity<DIVA>();
                _characterModeAdapter = _diva.FindCharacterComponent<CharacterModeAdapter>();
                _loopbackAudioService = Container.Instance.FindService<LoopbackAudioService>();
                return;
            }
            
            _isNotUsed = true;
        }
        
        
        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.Electricity;
        }
      
        public void GameStart()
        {
            if(_isNotUsed)return;
            StartAction();
        }



        public void GameTick()
        {
            if(_isNotUsed)return;
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


        protected  override void StartAction()
        {
            Debugging.Instance.Log($"Старт события {GetActionType()} particles count = {_particlesSystems.Length}",Debugging.Type.CustomAction);
            foreach (var particle in _particlesSystems)
            {
                particle.On();
                //particle.SetColor(Color.black);
            }
        }
        protected  override void StopAction()
        {
          
        }
        
        
      
    }
}