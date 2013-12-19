
namespace CommonInterfacesModule
{

    public delegate void BroadcastGameStateHandler(string gameName, string gameState);
    public delegate void DeleteGameControllerHandler (string gameName);

    public interface IGameController
    {
        event BroadcastGameStateHandler BroadcastGameState;
        event DeleteGameControllerHandler DeleteGameController;

        GameState GameState { get; set; }

        void MakeMove(string playerName, Move move);

    }
}
