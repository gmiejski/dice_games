using CommonInterfacesModule;
using Gui.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gui.Pages
{
    public partial class NewGame : System.Web.UI.Page
    {
        public string PlayerName { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            PlayerName = Session["playerName"].ToString();
        }

        protected void CreateGame_Click(object sender, EventArgs e)
        {
            Global.server.CreateGame(PlayerName, newGameName.Text, GameType.NStar, 2, 3, BotLevel.Easy);
            Session["gameName"] = newGameName.Text;
            Response.Redirect("Game.aspx", false);
        }

        protected void LeaveNewGame_Click(object sender, EventArgs e)
        {
            Response.Redirect("Main.aspx");
        }
    }
}