using Code.Components.Characters;
using Code.Components.Objects;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Facades;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.CustomActions
{
    public class CustomAction_StarryMouse : CustomAction, IGameTickListener, IGameExitListener , IGameStartListener
    {
        [Header("Character")] 
        private readonly ColliderButton _characterButton;
        private readonly CharacterAnimationAnalytic _characterAnimationAnalytic;
        [Header("Services")] 
        private readonly PositionService _positionService;
        private readonly CoroutineRunner _coroutineRunner;
        [Header("Statis values")] 
        private readonly ParticleSystemFacade _particle;
        private float _duration;
        [Header("Dinamic values")] 
        private bool _isActive;
        private Vector3 _lastPoint;

        public CustomAction_StarryMouse()
        {
            //character
            var diva = Container.Instance.FindEntity<DIVA>();
            _characterButton = diva.FindCommonComponent<ColliderButton>();
            _characterAnimationAnalytic = diva.FindCharacterComponent<CharacterAnimationAnalytic>();
            //services 
            _positionService = Container.Instance.FindService<PositionService>();
            _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();
            //static values
            _duration = Container.Instance.FindConfig<TimeConfig>().Duration.StarryMouse;
            var particles = Container.Instance.FindService<ParticlesDictionary>();
            if (!particles.TryGetParticle(ParticleType.StarryMouse, out var particlesFacades))
            {
                Debugging.Instance.ErrorLog($"Партикл по типу {ParticleType.SkyStars} не добавлен в библиотеку партиклов");
            }
            _particle = particlesFacades[0];
            
            SubscribeToEvents(true);
        }

        public void GameStart()
        {
   
            _particle.Off();
            
        }

        public void GameExit()
        {
            SubscribeToEvents(false);
        }

        public void GameTick()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                StartAction();
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                StopAction();
            }

          
                var currentMousePosition = _positionService.GetMouseWorldPosition();
                _particle.transform.position = currentMousePosition;
            
    
            
        }

        public sealed override void StartAction()
        {
            _isActive = true;
            _particle.transform.position = _positionService.GetMouseWorldPosition();
            _particle.On();
            _coroutineRunner.StartActionWithDelay(StopAction, _duration);
            Debugging.Instance.Log($"[{GetActionType()}] [Start Action]",Debugging.Type.CustomAction);
        }

        public override void StopAction()
        {
            _isActive = false;
            _particle.Off();
            Debugging.Instance.Log($"[{GetActionType()}] [Stop Action]",Debugging.Type.CustomAction);
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.StarryMouse;
        }

        private void SubscribeToEvents(bool flag)
        {
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
            Debugging.Instance.Log($"[{GetActionType()}] [CharacterButtonOnDownEvent] is active {_isActive}",Debugging.Type.CustomAction);
            if (pressDuration < 0.1 && _characterAnimationAnalytic.GetAnimationMode() is not CharacterAnimationMode.Seat)
            {
                if (_isActive)
                {
                    return;
                }
                StartAction();
            }
        }
    }
}