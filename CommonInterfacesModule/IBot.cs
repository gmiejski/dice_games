using System.Security.Cryptography.X509Certificates;

namespace CommonInterfacesModule
{
    public delegate void BotMovedHandler(string botName, IMove move);

    public interface IBot
    {
        event BotMovedHandler BotMoved;

        IMove GetNextMove(IGameState gameState);
        void SendGameState(IGameState gameState);

    }
}
