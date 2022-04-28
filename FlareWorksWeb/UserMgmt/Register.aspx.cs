using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FlareWorks.Library.Database;
using FlareWorks.MemoryMgmt;
using FlareWorks.Models.ControlledValues;
using FlareWorks.Models.Users;

namespace FlareworksWeb.UserMgmt
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (LocationDropDownList.Items.Count == 0)
            {
                foreach (LocationInfo location in ApplicationCache.Locations)
                {
                    LocationDropDownList.Items.Add(new ListItem(location.Code, location.ID.ToString()));
                }
            }
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx");

        }

        protected void RegisterButton_Click(object sender, EventArgs e)
        {
            // Get all the values from the boxes
            string username = UserNameTextBox.Text.Trim();
            string password = PasswordTextBox.Text.Trim();
            string password2 = PasswordTextBox2.Text.Trim();
            string first_name = FirstNameTextBox.Text.Trim();
            string last_name = LastNameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string location = LocationDropDownList.SelectedItem.Value;

            // Do some very basic validation first
            StringBuilder errorBuilder = new StringBuilder();
            if (username.Length == 0) errorBuilder.Append("UserName is required.<br />");
            if ((password.Length == 0) || ( password2.Length == 0 )) errorBuilder.Append("Password (and confirmation) is required.<br />");
            if (first_name.Length == 0) errorBuilder.Append("Your first name is required.<br />");
            if (last_name.Length == 0) errorBuilder.Append("Your last name is required.<br />");
            if (email.Length == 0) errorBuilder.Append("Email address is required.<br />");
 
            // If very basic validation failed, show it
            if (errorBuilder.Length > 0)
            {
                ErrorLabel.Text = "<div id=\"login_register_error\">" + errorBuilder.ToString() + "</div>";
                return;
            }

            // Username and password basic validation
            if (username.Length < 8) errorBuilder.Append("Username must be eight digits long at least.");
            if (password.Length < 8) errorBuilder.Append("Password must be eight digits long at least.");
            else if (password == username) errorBuilder.Append("Password cannot be the same as your username.");
            else if (password != password2) errorBuilder.Append("Passwords do not match.");

            // If username/password basic validation failed, show it
            if (errorBuilder.Length > 0)
            {
                ErrorLabel.Text = "<div id=\"login_register_error\">" + errorBuilder.ToString() + "</div>";
                return;
            }

            // Try to validate the email address
            bool isEmail = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            if (!isEmail)
            {
                ErrorLabel.Text = "<div id=\"login_register_error\">Email address is not valid.</div>";
                return;
            }

            bool email_exists;
            bool username_exists;
            DatabaseGateway.UserName_Exists(username, email, out username_exists, out email_exists);
            if (email_exists)
            {
                ErrorLabel.Text = "<div id=\"login_register_error\">An account for that email address already exists.</div>";
                return;
            }
            else if (username_exists)
            {
                ErrorLabel.Text = "<div id=\"login_register_error\">That username is taken.  Please choose another.</div>";
                return;
            }

            // Ensure the last name and first name are capitalized somewhat
            bool all_caps = true;
            bool all_lower = true;
            foreach (char thisChar in first_name)
            {
                if (Char.IsUpper(thisChar))
                    all_lower = false;
                if (Char.IsLower(thisChar))
                    all_caps = false;

                if ((!all_caps) && (!all_lower))
                    break;
            }
            if ((all_caps) || (all_lower))
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                first_name = textInfo.ToTitleCase(first_name.ToLower()); //War And Peace
            }
            all_lower = true;
            all_caps = true;
            foreach (char thisChar in last_name)
            {
                if (Char.IsUpper(thisChar))
                    all_lower = false;
                if (Char.IsLower(thisChar))
                    all_caps = false;

                if ((!all_caps) && (!all_lower))
                    break;
            }
            if ((all_caps) || (all_lower))
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                last_name = textInfo.ToTitleCase(last_name.ToLower()); //War And Peace
            }

            // Register this user
            DatabaseGateway.Register_User(username, password, first_name + " " + last_name, email, location);

            // Retrieve the user from the database
            UserInfo user = DatabaseGateway.Get_User(username, password);

            // If this is NULL now, there was an issue!
            if (user == null)
            {
                ErrorLabel.Text = "<div id=\"login_register_error\">SYSTEM ERROR: Unable to register valid new user.<br /><br />Please alert your IT support at Mark.V.Sullivan@sobekdigital.com.</div>";
                return;
            }

            // Set as the current user
            HttpContext.Current.Session["CurrentUser"] = user;

            // If completely successful, send to the Thank you screen
            Response.Redirect("ThankYou.aspx");
        }
    }
}