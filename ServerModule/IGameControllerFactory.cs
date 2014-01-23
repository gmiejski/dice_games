using CommonInterfacesModule;

namespace ServerModule
{
    public interface IGameControllerFactory
    {
        IGameController CreateGameController(CreatedGame createdGame);
    }
}