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

            Debugging.Instance.Log($"Рандомная сиквенция: старт", Debugging.Type.BehaviorTree);
            _currentNodeIndex = 0;
            _currentChild = _orderNodes[_currentNodeIndex];
            _currentChild.Run(callback: this);
        }

        void IBehaviourCallback.InvokeCallback(BaseNode node, bool success)
        {
            Debugging.Instance.Log($"Рандомная сиквенция: калбэк {success}", Debugging.Type.BehaviorTree);
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

            Debugging.Instance.Log($"Рандомная сиквенция: калбэк следующая нода", Debugging.Type.BehaviorTree);
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
                Debugging.Instance.Log($"Рандомная сиквенция: брейк", Debugging.Type.BehaviorTree);
            }
            else
            {
                Debugging.Instance.Log($"Рандомная сиквенция: брейк -> нет дочерней ноды", Debugging.Type.BehaviorTree);
                
            }
        }


    }
}