using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.BaseNodes
{
    public sealed class BehaviourLooper : MonoBehaviour
    {
        private BehaviourNode _rootNode;

        private void FixedUpdate()
        {
            if (_rootNode is { IsRunning: false })
            {
                _rootNode.Run(null);
            }
        }
    }
}