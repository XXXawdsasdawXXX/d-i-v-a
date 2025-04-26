using Code.Utils;

namespace Code.BehaviorTree.Diva
{
    public partial class BehaviourNode_Sleep
    {
        protected override void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _characterButton.SeriesOfClicksEvent += _onClickSeries;
                _timeObserver.OnDayStarted += _rouse;
                _sleepState.OnChanged += _onChangedSleepStateValue;
            }
            else
            {
                _characterButton.SeriesOfClicksEvent -= _onClickSeries;
                _timeObserver.OnDayStarted -= _rouse;
                _sleepState.OnChanged -= _onChangedSleepStateValue;
            }
        }

        private void _onClickSeries(int clickCount)
        {
            if (clickCount >= 5)
            {
                _rouse();
            }
        }

        private void _onChangedSleepStateValue(float sleepValue)
        {
            if (_sleepState.GetPercent() > 0.9f)
            {
#if DEBUGGING
                Log.Info(this, $"[_onChangedSleepStateValue] Sleep is max.", Log.Type.BehaviorTree);
#endif

                _stopSleep();
            }
        }
    }
}