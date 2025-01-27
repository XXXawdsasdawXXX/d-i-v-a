using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.GameLoop
{
    public interface IGameListeners
    {
    }

    public interface ISubscriber : IGameListeners
    {
        UniTask Subscribe();
        void Unsubscribe();
    }
    
    public  interface IInitListener : IGameListeners
    {
        UniTask GameInitialize();
    }

    public interface ILoadListener : IGameListeners
    {
        UniTask GameLoad();
    }

    public interface IStartListener : IGameListeners
    {
        UniTask GameStart();
    }

    public interface IUpdateListener : IGameListeners
    {
        void GameUpdate();
    }
    
    public interface IExitListener : IGameListeners
    {
        void GameExit();
    }
}