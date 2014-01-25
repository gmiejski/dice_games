using GUIModule.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonInterfacesModule;

namespace GUIModule.Pages
{
    public partial class Main : System.Web.UI.Page
    {
        private IServer server;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["playerName"] == null) {
                Response.Redirect("~/Pages/Login.aspx", false);
            }
            PlayerName = (string)Session["playerName"];

            server = Global.server;
            var games = server.GetAvailableGames();
            AvailableGamesTable.DataSource = games;
            AvailableGamesTable.DataBind();
        }

        public void JoinGame_Click(object sender, EventArgs e)
        {
            string joinedGameName = ((Button)sender).CommandArgument;

            if (Global.server.JoinGame(PlayerName, joinedGameName))
            {
                Session["gameName"] = joinedGameName;
                if (Session["playerName"] == null) {
                    Response.Redirect("~/Pages/Login.aspx");
                }
                Response.Redirect("~/Pages/Game.aspx", false);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('Nie udało się dołączyć do gry!')", true);
            }
        }

        public string PlayerName { get; set; }

        protected void NewGame_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/NewGame.aspx");
        }

        protected void Logout_Click(object sender, EventArgs e)
        {
            Session["playerName"] = null;
            server.UnregisterPlayer(PlayerName);
            Response.Redirect("~/Pages/Login.aspx", false);
        }
    }
}