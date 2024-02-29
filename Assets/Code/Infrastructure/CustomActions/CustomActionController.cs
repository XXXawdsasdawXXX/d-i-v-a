using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using UnityEngine;

namespace Code.Infrastructure.CustomActions
{
    public class CustomActionController : MonoBehaviour,IGameInitListener
    {
        private CustomActionExecutor _executor;
        private TimeObserver _timeObserver;

        public void GameInit()
        {
            _executor = Container.Instance.FindService<CustomActionExecutor>();
            _timeObserver = Container.Instance.FindService<TimeObserver>();
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _timeObserver.StartDayEvent += OnDay;
                _timeObserver.StartNightEvent += OnNight;
                
            }
            else
            {
                
            }
        }

        private void OnNight()
        {
        }

        private void OnDay()
        {
            
        }

        private void TimeObserverOnInitTimeEvent()
        {
        }
    }
}