using System.Collections;
using Code.Data.Value.RangeFloat;
using Code.Infrastructure.DI;
using Code.Services;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.BaseNodes
{
    public sealed class BehaviourNode_WaitForSeconds : BehaviourNode
    {
        private readonly RangedFloat _cooldown;
        private float _randomSeconds;

        
        private readonly CoroutineRunner _coroutineRunner;
        private Coroutine _coroutine;

        public BehaviourNode_WaitForSeconds(RangedFloat cooldown)
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
            yield return new WaitForSeconds(_randomSeconds);
            _coroutine = null;
            Return(true);
        }
    }
}