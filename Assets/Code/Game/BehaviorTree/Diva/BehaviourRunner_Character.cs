using Code.Infrastructure.GameLoop;
using Code.Infrastructure.ServiceLocator;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Game.BehaviorTree.Diva
{
    public sealed class BehaviourRunner_Character : MonoBehaviour, IService, IStartListener ,IUpdateListener
    {
        [SerializeField] private bool _isRun;

        private BaseNode _rootNode;
        
        public UniTask GameStart()
        {
            _rootNode = new BehaviourSelector_Character();
        
            return UniTask.CompletedTask;
        }

        public void GameUpdate()
        {
            if (!_isRun)
            {
                return;
            }

            if (_rootNode is { IsRunning: false })
            {
                _rootNode.Run(null);
            }
        }
    }
}