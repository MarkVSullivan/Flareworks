using System;
using FlareWorks.Models.Users;

namespace FlareworksWeb.Admin
{
    public partial class AdminMenu : System.Web.UI.Page
    {
        private UserInfo currentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Look for a user in the session
            currentUser = Session["CurrentUser"] as UserInfo;
            if (currentUser == null)
            {
                Response.Redirect("../UserMgmt/Login.aspx");
                return;
            }

            // Check for permissions
            if ((currentUser.PendingApproval) || (currentUser.Disabled))
            {
                Response.Redirect("../UserMgmt/NoPermissions.aspx");
                return;
            }

            if (!currentUser.Permissions.IsSystemAdmin)
            {
                Response.Redirect("../Default.aspx");
                return;
            }
        }

        protected void UserButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("UserMgmt.aspx");
        }
    }
}