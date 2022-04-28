using System;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using FlareWorks.Library.Database;
using FlareWorks.MemoryMgmt;
using FlareWorks.Models.ControlledValues;
using FlareWorks.Models.Users;

namespace FlareworksWeb.Admin
{
    public partial class UserSingleMgmt : System.Web.UI.Page
    {
        private UserInfo currentUser;
        private UserInfo editUser;

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

            // Try to get the user by the query id
            string user_id = Request.QueryString["id"];
            int user_id_int;
            if ((!String.IsNullOrEmpty(user_id)) && ( int.TryParse(user_id, out user_id_int)))
            {
                editUser = DatabaseGateway.Get_User(user_id_int);
            }

            // If no edit user, go back to main user edit screen
            if (editUser == null)
            {
                Response.Redirect("UserMgmt.aspx");
                return;
            }

            // Fill in the drop down list
            if (LocationDropDownList.Items.Count == 0)
            {
                foreach (LocationInfo location in ApplicationCache.Locations)
                {
                    LocationDropDownList.Items.Add(new ListItem(location.Code, location.ID.ToString()));
                }
            }

            // Determine the user note at the top, if there is one.
            if (editUser.PendingApproval)
            {
                UserNotesLabel.Text = "This user registered on " + editUser.DateAdded.ToShortDateString() + " and is still pending approval.<br /><br />";
            } 
            else if (editUser.Disabled)
            {
                UserNotesLabel.Text = "This user is currently disabled.<br /><br />";
            }

            // If this isnt post back, add the item information
            if (!IsPostBack)
            {
                UserNameLabel.Text = editUser.UserName;
                FullNameLabel.Text = editUser.FullName;
                EmailTextBox.Text = editUser.Email;
                DisplayNameTextBox.Text = editUser.DisplayName;
                CanQcCheckbox.Checked = editUser.Permissions.CanQC;
                CanPullReportsCheckBox.Checked = editUser.Permissions.CanRunReports;
                CanAddToPullListCheckBox.Checked = editUser.Permissions.CanAddToPullLists;
                IsPullListAdminCheckBox.Checked = editUser.Permissions.IsPullListAdmin;
                IsSystemAdminCheckBox.Checked = editUser.Permissions.IsSystemAdmin;
                ActiveCheckBox.Checked = ((!editUser.PendingApproval) && (!editUser.Disabled));
                PccCategoryActive.Checked = editUser.Permissions.CatalogingSpecialist;
            }
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("UserMgmt.aspx");
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            // We have just a couple fields to validate here
            string email = EmailTextBox.Text.Trim();
            string display_name = DisplayNameTextBox.Text.Trim();

            // Try to validate the email address
            if (email.Length == 0)
            {
                ErrorLabel.Text = "<div id=\"login_register_error\">Email address is required.</div>";
                return;
            }
            else
            {
                bool isEmail = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
                if (!isEmail)
                {
                    ErrorLabel.Text = "<div id=\"login_register_error\">Email address is not valid.</div>";
                    return;
                }
            }

            // Validate the display name
            if (display_name.Length < 3 )
            {
                ErrorLabel.Text = "<div id=\"login_register_error\">Display name must be at least three characters long.</div>";
                return;
            } 
            else if (display_name.Length > 30)
            {
                ErrorLabel.Text = "<div id=\"login_register_error\">Display name is too long.  It must be less than thirty characters.</div>";
                return;
            }


            // Build the user object by copying everything over first
            UserInfo newUser = new UserInfo();
            newUser.PrimaryKey = editUser.PrimaryKey;
            newUser.UserName = editUser.UserName;
            newUser.FullName = editUser.FullName;

            // Populate some of the account info from the controls
            newUser.DisplayName = display_name;
            newUser.Email = email;

            // Now, populate from the checkboxes 
            newUser.Permissions.CatalogingSpecialist = PccCategoryActive.Checked;
            newUser.Permissions.CanQC = CanQcCheckbox.Checked;
            newUser.Permissions.CanRunReports = CanPullReportsCheckBox.Checked;
            newUser.Permissions.CanAddToPullLists = CanAddToPullListCheckBox.Checked;
            newUser.Permissions.IsPullListAdmin = IsPullListAdminCheckBox.Checked;
            newUser.Permissions.IsSystemAdmin = IsSystemAdminCheckBox.Checked;

            // For now, just set these both
            newUser.PendingApproval = !ActiveCheckBox.Checked;
            newUser.Disabled = !ActiveCheckBox.Checked;

            newUser.Location = new LocationInfo(int.Parse(LocationDropDownList.SelectedValue), LocationDropDownList.SelectedItem.Text, LocationDropDownList.SelectedItem.Text);

            // Now, save this to the database
            if (!DatabaseGateway.Save_User(newUser))
            {
                ErrorLabel.Text = "<div id=\"login_register_error\">SYSTEM ERROR: Unable to save your changes.<br /><br />Please alert your IT support at Mark.V.Sullivan@sobekdigital.com.</div>";
                return;
            }

            // Saved just fine, so send back to the user list
            Response.Redirect("UserMgmt.aspx");
        }
    }
}