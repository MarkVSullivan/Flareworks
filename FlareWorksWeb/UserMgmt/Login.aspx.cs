using System;
using System.Text;
using System.Web;
using FlareWorks.Library.Database;
using FlareWorks.Models.Users;

namespace FlareworksWeb.UserMgmt
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Look for a user in the session
            UserInfo currentUser = UserChecks.Check_Current_User();
            if (currentUser != null)
            {
                Response.Redirect("../Default.aspx");
                return;
            }
        }

        protected void LogInButton_Click(object sender, EventArgs e)
        {
            // Get the values
            string username = UserNameTextBox.Text.Trim();
            string password = PasswordTextBox.Text.Trim();

            // Do some very basic validation first
            StringBuilder errorBuilder = new StringBuilder();
            if (username.Length == 0) errorBuilder.Append("UserName (or email) is required.<br />");
            if (password.Length == 0) errorBuilder.Append("Password is required.<br />");

            // If very basic validation failed, show it
            if (errorBuilder.Length > 0)
            {
                ErrorLabel.Text = "<div id=\"login_register_error\">" + errorBuilder.ToString() + "</div>";
                return;
            }

            // Try to get this user
            UserInfo user = DatabaseGateway.Get_User(username, password);
           // user = DatabaseGateway.Get_User(21);

            // If this is NULL now, there was an issue!
            if (user == null)
            {
                ErrorLabel.Text = "<div id=\"login_register_error\">Invalid username/password combination.<br /><br />If you think this is in error, please contact your manager.</div>";
                return;
            }

            // Set as the current user
            HttpContext.Current.Session["CurrentUser"] = user;

            // Add a cookie to this
            HttpCookie userCookie = new HttpCookie("FlareUser");
            userCookie.Values["userid"] = user.PrimaryKey.ToString();
            userCookie.Values["security_hash"] = user.Security_Hash(HttpContext.Current.Request.UserHostAddress);
            userCookie.Expires = DateTime.Now.AddDays(14);
            HttpContext.Current.Response.Cookies.Add(userCookie);

            // Was this a temporary password?
            if (user.TemporaryPassword)
            {
                Response.Redirect("ResetPassword.aspx");
                return;
            }

            // If completely successful, send to the Thank you screen
            Response.Redirect("../Default.aspx");
        }
    }
}