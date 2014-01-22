using GUIModule.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonInterfacesModule;
using GUIModule.Pages;
using System.Text;

namespace GUIModule
{
    public partial class Game : System.Web.UI.Page
    {
        private IServer _server;

        protected void Page_Load(object sender, EventArgs e)
        {
            _server = Global.server;
            if ((Session["playerName"] == null) || (Session["gameName"] == null))
            {
                Response.Redirect("Login.aspx");
            }

            PlayerName = Session["playerName"].ToString();
            GameName = Session["gameName"].ToString();

            EncodedGameName = EncodeString(GameName);
            EncodedPlayerName = EncodeString(PlayerName);

            WinnerLabel.Visible = false;

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
                AwaitingPlayersList.Visible = false;
                PlayersList.Visible = true;
                ThrowDice.Visible = true;

                

                if (GameData.Winner != null) {
                    WinnerLabel.Visible = true;
                }

                PlayersList.DataSource = controller.GetPlayers();
                PlayersList.DataBind();

                userDice.DataSource = controller.GetPlayers()[PlayerName].Dices;
                userDice.DataBind();

                ThrowDice.Enabled = GameData.WhoseTurn == PlayerName;
            }
            else
            {
                PlayersList.Visible = false;
                ThrowDice.Visible = false;
                AwaitingPlayersList.Visible = true;

                AwaitingPlayersList.DataSource = controller.GetPlayers();
                AwaitingPlayersList.DataBind();
            }
        }

        private string EncodeString(string text)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in text)
            {
                string encodedValue = "\\u" + ((int)c).ToString("x4");
                sb.Append(encodedValue);
            }
            return sb.ToString();
        }

        public void PlayersDataBound(object sender, EventArgs e)
        {
            foreach (var row in PlayersList.Rows.OfType<GridViewRow>()
                .Where(row => row.Cells[0].Text == GameData.WhoseTurn))
            {
                row.CssClass = "playerHasTurn";
            }

            foreach (var row in PlayersList.Rows.OfType<GridViewRow>()
                .Where(row => row.Cells[0].Text == PlayerName))
            {
                row.Cells[0].CssClass = "ourPlayer";
            }
        }

        protected void LeaveGame_Click(object sender, EventArgs e)
        {
            _server.RemovePlayer(PlayerName);
            Session["gameName"] = null;
            Response.Redirect("Main.aspx", false);
        }

        public string GameName { get; set; }
        public string EncodedGameName { get; set; }
        public string EncodedPlayerName { get; set; }
        public string PlayerName { get; set; }
        public GameData GameData { get; set; }
    }
}
