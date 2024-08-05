using Code.Components.Characters;
using Code.Components.Common;
using Code.Components.Entities;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Facades;
using Code.Data.Storages;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.CustomActions
{
    public class CustomAction_StarryMouse : CustomAction,IGameInitListener ,IGameStartListener, IGameTickListener, IGameExitListener
    {
        private  bool _isNotUsed;
        [Header("Character")] 
        private  ColliderButton _characterButton;
        private  CharacterAnimationAnalytic _characterAnimationAnalytic;
        [Header("Services")] 
        private  PositionService _positionService;
        private  CoroutineRunner _coroutineRunner;
        [Header("Statis values")] 
        private  ParticleSystemFacade _particle;
        private float _duration;
        [Header("Dinamic values")] 
        private bool _isActive;
        private Vector3 _lastPoint;
        

        public void GameInit()
        {
            //static values
            var particles = Container.Instance.FindStorage<ParticlesStorage>();
            if (particles.TryGetParticle(ParticleType.StarryMouse, out var particlesFacades))
            {
                _particle = particlesFacades[0];
                _duration = Container.Instance.FindConfig<TimeConfig>().Duration.StarryMouse;
                //character
                var diva = Container.Instance.FindEntity<DIVA>();
                _characterButton = diva.FindCommonComponent<ColliderButton>();
                _characterAnimationAnalytic = diva.FindCharacterComponent<CharacterAnimationAnalytic>();
                //services 
                _positionService = Container.Instance.FindService<PositionService>();
                _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();
                
                SubscribeToEvents(true);
                return;
            }
            
            _isNotUsed = true;
        }

        public void GameStart()
        {
            if(_isNotUsed)return;
            _particle.Off();
        }

        public void GameExit()
        {
            if(_isNotUsed)return;
            SubscribeToEvents(false);
        }

        public void GameTick()
        {
            if(_isNotUsed)return;
            var currentMousePosition = _positionService.GetMouseWorldPosition();
            _particle.transform.position = currentMousePosition;
        }

        protected  sealed override void TryStartAction()
        {
            if(_isNotUsed)return;
            _isActive = true;
            _particle.transform.position = _positionService.GetMouseWorldPosition();
            _particle.On();
            _coroutineRunner.StartActionWithDelay(StopAction, _duration);
            Debugging.Instance.Log($"[{GetActionType()}] [Start Action]", Debugging.Type.CustomAction);
        }

        protected  override void StopAction()
        {
            if(_isNotUsed)return;
            _isActive = false;
            _particle.Off();
            Debugging.Instance.Log($"[{GetActionType()}] [Stop Action]", Debugging.Type.CustomAction);
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.StarryMouse;
        }

        private void SubscribeToEvents(bool flag)
        {
            if(_isNotUsed)return;
            if (flag)
            {
                _characterButton.UpEvent += OnButtonUp;
            }
            else
            {
                _characterButton.UpEvent -= OnButtonUp;
            }
        }

        private void OnButtonUp(Vector2 _, float pressDuration)
        {
            if(_isNotUsed)return;
            Debugging.Instance.Log($"[{GetActionType()}] [CharacterButtonOnDownEvent] is active {_isActive}",
                Debugging.Type.CustomAction);
            if (pressDuration < 0.1 &&
                _characterAnimationAnalytic.GetAnimationMode() is not CharacterAnimationMode.Seat)
            {
                if (_isActive)
                {
                    return;
                }

                TryStartAction();
            }
        }
    }
}