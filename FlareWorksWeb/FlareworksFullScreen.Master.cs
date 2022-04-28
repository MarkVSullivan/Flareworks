using FlareWorks.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FlareworksWeb
{
    public partial class FlareworksFullScreen : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Add_Js()
        {
            // Determine the base url
            string base_url = Request.Url.Scheme + "://" + Request.Url.Authority + "/";

            // Add all the options
            Response.Output.WriteLine("<script src=\"" + base_url + "/js/flareworks.js\" type=\"text/javascript\"></script>");

        }

        protected void Add_Username()
        {
            // Look for a user in the session
            UserInfo currentUser = Session["CurrentUser"] as UserInfo;
            if (currentUser != null)
            {
                Response.Output.WriteLine(currentUser.DisplayName + " | <a href=\"http://flareworks.sobekdigital.com/UserMgmt/Logout.aspx\">Logout</a>");
            }
        }
    }
}