using Code.Game.Effects;
using Code.Game.Entities.Common;
using Code.Game.Entities.Diva;
using Code.Game.Services;
using Code.Game.Services.Position;
using Code.Game.Services.Time;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.ServiceLocator;
using Code.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace Code.Game.CustomActions
{
    [Preserve]
    public class CustomAction_StarryMouse : CustomAction, ISubscriber, IInitializeListener, IUpdateListener 
    {
        [Header("Character")] 
        private ColliderButton _characterButton;
        private DivaAnimationAnalytic _divaAnimationAnalytic;
        
        [Header("Services")] 
        private PositionService _positionService;
        private CoroutineRunner _coroutineRunner;
        private ParticlesStorage _particlesStorage;
        
        [Header("Static values")] 
        private ParticleSystemFacade _particle;
        private float _duration;
        
        [Header("Dynamic values")] 
        private bool _isActive;
        private Vector3 _lastPoint;


        public UniTask GameInitialize()
        {
            _duration = Container.Instance.GetConfig<TimeConfig>().Duration.StarryMouse;
           
            //diva
            DivaEntity diva = Container.Instance.FindEntity<DivaEntity>();
            _characterButton = diva.FindCommonComponent<ColliderButton>();
            _divaAnimationAnalytic = diva.FindCharacterComponent<DivaAnimationAnalytic>();
            
            //services 
            _positionService = Container.Instance.GetService<PositionService>();
            _coroutineRunner = Container.Instance.GetService<CoroutineRunner>();
           
            _particlesStorage = Container.Instance.FindStorage<ParticlesStorage>();
            
            return UniTask.CompletedTask;
        }

        public void Subscribe()
        {
            _characterButton.OnPressedUp += _onPressedUp;
        }
        
        public void GameUpdate()
        {
            if (_isActive)
            {
                Vector3 currentMousePosition = _positionService.GetMouseWorldPosition();
                
                _particle.transform.position = currentMousePosition;
            }
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
            if (_particle == null)
            {
                if (_particlesStorage.TryGetParticle(EParticleType.StarryMouse, out ParticleSystemFacade[] particlesFacades))
                {
                    _particle = particlesFacades[0];
                }
            }

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