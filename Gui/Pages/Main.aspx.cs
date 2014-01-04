using Gui.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonInterfacesModule;

namespace Gui.Pages
{
    public partial class Main : System.Web.UI.Page
    {
        private IServer server;
        protected void Page_Load(object sender, EventArgs e)
        {
            //Session["playerName"] = "Jutrko"; // tymczasowo zeby chodzilo

            if (Session["playerName"] == null) {
                Response.Redirect("Login.aspx", false);
            }
            PlayerName = (string)Session["playerName"];

            server = Global.server;
            var games = server.GetAvailableGames();
            availableGamesTable.DataSource = games;
            availableGamesTable.DataBind();
        }

        public void JoinGame_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(((Button)sender).CommandArgument);
            string joinedGameName = server.GetAvailableGames()[id].GameName;

            if (Global.server.JoinGame(PlayerName, joinedGameName))
            {
                Session["gameName"] = joinedGameName;
                if (Session["playerName"] == null) {
                    throw new InvalidOperationException(Session["playerName"].ToString() + " | " + Session["gameName"].ToString());
                }
                Response.Redirect("Game.aspx", false);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('Nie da sie dolaczyc do gry!')", true);
            }
        }

        public string PlayerName { get; set; }

        protected void NewGame_Click(object sender, EventArgs e)
        {
            Response.Redirect("NewGame.aspx");
        }

        protected void Logout_Click(object sender, EventArgs e)
        {
            Session["playerName"] = null;
            Response.Redirect("Login.aspx", false);
        }
        
    }


}