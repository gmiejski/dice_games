using CommonInterfacesModule;
using Gui.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace Gui
{
    public class Global : System.Web.HttpApplication
    {

        public static IServer server;

        protected void Application_Start(object sender, EventArgs e)
        {

            Application.Lock();
            try
            {
                server = new Server();
            }
            finally
            {
                Application.UnLock();
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {
            
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}