namespace Code.Infrastructure.GameLoop
{
    public interface IGameListeners
    {
        
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