using Code.Data.Value.RangeInt;
using Code.Infrastructure.Services;
using Code.Utils;

namespace Code.Infrastructure.BehaviorTree.BaseNodes
{
    public sealed class SubNode_WaitForTicks : BaseNode
    {
        private readonly TickCounter _tickCounter;
        private readonly RangedInt _cooldownRangedTick;
        
        public SubNode_WaitForTicks(RangedInt cooldownRangedTick)
        {
            _cooldownRangedTick = cooldownRangedTick;
            _tickCounter = new TickCounter();
            _tickCounter.WaitedEvent += () => Return(true);
        }
        
        protected override void Run()
        {
            if (IsCanRun())
            {
                var tickCount = _cooldownRangedTick.GetRandomValue();
                _tickCounter.StartWait(tickCount);
                Debugging.Instance.Log($"Caб нода ожидания: запуск. количество тиков {tickCount}", Debugging.Type.BehaviorTree);
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