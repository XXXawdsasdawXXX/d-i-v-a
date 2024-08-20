using Code.Infrastructure.BehaviorTree.BaseNodes;
using Code.Utils;

namespace Code.Infrastructure.BehaviorTree.Character.Behavior
{
    public partial class BehaviourNode_Sleep 
    {
        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _characterButton.SeriesOfClicksEvent += OnClickSeries;
                _timeObserver.StartDayEvent += Rouse;
                _sleepState.ChangedEvent += OnChangedSleepStateValue;
                _microphoneAnalyzer.MaxDecibelRecordedEvent += OnMaxDecibelRecorder;
            }
            else
            {
                _characterButton.SeriesOfClicksEvent -= OnClickSeries;
                _timeObserver.StartDayEvent -= Rouse;
                _sleepState.ChangedEvent -= OnChangedSleepStateValue;
                _microphoneAnalyzer.MaxDecibelRecordedEvent -= OnMaxDecibelRecorder;
            }
        }

        private void OnMaxDecibelRecorder()
        {
            RunNode(_subNode_reactionToVoice);
        }

        private void OnClickSeries(int clickCount)
        {
            if (clickCount >= 5)
            {
                Rouse();
            }
        }

        private void OnChangedSleepStateValue(float sleepValue)
        {
            if (_sleepState.GetPercent() > 0.9f)
            {
                Debugging.Instance.Log($"Нода сна: сон на максимальном значение", Debugging.Type.BehaviorTree);
                StopSleep();
            }
        }
    }
}