using Code.Utils;

namespace Code.Infrastructure.BehaviorTree.BaseNodes
{
    public class BehaviourNode_RandomSequence : BehaviourNode, IBehaviourCallback
    {
        private readonly BehaviourNode[] _orderNodes;

        private BehaviourNode _currentChild;
        private int _currentNodeIndex;
        
        public BehaviourNode_RandomSequence(BehaviourNode[] orderNodes)
        {
            _orderNodes = orderNodes;
            Extensions.ShuffleArray(_orderNodes);
        }
        
        protected override void Run()
        {
            if (_orderNodes.Length <= 0)
            {
                Return(true);
                return;
            }

            _currentNodeIndex = 0;
            _currentChild = _orderNodes[_currentNodeIndex];
            _currentChild.Run(callback: this);
        }

        void IBehaviourCallback.InvokeCallback(BehaviourNode node, bool success)
        {
            if (!success)
            {
                Return(false);
                return;
            }

            if (_currentNodeIndex + 1 >= _orderNodes.Length)
            {
                Return(true);
                return;
            }

            _currentNodeIndex++;
            _currentChild = _orderNodes[_currentNodeIndex];
            _currentChild.Run(callback: this);
        }

        protected override void OnBreak()
        {
            if (_currentChild != null && _currentChild.IsRunning)
            {
                _currentChild.Break();
            }
        }

        protected override void OnReturn(bool success)
        {
            //    Debug.Log($"NODE: {name} RETURN: {result}");
        }

        protected override void OnDispose()
        {
            _currentChild = null;
        }
    }
}