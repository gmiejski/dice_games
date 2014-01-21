
namespace CommonInterfacesModule
{


    public delegate void BroadcastGameStateHandler(string gameName);
    public delegate bool DeleteGameControllerHandler (string gameName);

    public interface IGameController
    {
        event BroadcastGameStateHandler BroadcastGameState;
        event DeleteGameControllerHandler DeleteGameController;

        GameState GameState { get; set; }

        bool MakeMove(string playerName, Move move);

    }
}
