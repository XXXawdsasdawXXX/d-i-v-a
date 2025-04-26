using Code.Data;
using Code.Entities.Common;
using Code.Entities.Diva;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Code.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace Code.Infrastructure.CustomActions
{
    [Preserve]
    public class CustomAction_StarryMouse : CustomAction, ISubscriber, IInitListener, IStartListener, IUpdateListener 
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


        public UniTask GameInitialize()
        {
            //static values
            ParticlesStorage particles = Container.Instance.FindStorage<ParticlesStorage>();
          
            if (particles.TryGetParticle(EParticleType.StarryMouse, out ParticleSystemFacade[] particlesFacades))
            {
                _particle = particlesFacades[0];
                _duration = Container.Instance.FindConfig<TimeConfig>().Duration.StarryMouse;
               
                //diva
                DivaEntity diva = Container.Instance.FindEntity<DivaEntity>();
                _characterButton = diva.FindCommonComponent<ColliderButton>();
                _divaAnimationAnalytic = diva.FindCharacterComponent<DivaAnimationAnalytic>();
                
                //services 
                _positionService = Container.Instance.GetService<PositionService>();
                _coroutineRunner = Container.Instance.GetService<CoroutineRunner>();
            }
            
            return UniTask.CompletedTask;
        }

        public void Subscribe()
        {
            _characterButton.OnPressedUp += _onPressedUp;
        }

        public UniTask GameStart()
        {
            _particle.Off();
            
            return UniTask.CompletedTask;
        }

        public void GameUpdate()
        {
            Vector3 currentMousePosition = _positionService.GetMouseWorldPosition();
            
            _particle.transform.position = currentMousePosition;
        }

        public void Unsubscribe()
        {
            _characterButton.OnPressedUp -= _onPressedUp;
        }

        public override ECustomCutsceneActionType GetActionType()
        {
            return ECustomCutsceneActionType.StarryMouse;
        }

        protected sealed override void TryStartAction()
        {
            _isActive = true;
            
            _particle.transform.position = _positionService.GetMouseWorldPosition();
            
            _particle.On();
            
            _coroutineRunner.StartActionWithDelay(StopAction, _duration);

            Log.Info(this, $"[start Action] {GetActionType()}.", Log.Type.CustomAction);
        }

        protected override void StopAction()
        {
            _isActive = false;
            
            _particle.Off();

            Log.Info(this, $"[stop Action] {GetActionType()}.", Log.Type.CustomAction);
        }

        private void _onPressedUp(Vector2 _, float pressDuration)
        {
            Log.Info(this, $"[_onPressedUp] {GetActionType()}. Is active {_isActive}.", Log.Type.CustomAction);

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