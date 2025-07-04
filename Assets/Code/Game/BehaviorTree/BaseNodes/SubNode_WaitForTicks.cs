using Code.Data;
using Code.Game.Services;
using Code.Utils;

namespace Code.Game.BehaviorTree
{
    public sealed class SubNode_WaitForTicks : BaseNode
    {
        private readonly TickCounter _tickCounter;
        private readonly RangedInt _cooldownRangedTick;

        public SubNode_WaitForTicks(RangedInt cooldownRangedTick)
        {
            _cooldownRangedTick = cooldownRangedTick;
            _tickCounter = new TickCounter();
            _tickCounter.OnWaitIsOver += () => Return(true);
        }

        protected override void Run()
        {
            if (IsCanRun())
            {
                int tickCount = _cooldownRangedTick.GetRandomValue();
                _tickCounter.StartWait(tickCount);
                Log.Info($"Caб нода ожидания: запуск. количество тиков {tickCount}",
                    Log.Type.BehaviorTree);
            }
            else
            {
                Return(false);
            }
        }

        protected override bool IsCanRun()
        {
            return _tickCounter.IsExpectedStart;
        }

        protected override void OnBreak()
        {
            _tickCounter.StopWait();
        }
    }
}