namespace CommonInterfacesModule
{
	public class GameControllerFactory
	{
	    private GameControllerFactory()
	    {
	    }

	    public static IGameController CreateGameController(ICreatedGame createdGame)
		{
			throw new System.NotImplementedException();
		}

	}
}

