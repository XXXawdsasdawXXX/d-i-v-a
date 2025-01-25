namespace Code.Infrastructure.GameLoop
{
    public interface IGameListeners
    {
    }

    internal interface IInitListener : IGameListeners
    {
        void GameInitialize();
    }

    public interface ILoadListener : IGameListeners
    {
        void GameLoad();
    }

    public interface IStartListener : IGameListeners
    {
        void GameStart();
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