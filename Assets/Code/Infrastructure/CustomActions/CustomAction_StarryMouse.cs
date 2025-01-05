using Code.Components.Common;
using Code.Components.Entities.Characters;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Storages;
using Code.Data.VFX;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.CustomActions
{
    public class CustomAction_StarryMouse : CustomAction, IGameInitListener, IGameStartListener, IGameUpdateListener,
        IGameExitListener
    {
        [Header("Character")] private ColliderButton _characterButton;
        private CharacterAnimationAnalytic _characterAnimationAnalytic;
        [Header("Services")] private PositionService _positionService;
        private CoroutineRunner _coroutineRunner;
        [Header("Statis values")] private ParticleSystemFacade _particle;
        private float _duration;
        [Header("Dinamic values")] private bool _isActive;
        private Vector3 _lastPoint;


        public void GameInit()
        {
            //static values
            ParticlesStorage particles = Container.Instance.FindStorage<ParticlesStorage>();
            if (particles.TryGetParticle(ParticleType.StarryMouse, out ParticleSystemFacade[] particlesFacades))
            {
                _particle = particlesFacades[0];
                _duration = Container.Instance.FindConfig<TimeConfig>().Duration.StarryMouse;
                //character
                DIVA diva = Container.Instance.FindEntity<DIVA>();
                _characterButton = diva.FindCommonComponent<ColliderButton>();
                _characterAnimationAnalytic = diva.FindCharacterComponent<CharacterAnimationAnalytic>();
                //services 
                _positionService = Container.Instance.FindService<PositionService>();
                _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();

                SubscribeToEvents(true);
            }
        }

        public void GameStart()
        {
            _particle.Off();
        }

        public void GameExit()
        {
            SubscribeToEvents(false);
        }

        public void GameUpdate()
        {
            Vector3 currentMousePosition = _positionService.GetMouseWorldPosition();
            _particle.transform.position = currentMousePosition;
        }

        protected sealed override void TryStartAction()
        {
            _isActive = true;
            _particle.transform.position = _positionService.GetMouseWorldPosition();
            _particle.On();
            _coroutineRunner.StartActionWithDelay(StopAction, _duration);
            Debugging.Instance.Log($"[{GetActionType()}] [Start Action]", Debugging.Type.CustomAction);
        }

        protected override void StopAction()
        {
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
            if (flag)
            {
                _characterButton.OnPressedUp += OnPressedUp;
            }
            else
            {
                _characterButton.OnPressedUp -= OnPressedUp;
            }
        }

        private void OnPressedUp(Vector2 _, float pressDuration)
        {
            Debugging.Instance.Log($"[{GetActionType()}] [CharacterButtonOnDownEvent] is active {_isActive}",
                Debugging.Type.CustomAction);
            if (pressDuration < 0.1 && _characterAnimationAnalytic.GetAnimationMode() is CharacterAnimationMode.Stand)
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