using UnityEngine;

namespace Code.Infrastructure.BehaviorTree
{
    public abstract class BaseNode 
    {
        //Базовый нод
        public bool IsRunning
        {
            get { return _isRunning; }
        }

        private bool _isRunning;

        private IBehaviourCallback _callback; // через это нод говорит родительскому узлу, что он успешен или не успешен

        //одиновская кнопка
        public void Run(IBehaviourCallback callback)
        {
            if (_isRunning)
            {
                return;
            }

            _callback = callback;
            _isRunning = true;
            Run();
        }

        //одиновская кнопка
        public void Break()
        {
            if (!_isRunning)
            {
                return;
            }

            OnBreak();
            _isRunning = false;
            _callback = null;
        }

        protected abstract void Run();

        protected void Return(bool success)
        {
            if (!_isRunning)
            {
                return;
            }

            _isRunning = false;
            OnReturn(success);
          
            InvokeCallback(success);
        }

        protected virtual void OnReturn(bool success)
        {
        }

        protected virtual void OnBreak()
        {
        }

  
        private void InvokeCallback(bool success)
        {
            if (_callback == null)
            {
                return;
            }

            var callback = _callback;
            _callback = null;
            callback.InvokeCallback(this, success);
        }
    }
}