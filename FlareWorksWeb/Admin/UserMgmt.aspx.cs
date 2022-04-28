using System;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Web;
using FlareWorks.Library.Database;
using FlareWorks.Library.Tools;
using FlareWorks.Models.Users;

namespace FlareworksWeb.Admin
{
    public partial class UserMgmt : System.Web.UI.Page
    {
        private UserInfo currentUser;
        private DataTable allUsers;

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

            // Get the list of all users
            allUsers = DatabaseGateway.Get_All_Users();

            if (IsPostBack)
            {
                // Pull the standard values
                NameValueCollection form = HttpContext.Current.Request.Form;

                // Get the curret action
                string action = form["admin_user_action"];
                string id = form["admin_user_id"];

                if (action == "reset")
                {
                    int id_int;
                    if (int.TryParse(id, out id_int))
                    {
                        // Get this user first
                        UserInfo resetUser = DatabaseGateway.Get_User(id_int);

                        // Only continue if there was a match
                        if (resetUser != null)
                        {
                            // Create the random password
                            StringBuilder passwordBuilder = new StringBuilder();
                            Random randomGenerator = new Random(DateTime.Now.Millisecond);
                            while (passwordBuilder.Length < 12)
                            {
                                switch (randomGenerator.Next(0, 3))
                                {
                                    case 0:
                                        int randomNumber = randomGenerator.Next(65, 91);
                                        if ((randomNumber != 79) && (randomNumber != 75)) // Omit the 'O' and the 'K', confusing
                                            passwordBuilder.Append((char) randomNumber);
                                        break;

                                    case 1:
                                        int randomNumber2 = randomGenerator.Next(97, 123);
                                        if ((randomNumber2 != 111) && (randomNumber2 != 108) && (randomNumber2 != 107)) // Omit the 'o' and the 'l' and the 'k', confusing
                                            passwordBuilder.Append((char) randomNumber2);
                                        break;

                                    case 2:
                                        // Zero and one is omitted in this range, confusing
                                        int randomNumber3 = randomGenerator.Next(50, 58);
                                        passwordBuilder.Append((char) randomNumber3);
                                        break;
                                }
                            }
                            string new_password = passwordBuilder.ToString();

                            // Reset this password
                            if (!DatabaseGateway.Reset_User_Password(id_int, new_password, true))
                            {
                                ErrorLabel.Text = "<span style=\"color:Red;font-weight:bold;margin-left:20px;\">ERROR reseting password<span><br /><br />";
                            }
                            else
                            {
                                //if (Email_Helper.SendEmail(resetUser.Email, "FlareWorks Password Reset", resetUser.DisplayName + ",\n\nYour FlareWorks password has been reset to a temporary password.  The first time you logon, you will be required to change it.\n\n\tUsername: " + resetUser.UserName + "\n\tPassword: " + new_password + "\n\nYour password is case-sensitive and must be entered exactly as it appears above when logging on.\n\nIf you have any questions or problems logging on, feel free to contact us at Mark.V.Sullivan@sobekdigital.com, or reply to this email.\n\nhttp://flareworks.sobekdigital.com\n", false))
                                //{
                                ErrorLabel.Text = "<span style=\"color:#006600;font-weight:bold;margin-left:20px;\">Reset of password (" + new_password + ") for '" + resetUser.DisplayName + "' complete.</span><br /><br />";
                                //}
                                //else
                                //{
                                //    ErrorLabel.Text = "ERROR while sending new password (" + new_password + ") for '" + resetUser.DisplayName + "'!";
                                //}
                            }
                        }
                    }
                }
            }
        }


        protected void Add_User_Table()
        {
            // Handle some fringe cases first
            if ((allUsers == null) || ( allUsers.Rows.Count == 0 ))
            {
                Response.Output.WriteLine("<p>SYSTEM ERROR: Unable to retrieve any users from the database.</p>");
                Response.Output.WriteLine("<p>Please alert your IT support at Mark.V.Sullivan@sobekdigital.com.</p>");
                return;
            }

            // Add this table
            Response.Output.WriteLine("<table id=\"user_table\" class=\"admin_table\" cellspacing=\"0\">");
            Response.Output.WriteLine("  <tr>");
            Response.Output.WriteLine("    <th> &nbsp; Actions</th>");
            Response.Output.WriteLine("    <th>UserName</th>");
            Response.Output.WriteLine("    <th>Name</th>");
            Response.Output.WriteLine("    <th>Location</th>");
            Response.Output.WriteLine("    <th style=\"width:80px\">Can QC</th>");
            Response.Output.WriteLine("    <th style=\"width:80px\">Reports</th>");
            Response.Output.WriteLine("    <th style=\"width:80px\">Cataloging</th>");
            Response.Output.WriteLine("    <th style=\"width:80px\">Is Admin</th>");
            Response.Output.WriteLine("  </tr>");

            foreach (DataRow thisRow in allUsers.Rows)
            {
                // Get some user flags
                bool disabled = bool.Parse(thisRow["Disabled"].ToString());
                bool pendingApproval = bool.Parse(thisRow["PendingApproval"].ToString());
                string row_css_class = "user_row";
                if (disabled) row_css_class = "user_disabled_row";
                else if (pendingApproval) row_css_class = "user_pending_row";

                // Get the userid and then the edit link
                string userid = thisRow["UserID"].ToString();
                string edit_link = "UserSingleMgmt.aspx?id=" + userid;

                // Get the reset javascript
                string reset_js = "<a title=\"Click to reset the password\" href=\"javascript:reset_password('" + userid + "','" + thisRow["DisplayName"] + "');\">";

                Response.Output.WriteLine("  <tr class=\"" + row_css_class + "\">");
                Response.Output.WriteLine("    <td> &nbsp; (<a href=\"" + edit_link + "\" title=\"Edit or change permissions for this user\">edit</a>|" + reset_js + "reset</a>)</td>");
                Response.Output.WriteLine("    <td>" + thisRow["Username"] + "</td>");
                Response.Output.WriteLine("    <td>" + thisRow["DisplayName"] + "</td>");
                Response.Output.WriteLine("    <td>" + thisRow["LocationCode"] + "</td>");

                // Is this disabled?
                if (disabled)
                {
                    Response.Output.WriteLine("    <td colspan=\"4\" style=\"text-align:center\">Disabled User</td>");
                }
                else if (pendingApproval)
                {
                    Response.Output.WriteLine("    <td colspan=\"4\" style=\"text-align:center\">Pending Approval</td>");
                }
                else
                {
                    if ( bool.Parse(thisRow["CanQC"].ToString()))
                        Response.Output.WriteLine("    <td style=\"padding-left:21px;\"><input type=\"checkbox\" checked=\"checked\" onclick=\"return false;\"/></td>");
                    else
                        Response.Output.WriteLine("    <td style=\"padding-left:21px;\"><input type=\"checkbox\" onclick=\"return false;\"/></td>");

                    if (bool.Parse(thisRow["CanRunReports"].ToString()))
                        Response.Output.WriteLine("    <td style=\"padding-left:21px;\"><input type=\"checkbox\" checked=\"checked\" onclick=\"return false;\"/></td>");
                    else
                        Response.Output.WriteLine("    <td style=\"padding-left:21px;\"><input type=\"checkbox\" onclick=\"return false;\"/></td>");

                    if (bool.Parse(thisRow["CatalogingSpecialist"].ToString()))
                        Response.Output.WriteLine("    <td style=\"padding-left:21px;\"><input type=\"checkbox\" checked=\"checked\" onclick=\"return false;\"/></td>");
                    else
                        Response.Output.WriteLine("    <td style=\"padding-left:21px;\"><input type=\"checkbox\" onclick=\"return false;\"/></td>");


                    if (bool.Parse(thisRow["IsSystemAdmin"].ToString()))
                        Response.Output.WriteLine("    <td style=\"padding-left:21px;\"><input type=\"checkbox\" checked=\"checked\" onclick=\"return false;\"/></td>");
                    else
                        Response.Output.WriteLine("    <td style=\"padding-left:21px;\"><input type=\"checkbox\" onclick=\"return false;\"/></td>");
                }
                Response.Output.WriteLine("  </tr>");
            }


            Response.Output.WriteLine("</table>");
        }
    }
}