using Code.Data.Configs;
using Code.Data.Enums;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.CustomActions
{
    public class CustomAction_StarryMouse : CustomAction, IGameTickListener
    {
        [Header("Services")]
        private readonly PositionService _positionService;
        [Header("Statis calues")]
        private readonly ParticleSystem _particle;
        private float _duration;
        [Header("Dinamic values")]
        private bool _isActive;

        private Vector3 _lastPoint;

        public CustomAction_StarryMouse()
        {
            //services 
            _positionService = Container.Instance.FindService<PositionService>();
            //static values
            _duration = Container.Instance.FindConfig<TimeConfig>().Duration.StarryMouse;
            var particles = Container.Instance.FindService<ParticlesDictionary>();
             if (!particles.TryGetParticle(ParticleType.StarryMouse,out _particle))
             {
                 Debugging.Instance.ErrorLog($"Партикл по типу {ParticleType.SkyStars} не добавлен в библиотеку партиклов");
             }
             
             StartAction();
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
            
            if (_isActive)
            {
                var currentMousePosition =  _positionService.GetMouseWorldPosition();
                if (Vector3.Distance(currentMousePosition, _lastPoint) > 0.3f)
                {
                    _lastPoint = currentMousePosition;
                    _particle.transform.position = _lastPoint;
              
                    var em = _particle.emission;
                    em.enabled = true;
                }
                else
                {
                    var em = _particle.emission;
                    em.enabled = false;
                }
            }
        }

        public override void StartAction()
        {
            _isActive = true;
            _particle.Play();
            var em = _particle.emission;
            em.enabled = true;
        }

        public override void StopAction()
        {
            _isActive = false;
            _particle.Stop();
            var em = _particle.emission;
            em.enabled = false;
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.StarryMouse;
        }
    }
}