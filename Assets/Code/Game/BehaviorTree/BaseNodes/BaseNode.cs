namespace Code.Game.BehaviorTree
{
    public abstract class BaseNode
    {
        public bool IsRunning => _isRunning;

        private bool _isRunning;

        private IBehaviourCallback _callback;

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

        protected abstract bool IsCanRun();

        protected void Return(bool success)
        {
            if (!_isRunning)
            {
                return;
            }

            _isRunning = false;
         
            OnReturn(success);
            
            _invokeCallback(success);
        }

        protected virtual void OnReturn(bool success)
        {
        }

        protected virtual void OnBreak()
        {
        }


        private void _invokeCallback(bool success)
        {
            if (_callback == null)
            {
                return;
            }

            IBehaviourCallback callback = _callback;
            
            _callback = null;
            
            callback.InvokeCallback(this, success);
        }
    }
}