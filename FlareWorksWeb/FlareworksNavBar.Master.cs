using System;
using System.Web;
using FlareWorks.Models.Users;

namespace FlareworksWeb
{
    public partial class FlareworksNavBar : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Add_User_Menus()
        {
            // Get the current user
            UserInfo thisUser = HttpContext.Current.Session["CurrentUser"] as UserInfo;

            // Determine the base url
            string base_url = Request.Url.Scheme + "://" + Request.Url.Authority + "/";

            // Add all the options
            Response.Output.WriteLine("<li><a href=\"" + base_url + "Default.aspx\">Home</a></li>");

            if ((thisUser != null) && (!thisUser.PendingApproval) && (!thisUser.Disabled))
            {
                if ( thisUser.Permissions.CanProcessItems ) Response.Output.WriteLine("<li><a href=\"" + base_url + "TitleEntry.aspx?id=new\">Add an item</a></li>");
                if (thisUser.Permissions.CanQC) Response.Output.WriteLine("<li><a href=\"" + base_url + "QcDashboard.aspx\">Quality Control</a></li>");

                if ( thisUser.Permissions.CanAdvancedSearch )
                    Response.Output.WriteLine("<li><a href=\"" + base_url + "AdminSearch.aspx\">Search</a></li>");
                else
                    Response.Output.WriteLine("<li><a href=\"" + base_url + "Search.aspx\">Search</a></li>");

                if (thisUser.Permissions.CanRunReports) Response.Output.WriteLine("<li><a href=\"" + base_url + "Reports.aspx\">Reports</a></li>");
                if (thisUser.Permissions.IsSystemAdmin) Response.Output.WriteLine("<li><a href=\"" + base_url + "Admin/AdminMenu.aspx\">Admin</a></li>");
            }

            Response.Output.WriteLine("<li><a href=\"" + base_url + "UserMgmt/Logout.aspx\">Logout</a></li>");
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