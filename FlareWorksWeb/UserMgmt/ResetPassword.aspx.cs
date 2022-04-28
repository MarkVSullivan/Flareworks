using System;
using System.Text;
using FlareWorks.Library.Database;
using FlareWorks.Models.Users;

namespace FlareworksWeb.UserMgmt
{
    public partial class ResetPassword : System.Web.UI.Page
    {
        private UserInfo currentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Look for a user in the session
            currentUser = Session["CurrentUser"] as UserInfo;
            if (currentUser == null)
            {
                Response.Redirect("~/UserMgmt/Login.aspx");
                return;
            }

            // Check for permissions
            if ((currentUser.PendingApproval) || (currentUser.Disabled))
            {
                Response.Redirect("~/UserMgmt/NoPermissions.aspx");
                return;
            }

            UserNameLabel.Text = currentUser.UserName;
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            // Get all the values from the boxes
            string password = PasswordTextBox.Text.Trim();
            string password2 = PasswordTextBox2.Text.Trim();

            if ((password.Length == 0) || (password2.Length == 0)) 
            {
                ErrorLabel.Text = "<div id=\"login_register_error\">Password (and confirmation) is required.<br /></div>";
                return;
            }

            // Username and password basic validation
            StringBuilder errorBuilder = new StringBuilder();
            if (password.Length < 8) errorBuilder.Append("Password must be eight digits long at least.");
            else if (password == currentUser.UserName) errorBuilder.Append("Password cannot be the same as your username.");
            else if (password != password2) errorBuilder.Append("Passwords do not match.");

            // If username/password basic validation failed, show it
            if (errorBuilder.Length > 0)
            {
                ErrorLabel.Text = "<div id=\"login_register_error\">" + errorBuilder.ToString() + "</div>";
                return;
            }

            // Reset this password
            if (!DatabaseGateway.Reset_User_Password(currentUser.PrimaryKey, password, false))
            {
                ErrorLabel.Text = "<span style=\"color:Red;font-weight:bold;margin-left:20px;\">ERROR reseting password<span><br /><br />";
                return;
            }

            Response.Redirect("../Default.aspx");
        }
    }
}