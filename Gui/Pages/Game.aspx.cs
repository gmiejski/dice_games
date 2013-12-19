using Gui.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gui
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
            //Session["playerName"] = "Edek";
            //Session["gameName"] = "Gra";
            if ((Session["playerName"] == null) || (Session["gameName"] == null))
            {
                throw new InvalidOperationException("Blad " + Session["playerName"] + " | " + Session["gameName"]);
            }

            PlayerName = Session["playerName"].ToString();
            GameName = Session["gameName"].ToString();
            GameData = new GameData();

            winnerLabel.Visible = false;
            if (_server.GetGameState(GameName) != null) // gra trwa
            {
                awaitingPlayersList.Visible = false;
                playersList.Visible = true;
                throwDice.Visible = true;

                GameData.Name = GameName;
                var currGame = _server.GetGameState(GameName);
                GameData.State = (currGame.IsOver ? "zakończona" : "trwa");
                GameData.WhoseTurn = currGame.WhoseTurn;
                GameData.Winner = currGame.WinnerName;
                
                if (GameData.Winner != null) {
                    winnerLabel.Visible = true;
                }

                playersList.DataSource = currGame.PlayerStates;
                playersList.DataBind();

                userDice.DataSource = currGame.PlayerStates[PlayerName].Dices;
                userDice.DataBind();

                throwDice.Enabled = (currGame.WhoseTurn == PlayerName);
            }
            else
            {
                playersList.Visible = false;
                throwDice.Visible = false;
                awaitingPlayersList.Visible = true;

                var availableGame = (from game in _server.GetAvailableGames()
                    where game.GameName == GameName
                    select game);
                if (availableGame.Count() > 0)
                {
                    var currGame = availableGame.First();
                    GameData.Name = currGame.GameName;
                    GameData.State = "oczekiwanie na graczy";

                    awaitingPlayersList.DataSource = currGame.PlayerNames;
                    awaitingPlayersList.DataBind();
                }
                else
                {
                    Session["gameName"] = null;
                    Response.Redirect("Main.aspx", false);
                }
            }
            
        }

        public void PlayersDataBound(object sender, EventArgs e)
        {
            for (int i = 0; i < playersList.Rows.Count; i++)
            {
                if (playersList.Rows[i].Cells[0].Text == GameData.WhoseTurn) {
                    playersList.Rows[i].CssClass = "playerHasTurn";
                }
            }
        }

        protected void LeaveGame_Click(object sender, EventArgs e)
        {
            _server.DeleteGame(GameName);
            Session["gameName"] = null;
            Response.Redirect("Main.aspx");
        }
    }
}
