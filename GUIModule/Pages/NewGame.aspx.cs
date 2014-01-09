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
            
            Global.server.CreateGame(PlayerName, NewGameName.Text, gameType, 
                Int32.Parse(NewGamePlayers.Text), Int32.Parse(NewGameBots.Text), botLevel);
            Session["gameName"] = NewGameName.Text;
            Response.Redirect("Game.aspx", false);
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