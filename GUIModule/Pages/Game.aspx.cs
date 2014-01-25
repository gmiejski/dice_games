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
using System.Data;

namespace GUIModule
{
    public partial class Game : System.Web.UI.Page
    {
        private IServer _server;
        private int _orderBy;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                _orderBy = (int)ViewState["orderBy"];
            }
            else
            {
                ViewState["orderBy"] = _orderBy = 0;
            }

            _server = Global.server;
            if ((Session["playerName"] == null) || (Session["gameName"] == null))
            {
                Response.Redirect("~/Pages/Login.aspx");
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
                Response.Redirect("~/Pages/Main.aspx");
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
                
                Players = controller.GetPlayers().OrderBy(delegate(KeyValuePair<string, PlayerState> v) { return v.Key; });
                sort();
                PlayersList.DataSource = Players;
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
            Response.Redirect("~/Pages/Main.aspx", false);
        }

        protected void PlayersSorting(object sender, GridViewSortEventArgs e)
        {
            int oldOrderBy = _orderBy;

            if (e.SortExpression == "Value.CurrentResultValue")
                _orderBy = 2;
            else if (e.SortExpression == "Key")
                _orderBy = 0;
            else _orderBy = 4;

            _orderBy += (oldOrderBy % 2 == 0) ? 1 : 0;
            ViewState["orderBy"] = _orderBy;
            sort();
            PlayersList.DataSource = Players;
            PlayersList.DataBind();
        }

        private void sort()
        {
            if (_orderBy < 2)
            { // sort by key
                if (_orderBy % 2 == 0)
                    Players = Players.OrderBy(delegate(KeyValuePair<string, PlayerState> v1) { return v1.Key; });
                else
                    Players = Players.OrderByDescending(delegate(KeyValuePair<string, PlayerState> v1) { return v1.Key; });
            }
            else
            { // sort by general or current result
                Func<KeyValuePair<string, PlayerState>, int> fun = null;

                if (_orderBy < 4)
                    fun = delegate(KeyValuePair<string, PlayerState> v1) { return v1.Value.CurrentResultValue; };
                else
                    fun = delegate(KeyValuePair<string, PlayerState> v1) { return v1.Value.NumberOfWonRounds; };

                if (_orderBy % 2 == 0)
                    Players = Players.OrderBy(fun);
                else
                    Players = Players.OrderByDescending(fun);
            }
        }

        public string GameName { get; set; }
        public string EncodedGameName { get; set; }
        public string EncodedPlayerName { get; set; }
        public string PlayerName { get; set; }
        public GameData GameData { get; set; }

        public IOrderedEnumerable<KeyValuePair<string, PlayerState>> Players { get; set; }
    }
}
