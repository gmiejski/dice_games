using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GUIModule.App_Code
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["playerName"] != null)
            {
                Response.Redirect("~/Pages/Main.aspx", false);
            }
        }

        protected void LoginConfirm_Click(object sender, EventArgs e)
        {
            if (Global.server.RegisterPlayer(LoginName.Text, "context_" + Session["playerName"]))
            {
                Session["playerName"] = LoginName.Text;
                Response.Redirect("~/Pages/Main.aspx", false);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('To imie jest juz zajete!')", true);
            }
        }
    }
}