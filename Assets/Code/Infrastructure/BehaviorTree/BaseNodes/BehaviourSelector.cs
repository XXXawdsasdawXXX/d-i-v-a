using Code.Infrastructure.BehaviorTree.CustomNodes;
using Code.Utils;

namespace Code.Infrastructure.BehaviorTree.BaseNodes
{
    public sealed class BehaviourSelector : BehaviourNode, IBehaviourCallback
    {
        private readonly BehaviourNode[] _orderedNodes;
        private BehaviourNode _currentChild;

        private int _currentChildIndex;
        public BehaviourSelector()
        {
            _orderedNodes = new BehaviourNode[]
            {
                new BehaviorNode_Sleep(),
                new BehaviorNode_Stand(),
                new BehaviorNode_Seat(),
                
                //Eat
                
                //Stand
                
            };
        }
        
        protected override void Run()
        {
            if (_orderedNodes == null || _orderedNodes.Length <= 0)
            {
                Return(false);
                return;
            }

            _currentChildIndex = 0;
            _currentChild = _orderedNodes[_currentChildIndex];
            _currentChild.Run(callback: this);
        }

        void IBehaviourCallback.InvokeCallback(BehaviourNode node, bool success)
        {
            if (success)
            {
                Return(true);
                return;
            }

            if (_currentChildIndex + 1 >= _orderedNodes.Length)
            {
                Return(false);
                return;
            }

            _currentChildIndex++;
            _currentChild = _orderedNodes[_currentChildIndex];
            _currentChild.Run(callback: this);
        }

        protected override void OnBreak()
        {
            if (_currentChild != null && _currentChild.IsRunning)
            {
                _currentChild.Break();
            }
        }
        
        protected override void OnDispose()
        {
            _currentChild = null;
        }
    }
}