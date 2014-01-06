using GUIModule.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonInterfacesModule;
using GUIModule.Pages;

namespace GUIModule
{
    public partial class Game : System.Web.UI.Page
    {
        private IServer _server;
        public string GameName { get; set; }
        public string PlayerName {get; set; }
        public GameData GameData { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            _server = Global.server;
            if ((Session["playerName"] == null) || (Session["gameName"] == null))
            {
                Response.Redirect("Login.aspx");
            }

            PlayerName = Session["playerName"].ToString();
            GameName = Session["gameName"].ToString();

            winnerLabel.Visible = false;

            AbstractGameViewController controller = null;
            try
            {
                controller = AbstractGameViewController.NewInstance(PlayerName, GameName, _server);
            }
            catch (InvalidOperationException)
            {
                Session["gameName"] = null;
                Response.Redirect("Main.aspx");
            }

            // set things like name, winner etc.
            GameData = controller.GetGameData();

            if (controller.IsOngoing()) // gra trwa
            {
                awaitingPlayersList.Visible = false;
                playersList.Visible = true;
                throwDice.Visible = true;

                

                if (GameData.Winner != null) {
                    winnerLabel.Visible = true;
                }

                playersList.DataSource = controller.GetPlayers();
                playersList.DataBind();

                userDice.DataSource = controller.GetPlayers()[PlayerName].Dices;
                userDice.DataBind();

                throwDice.Enabled = GameData.WhoseTurn == PlayerName;
            }
            else
            {
                playersList.Visible = false;
                throwDice.Visible = false;
                awaitingPlayersList.Visible = true;

                awaitingPlayersList.DataSource = controller.GetPlayers();
                awaitingPlayersList.DataBind();
            }
        }

        public void PlayersDataBound(object sender, EventArgs e)
        {
            foreach (var row in playersList.Rows.OfType<GridViewRow>()
                .Where(row => row.Cells[0].Text == GameData.WhoseTurn))
            {
                row.CssClass = "playerHasTurn";
            }
        }

        protected void LeaveGame_Click(object sender, EventArgs e)
        {
            _server.DeleteGame(GameName);
            Session["gameName"] = null;
            Response.Redirect("Main.aspx", false);
        }
    }
}
