using System.Collections;
using Code.Data.Value.RangeFloat;
using Code.Infrastructure.DI;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.BaseNodes
{
    public sealed class SubNode_WaitForSeconds : BaseNode
    {
        private readonly RangedFloat _cooldown;
        private float _randomSeconds;

        
        private readonly CoroutineRunner _coroutineRunner;
        private Coroutine _coroutine;

        public SubNode_WaitForSeconds(RangedFloat cooldown)
        {
            _cooldown = cooldown;
            _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();
        }
        
        protected override void Run()
        { 
            _randomSeconds = _cooldown.GetRandomValue();
           _coroutine = _coroutineRunner.StartRoutine(WaitForSeconds());
        }

        protected override void OnDispose()
        {
            if (_coroutine != null)
            {
                _coroutineRunner.Stop(_coroutine);
                _coroutine = null;
            }
        }

        private IEnumerator WaitForSeconds()
        {
            Debugging.Instance.Log($"Саб нода ожидания: выбрано. длительность {_randomSeconds}", Debugging.Type.BehaviorTree);
            yield return new WaitForSeconds(_randomSeconds);
            _coroutine = null;
            Return(true);
        }
    }
}