
namespace CommonInterfacesModule
{
    public delegate void BotMovedHandler(string botName, Move move);
    
    public interface IBot
    {
        
        event BotMovedHandler BotMoved;

        string Name { get; set; }

        Move GetNextMove(GameState gameState);
        void SendGameState(GameState gameState);

    }
}
