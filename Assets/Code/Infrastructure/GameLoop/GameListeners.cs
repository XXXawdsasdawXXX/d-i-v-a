namespace Code.Infrastructure.GameLoop
{
    public interface IGameListeners
    {
        
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

    public interface IGameSaveListener : IGameListeners
    {
        void GameSave();
        
    }
    public interface IGameExitListener : IGameListeners
    {
        void GameExit();
    }
}