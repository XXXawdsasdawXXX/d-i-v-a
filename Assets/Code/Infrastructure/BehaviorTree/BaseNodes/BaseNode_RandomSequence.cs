using Code.Utils;

namespace Code.Infrastructure.BehaviorTree.BaseNodes
{
    public class BaseNode_RandomSequence : BaseNode, IBehaviourCallback
    {
        private readonly BaseNode[] _orderNodes;

        private BaseNode _currentChild;
        private int _currentNodeIndex;
        
        public BaseNode_RandomSequence(BaseNode[] orderNodes)
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

        void IBehaviourCallback.InvokeCallback(BaseNode node, bool success)
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
                _currentChild = null;
            }
        }

        protected override void OnReturn(bool success)
        {
            //    Debug.Log($"NODE: {name} RETURN: {result}");
        }


    }
}