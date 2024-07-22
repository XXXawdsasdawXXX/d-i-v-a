using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.BaseNodes
{
    public sealed class BaseNode_Sequence : BaseNode, IBehaviourCallback
    {
        private BaseNode[] _orderNodes;// порядок элементов имеет значение

        private BaseNode _currentChild;
        private int _currentNodeIndex;
        
        public BaseNode_Sequence(BaseNode[] orderNodes)
        {
            _orderNodes = orderNodes;
        }
        
        protected override void Run()
        {
            if (IsCanRun())
            {
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