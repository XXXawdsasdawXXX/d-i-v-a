using Code.Utils;

namespace Code.BehaviorTree
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
            if (IsCanRun())
            {
#if DEBUGGING
                Log.Info(this, "[run]", Log.Type.BehaviorTree);
#endif
                _currentNodeIndex = 0;
                _currentChild = _orderNodes[_currentNodeIndex];
                _currentChild.Run(callback: this);
                return;
            }

            Return(false);
        }

        protected override bool IsCanRun()
        {
            return _orderNodes is { Length: > 0 };
        }

        void IBehaviourCallback.InvokeCallback(BaseNode node, bool success)
        {
#if DEBUGGING
            Log.Info(this, $"[InvokeCallback] Success {success}.", Log.Type.BehaviorTree);
#endif
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
#if DEBUGGING
            Log.Info(this, $"[InvokeCallback] Run next node {_currentChild.GetType().Name}", Log.Type.BehaviorTree);
#endif
            _currentChild.Run(callback: this);
        }

        protected override void OnBreak()
        {
            if (_currentChild != null && _currentChild.IsRunning)
            {
                _currentChild.Break();
                
                _currentChild = null;
#if DEBUGGING
                Log.Info(this, "[break]", Log.Type.BehaviorTree);
#endif
            }
            else
            {
#if DEBUGGING
                Log.Info(this, "[break] Has not child node.", Log.Type.BehaviorTree);
#endif
            }
        }
    }
}