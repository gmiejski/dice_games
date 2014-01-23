using CommonInterfacesModule;
using GUIModule.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GUIModule.Pages
{
    public partial class NewGame : System.Web.UI.Page
    {
        public string PlayerName { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["playerName"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            PlayerName = Session["playerName"].ToString();
        }

        protected void CreateGame_Click(object sender, EventArgs e)
        {
            GameType gameType = GetGameTypeFromString(NewGameType.SelectedValue);

            BotLevel botLevel = BotLevelHard.Checked ? BotLevel.Hard : BotLevel.Easy;

            int numberOfPlayers = 0;
            int numberOfBots = 0;
            int numberOfRounds = 0;

            bool canParseValues = Int32.TryParse(NewGamePlayers.Text, out numberOfPlayers) &&
                Int32.TryParse(NewGameBots.Text, out numberOfBots) &&
                Int32.TryParse(NewGameRounds.Text, out numberOfRounds);

            bool areValuesValid = (numberOfPlayers > 0) && (numberOfBots >= 0) && (numberOfRounds > 0);

            if (canParseValues && areValuesValid) // validate input data
            { // data ok, create a new game
                CreatedGame cg = Global.server.CreateGame(PlayerName, NewGameName.Text, gameType,
                    numberOfPlayers, numberOfBots, botLevel, numberOfRounds);

                if (cg != null)
                {
                    Session["gameName"] = NewGameName.Text;
                    Response.Redirect("Game.aspx", false); // redirect to a new games
                }
                else // for example name is already taken
                {
                    ClientScript.RegisterStartupScript(this.GetType(),
                    "myalert", "alert('Tworzenie gry nie powiodlo sie! (na przyklad ')", true);
                }
            }
            else // error
            {
                ClientScript.RegisterStartupScript(this.GetType(),
                    "myalert", "alert('Liczba graczy, botow lub rund nie jest poprawna liczba!')", true);
            }
        }

        private GameType GetGameTypeFromString(string type)
        {
            switch (type)
            {
                case "Poker":
                    return GameType.Poker;
                case "NPlus":
                    return GameType.NPlus;
                case "NStar":
                    return GameType.NStar;
                default:
                    throw new InvalidProgramException("no game type " + NewGameType.SelectedValue);
            }
        }

        protected void LeaveNewGame_Click(object sender, EventArgs e)
        {
            Response.Redirect("Main.aspx");
        }
    }
}