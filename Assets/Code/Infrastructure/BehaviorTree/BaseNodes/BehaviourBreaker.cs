using Code.Data.Enums;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.BaseNodes
{
    public sealed class BehaviourBreaker : MonoBehaviour
    {
        private BehaviorTreeBlackboard _blackboard;
        private BehaviourNode rootNode;

        private void OnEnable()
        {
            this._blackboard.OnVariableChanged += this.OnVariableChanged;
            this._blackboard.OnVariableRemoved += this.OnVariableChanged;
        }
        
        private void OnDisable()
        {
            this._blackboard.OnVariableChanged -= this.OnVariableChanged;
            this._blackboard.OnVariableRemoved -= this.OnVariableChanged;
        }

        private void OnVariableChanged(BlackboardKey key, object value)
        {
            if (key == BlackboardKey.None)
            {
                this.rootNode.Break();
            }
        }
    }
}