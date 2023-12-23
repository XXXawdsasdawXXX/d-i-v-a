using System.Collections;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.BaseNodes
{
    public sealed class BehaviourNode_WaitForSeconds : BehaviourNode
    {
        private float _seconds;
        private bool _success;
        private Coroutine _coroutine;

        private BehaviourNode_WaitForSeconds()
        {
                
        }
        
        protected override void Run()
        {
            //this._coroutine = this.StartCoroutine(this.WaitForSeconds());
        }

        protected override void OnBreak()
        {
            if (this._coroutine != null)
            {
            //    this.StopCoroutine(this._coroutine);
                this._coroutine = null;
            }
        }

        private IEnumerator WaitForSeconds()
        {
            yield return new WaitForSeconds(this._seconds);
            this._coroutine = null;
            this.Return(this._success);
        }
    }
}