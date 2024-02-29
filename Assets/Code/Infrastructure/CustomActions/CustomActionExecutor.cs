using System.Linq;
using Code.Data.Interfaces;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;

namespace Code.Infrastructure.CustomActions
{
    public class CustomActionExecutor: IService,IGameInitListener
    {
        private CustomAction[] _actions;
        public void GameInit()
        {
            _actions = Container.Instance.GetCustomActions();
        }

        public void InvokeCustomAction(CustomCutsceneActionType actionType)
        {
            _actions.FirstOrDefault(a => a.GetActionType() == actionType)?.StartAction();
        }
    }
}