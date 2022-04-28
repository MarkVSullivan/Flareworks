using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FlareWorks.Library.Database;
using FlareWorks.MemoryMgmt;
using FlareWorks.Models.ControlledValues;
using FlareWorks.Models.Users;
using FlareworksWeb.UserMgmt;

namespace FlareworksWeb
{
    public partial class Default : System.Web.UI.Page
    {
        private UserInfo currentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Look for a user in the session
            currentUser = UserChecks.Check_Current_User();
            if (currentUser == null)
            {
                Response.Redirect("UserMgmt/Login.aspx");
                return;
            }

            // Check for permissions
            if ((currentUser.PendingApproval) || (currentUser.Disabled))
            {
                Response.Redirect("UserMgmt/NoPermissions.aspx");
                return;
            }

            // If not postback, show the user dashboard data
            if (!IsPostBack)
            {
                DataSet dashboardData = DatabaseGateway.Get_User_Dashboard_Data(currentUser.DisplayName);

                // If some data, show it
                if (dashboardData != null)
                {
                    StringBuilder builder = new StringBuilder();

                    if (dashboardData.Tables[0].Rows.Count > 0)
                    {
                        builder.AppendLine("<fieldset><legend><strong>Items in Process</strong> ( Items saved, but not submitted )</legend>");
                        builder.AppendLine("<table id=\"dashboard_item_table\"><tr><th style=\"text-align:left; padding-left: 10px;\">Aleph</th><th>OCLC</th><th>Title</th><th>Institution</th><th>Material Type</th><th>Last Updated</th><th># Processed</th></tr>");

                        show_in_process_items(dashboardData.Tables[0], builder);

                        builder.AppendLine("</table>");
                        builder.AppendLine("</fieldset>");
                    }

                    if (dashboardData.Tables[1].Rows.Count > 0)
                    {
                        builder.AppendLine("<fieldset><legend><strong>Rejected Items</strong> ( Items rejected, pending your corrections )</legend>");
                        builder.AppendLine("<table id=\"dashboard_item_table\"><tr><th style=\"text-align:left; padding-left: 10px;\">Date Rejected</th><th>Date Submitted</th><th>Aleph</th><th>OCLC</th><th>Title</th><th>Institution</th><th>Material Type</th><th># Processed</th></tr>");

                        show_rejected_items(dashboardData.Tables[1], builder);

                        builder.AppendLine("</table>");
                        builder.AppendLine("</fieldset>");
                    }

                    LiteralControl literal = new LiteralControl();
                    literal.Text = builder.ToString();
                    UserDashboardData.Controls.Add(literal);
                }

            }

           
        }

        private void show_in_process_items(DataTable itemTable, StringBuilder builder)
        {
            // Get the column names for the items pending QC
            DataColumn pq_AlephColumn = itemTable.Columns["AlephNum"];
            DataColumn pq_IssnColumn = itemTable.Columns["ISSN"];
            DataColumn pq_InstitutionColumn = itemTable.Columns["InstitutionCode"];
            DataColumn pq_MaterialColumn = itemTable.Columns["MaterialType"];
            DataColumn pq_CountColumn = itemTable.Columns["ItemCount"];
            DataColumn pq_DateColumn = itemTable.Columns["LastUpdated"];
            DataColumn pq_TitleColumn = itemTable.Columns["Title"];
            DataColumn pq_OclcColumn = itemTable.Columns["OCLC"];
            DataColumn pq_IdColumn = itemTable.Columns["TitleID"];

            // Will need to pull all the data to display here
            int last_titleid = -1;
            string issn = String.Empty;
            string alephNum = String.Empty;
            string oclc = String.Empty;
            SortedList<string, string> institutions = new SortedList<string, string>();
            SortedList<string, string> materials = new SortedList<string, string>();
            int itemCount = 0;
            DateTime lastUpdated = DateTime.Now;
            int setid = -1;
            string title = String.Empty;
            
            DateTime latUpdated;

            // Values used when displaying 
            bool first_institution;
            bool first_material;

            // Step through each row
            foreach (DataRow thisRow in itemTable.Rows)
            {
                // Get this aleph
                int titleid = Int32.Parse(thisRow[pq_IdColumn].ToString());

                // Is this a new title?  In which case display the last
                if (titleid != last_titleid)
                {
                    // Show the dataa, if there was a last title
                    if (last_titleid > 0)
                    {
                        // Show the data
                        builder.AppendLine("  <tr onclick=\"window.location='TitleEntry.aspx?id=" + last_titleid + "'\">");

                        builder.AppendLine("    <td>" + alephNum + "</td>");
                        builder.AppendLine("    <td>" + oclc + "</td>");
                        builder.AppendLine("    <td>" + title + "</td>");


                        // Show all the institution
                        builder.Append("    <td>");
                        first_institution = true;
                        for (int i = 0; i < institutions.Count; i++)
                        {
                            if (!first_institution)
                                builder.Append("<br />");
                            else
                                first_institution = false;

                            builder.Append(institutions.Values[i]);
                        }
                        builder.AppendLine("</td>");

                        // Show all the material types
                        builder.Append("    <td>");
                        first_material = true;
                        for (int i = 0; i < materials.Count; i++)
                        {
                            if (!first_material)
                                builder.Append("<br />");
                            else
                                first_material = false;

                            builder.Append(materials.Values[i]);
                        }
                        builder.AppendLine("</td>");

                        // Show the last updated date and item count
                        builder.AppendLine("    <td>" + lastUpdated.ToShortDateString() + "</td>");
                        builder.AppendLine("    <td>" + itemCount + "</td>");
                        builder.AppendLine("  </tr>");
                    }

                    // Clear the values
                    institutions.Clear();
                    materials.Clear();
                    itemCount = 0;

                    // Now we are onto this row
                    last_titleid = titleid;

                    // Set some simple values from this row
                    lastUpdated = DateTime.Parse(thisRow[pq_DateColumn].ToString());
                    issn = thisRow[pq_IssnColumn].ToString();
                    title = thisRow[pq_TitleColumn].ToString();
                    alephNum = (thisRow[pq_AlephColumn] != DBNull.Value) ? thisRow[pq_AlephColumn].ToString() : String.Empty;
                    oclc = (thisRow[pq_OclcColumn] != DBNull.Value) ? thisRow[pq_OclcColumn].ToString() : String.Empty;
                }


                // Add this institutiion to the list, if not there
                string thisInstitution = thisRow[pq_InstitutionColumn].ToString();
                if (!institutions.ContainsKey(thisInstitution.ToUpper()))
                {
                    institutions.Add(thisInstitution.ToUpper(), thisInstitution);
                }

                // Add this material type to the list, if not there
                string thisMaterial = thisRow[pq_MaterialColumn].ToString();
                if (!materials.ContainsKey(thisMaterial.ToUpper()))
                {
                    materials.Add(thisMaterial.ToUpper(), thisMaterial);
                }

                // Add to the item count
                itemCount += int.Parse(thisRow[pq_CountColumn].ToString());

            }

            // Show the LAST data
            builder.AppendLine("  <tr onclick=\"window.location='TitleEntry.aspx?id=" + last_titleid + "'\">");

            builder.AppendLine("    <td>" + alephNum + "</td>");
            builder.AppendLine("    <td>" + oclc + "</td>");
            builder.AppendLine("    <td>" + title + "</td>");

            // Show all the institution
            builder.Append("    <td>");
            first_institution = true;
            for (int i = 0; i < institutions.Count; i++)
            {
                if (!first_institution)
                    builder.Append(" / ");
                else
                    first_institution = false;

                builder.Append(institutions.Values[i]);
            }
            builder.AppendLine("</td>");

            // Show all the material types
            builder.Append("    <td>");
            first_material = true;
            for (int i = 0; i < materials.Count; i++)
            {
                if (!first_material)
                    builder.Append("<br />");
                else
                    first_material = false;

                builder.Append(materials.Values[i]);
            }
            builder.AppendLine("</td>");

            // Show the last updated date and item count
            builder.AppendLine("    <td>" + lastUpdated.ToShortDateString() + "</td>");
            builder.AppendLine("    <td>" + itemCount + "</td>");
            builder.AppendLine("  </tr>");
        }

        private void show_rejected_items(DataTable itemTable, StringBuilder builder)
        {
            // Get the column names for the items pending QC
            DataColumn pq_SetColumn = itemTable.Columns["ItemWorkSetID"];
            DataColumn pq_AlephColumn = itemTable.Columns["AlephNum"];
            DataColumn pq_IssnColumn = itemTable.Columns["ISSN"];
            DataColumn pq_InstitutionColumn = itemTable.Columns["InstitutionCode"];
            DataColumn pq_MaterialColumn = itemTable.Columns["MaterialType"];
            DataColumn pq_CountColumn = itemTable.Columns["ItemCount"];
            DataColumn pq_DateColumn = itemTable.Columns["DateRejected"];
            DataColumn pq_SubmittedColumn = itemTable.Columns["DateSubmitted"];
            DataColumn pq_TitleColumn = itemTable.Columns["Title"];
            DataColumn pq_OclcColumn = itemTable.Columns["OCLC"];
            DataColumn pq_IdColumn = itemTable.Columns["TitleID"];

            // Will need to pull all the data to display here
            int last_titleid = -1;
            string issn = String.Empty;
            string alephNum = String.Empty;
            string oclc = String.Empty;
            SortedList<string, string> institutions = new SortedList<string, string>();
            SortedList<string, string> materials = new SortedList<string, string>();
            int itemCount = 0;
            DateTime dateRejected = DateTime.Now;
            int setid = -1;
            int last_setid = -1;
            string title = String.Empty;
            DateTime dateSubmitted = DateTime.Now;


            // Values used when displaying 
            bool first_institution;
            bool first_material;

            // Step through each row
            foreach (DataRow thisRow in itemTable.Rows)
            {
                // Get this aleph
                setid = int.Parse(thisRow[pq_SetColumn].ToString());

                // Is this a new ALEPH?  In which case display the last
                if (last_setid != setid)
                {
                    // Show the dataa, if there was a last aleph number
                    if (last_setid > 0)
                    {
                        // Show the data
                        builder.AppendLine("  <tr onclick=\"window.location='Correct.aspx?setid=" + last_setid + "'\">");
                        builder.AppendLine("    <td>" + dateRejected.ToShortDateString() + "</td>");
                        builder.AppendLine("    <td>" + dateSubmitted.ToShortDateString() + "</td>");
                        builder.AppendLine("    <td>" + alephNum + "</td>");
                        builder.AppendLine("    <td>" + oclc + "</td>");
                        builder.AppendLine("    <td>" + title + "</td>");


                        // Show all the institution
                        builder.Append("    <td>");
                        first_institution = true;
                        for (int i = 0; i < institutions.Count; i++)
                        {
                            if (!first_institution)
                                builder.Append("<br />");
                            else
                                first_institution = false;

                            builder.Append(institutions.Values[i]);
                        }
                        builder.AppendLine("</td>");

                        // Show all the material types
                        builder.Append("    <td>");
                        first_material = true;
                        for (int i = 0; i < materials.Count; i++)
                        {
                            if (!first_material)
                                builder.Append("<br />");
                            else
                                first_material = false;

                            builder.Append(materials.Values[i]);
                        }
                        builder.AppendLine("</td>");

                        // Show the last updated date and item count

                        builder.AppendLine("    <td>" + itemCount + "</td>");
                        builder.AppendLine("  </tr>");
                    }

                    // Clear the values
                    institutions.Clear();
                    materials.Clear();
                    itemCount = 0;

                    // Now we are onto this row
                    last_setid = setid;

                    // Set some simple values from this row
                    dateRejected = DateTime.Parse(thisRow[pq_DateColumn].ToString());
                    dateSubmitted = DateTime.Parse(thisRow[pq_SubmittedColumn].ToString());
                    issn = thisRow[pq_IssnColumn].ToString();
                    title = thisRow[pq_TitleColumn].ToString();
                    oclc = thisRow[pq_OclcColumn].ToString();
                    alephNum = thisRow[pq_AlephColumn].ToString();
                }


                // Add this institutiion to the list, if not there
                string thisInstitution = thisRow[pq_InstitutionColumn].ToString();
                if (!institutions.ContainsKey(thisInstitution.ToUpper()))
                {
                    institutions.Add(thisInstitution.ToUpper(), thisInstitution);
                }

                // Add this material type to the list, if not there
                string thisMaterial = thisRow[pq_MaterialColumn].ToString();
                if (!materials.ContainsKey(thisMaterial.ToUpper()))
                {
                    materials.Add(thisMaterial.ToUpper(), thisMaterial);
                }

                // Add to the item count
                itemCount += int.Parse(thisRow[pq_CountColumn].ToString());

            }

            // Show the LAST data
            builder.AppendLine("  <tr onclick=\"window.location='Correct.aspx?setid=" + last_setid + "'\">");
            builder.AppendLine("    <td>" + dateRejected.ToShortDateString() + "</td>");
            builder.AppendLine("    <td>" + dateSubmitted.ToShortDateString() + "</td>");
            builder.AppendLine("    <td>" + alephNum + "</td>");
            builder.AppendLine("    <td>" + oclc + "</td>");
            builder.AppendLine("    <td>" + title + "</td>");

            // Show all the institution
            builder.Append("    <td>");
            first_institution = true;
            for (int i = 0; i < institutions.Count; i++)
            {
                if (!first_institution)
                    builder.Append(" / ");
                else
                    first_institution = false;

                builder.Append(institutions.Values[i]);
            }
            builder.AppendLine("</td>");

            // Show all the material types
            builder.Append("    <td>");
            first_material = true;
            for (int i = 0; i < materials.Count; i++)
            {
                if (!first_material)
                    builder.Append("<br />");
                else
                    first_material = false;

                builder.Append(materials.Values[i]);
            }
            builder.AppendLine("</td>");

            // Show the last updated date and item count
            builder.AppendLine("    <td>" + itemCount + "</td>");
            builder.AppendLine("  </tr>");
        }

        protected void AddItemButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("TitleEntry.aspx");
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            if (currentUser.Permissions.CanAdvancedSearch)
                Response.Redirect("AdminSearch.aspx");
            else
                Response.Redirect("Search.aspx");
        }
    }
}