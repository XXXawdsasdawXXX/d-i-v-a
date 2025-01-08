using Code.Data;
using Code.Entities.Common;
using Code.Entities.Diva;
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
        [Header("Character")] 
        private ColliderButton _characterButton;
        private DivaAnimationAnalytic _divaAnimationAnalytic;
        
        [Header("Services")] 
        private PositionService _positionService;
        private CoroutineRunner _coroutineRunner;
        
        [Header("Static values")] 
        private ParticleSystemFacade _particle;
        private float _duration;
        
        [Header("Dynamic values")] 
        private bool _isActive;
        private Vector3 _lastPoint;


        public void GameInit()
        {
            //static values
            ParticlesStorage particles = Container.Instance.FindStorage<ParticlesStorage>();
          
            if (particles.TryGetParticle(EParticleType.StarryMouse, out ParticleSystemFacade[] particlesFacades))
            {
                _particle = particlesFacades[0];
                _duration = Container.Instance.FindConfig<TimeConfig>().Duration.StarryMouse;
                //character
                DivaEntity diva = Container.Instance.FindEntity<DivaEntity>();
                _characterButton = diva.FindCommonComponent<ColliderButton>();
                _divaAnimationAnalytic = diva.FindCharacterComponent<DivaAnimationAnalytic>();
                //services 
                _positionService = Container.Instance.FindService<PositionService>();
                _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();

                _subscribeToEvents(true);
            }
        }

        public void GameStart()
        {
            _particle.Off();
        }

        public void GameExit()
        {
            _subscribeToEvents(false);
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
#if DEBUGGING
            Debugging.Log(this, $"[start Action] {GetActionType()}.", Debugging.Type.CustomAction);
#endif
        }

        protected override void StopAction()
        {
            _isActive = false;
            
            _particle.Off();
#if DEBUGGING
            Debugging.Log(this, $"[stop Action] {GetActionType()}.", Debugging.Type.CustomAction);
#endif
        }

        public override ECustomCutsceneActionType GetActionType()
        {
            return ECustomCutsceneActionType.StarryMouse;
        }

        private void _subscribeToEvents(bool flag)
        {
            if (flag)
            {
                _characterButton.OnPressedUp += _onPressedUp;
            }
            else
            {
                _characterButton.OnPressedUp -= _onPressedUp;
            }
        }

        private void _onPressedUp(Vector2 _, float pressDuration)
        {
#if DEBUGGING
            Debugging.Log(this, $"[_onPressedUp] {GetActionType()}. Is active {_isActive}.", Debugging.Type.CustomAction);
#endif
           
            if (pressDuration < 0.1 && _divaAnimationAnalytic.GetAnimationMode() is EDivaAnimationMode.Stand)
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