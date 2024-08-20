namespace Code.Infrastructure.GameLoop
{
    public interface IGameListeners
    {
    }

    internal interface IGameInitListener : IGameListeners
    {
        void GameInit();
    }

    public interface IGameLoadListener : IGameListeners
    {
        void GameLoad();
    }

    public interface IGameStartListener : IGameListeners
    {
        void GameStart();
    }

    public interface IGameTickListener : IGameListeners
    {
        void GameTick();
    }


    public interface IGameExitListener : IGameListeners
    {
        void GameExit();
    }
}