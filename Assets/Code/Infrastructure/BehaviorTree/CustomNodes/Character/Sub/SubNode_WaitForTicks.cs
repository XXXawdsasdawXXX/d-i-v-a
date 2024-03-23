using Code.Data.Value.RangeFloat;
using Code.Services;
using Code.Utils;

namespace Code.Infrastructure.BehaviorTree.CustomNodes.Character.Sub
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
            if (_tickCounter.IsWaited)
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

        protected override void OnBreak()
        {
            _tickCounter.StopWait();
        }
    }
}