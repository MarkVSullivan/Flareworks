using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FlareWorks.Library.Database;
using FlareWorks.Library.Models.ControlledValues;
using FlareWorks.MemoryMgmt;
using FlareWorks.Models.ControlledValues;
using FlareWorks.Models.Users;
using FlareworksWeb.UserMgmt;

namespace FlareworksWeb
{
    public partial class QcDashboard : System.Web.UI.Page
    {
        private UserInfo currentUser;
        private DataSet qcDashboardData;
        private List<CheckBox> pendingCheckBoxes; 

        protected void Page_Init(object sender, EventArgs e)
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

            // If no QC rights, then send back to the default
            if (!currentUser.Permissions.CanQC)
            {
                Response.Redirect("Default.aspx");
                return;
            }


        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Add filter info, if this is not a postback
            if (FilterLocationsDropDown.Items.Count == 0)
            {
                FilterLocationsDropDown.Items.Add("");
                foreach (LocationInfo location in ApplicationCache.Locations)
                {
                    FilterLocationsDropDown.Items.Add(location.Code.ToUpper());
                }
            }

            if (FilterUsersDropDown.Items.Count == 0)
            {
                FilterUsersDropDown.Items.Add("");
                foreach (BriefWorkerInfo user in ApplicationCache.Workers)
                {
                    FilterUsersDropDown.Items.Add(user.Name);
                }
            }

            // If this is not a postback, set to the users last filters
            if (!IsPostBack)
            {
                if (currentUser.Recents.LastQcLocationSelected.Length > 0)
                    FilterLocationsDropDown.Text = currentUser.Recents.LastQcLocationSelected;

                if (currentUser.Recents.LastQcUserSelected.Length > 0)
                    FilterUsersDropDown.Text = currentUser.Recents.LastQcUserSelected;

            }


            // Pull the QC dashboard data
            try
            {
                string location = FilterLocationsDropDown.SelectedItem.Text;
                string user = FilterUsersDropDown.SelectedItem.Text;
                qcDashboardData = DatabaseGateway.Get_QC_Dashboard_Data(location, user, currentUser.PrimaryKey);
            }
            catch (Exception ee)
            {
                FormError.Text = "Exception caught while pulling the QC dashboard data.<br /><br />" + ee.Message;
                return;
            }



            show_qc_pending_data();

            show_qc_rejected_data();
        }

        private void show_qc_pending_data()
        {
            pendingCheckBoxes = new List<CheckBox>();

            // Are the items pending QC?
            if (qcDashboardData.Tables[0].Rows.Count == 0 )
            {
                // Show message for no pending items
                Literal noItemsLiteral = new Literal();
                noItemsLiteral.Text = "<br /><br /> &nbsp; &nbsp; &nbsp; Nicely done!  There are no items pending quality control at this time.<br /><br />";
                PendingQcItems.Controls.Add(noItemsLiteral);

                // Hide the able
                PendingQcTable.Visible = false;

                // Can hide the buttos now
                SelectAllButton.Visible = false;
                ApproveAllButton.Visible = false;
                return;
            }
            else
            {
                PendingQcItems.Controls.Clear();
                PendingQcTable.Visible = true;
                SelectAllButton.Visible = true;
                ApproveAllButton.Visible = true;
            }

            // Add the header row
            TableHeaderRow hdrRow = new TableHeaderRow();
            hdrRow.Cells.Add(new TableHeaderCell() { Text = "Approved" });
            hdrRow.Cells.Add(new TableHeaderCell() { Text = "Rejected" });
            hdrRow.Cells.Add(new TableHeaderCell() { Text = "Bib Number" });
            hdrRow.Cells.Add(new TableHeaderCell() { Text = "OCLC" });
            hdrRow.Cells.Add(new TableHeaderCell() { Text = "Worker" });
            hdrRow.Cells.Add(new TableHeaderCell() { Text = "Institution" });
            hdrRow.Cells.Add(new TableHeaderCell() { Text = "Material Type" });
            hdrRow.Cells.Add(new TableHeaderCell() { Text = "Last Updated" });
            hdrRow.Cells.Add(new TableHeaderCell() { Text = "# Processed" });            
            PendingQcTable.Rows.Add(hdrRow);

            // Get the column names for the items pending QC
            DataTable pqTable = qcDashboardData.Tables[0];
            DataColumn pq_PrimaryKey = pqTable.Columns["ItemWorkSetID"];
            DataColumn pq_AlephColumn = pqTable.Columns["AlephNum"];
            DataColumn pq_OclcColumn = pqTable.Columns["OCLC"];
            DataColumn pq_WorkersColumn = pqTable.Columns["WorkerName"];
            DataColumn pq_InstitutionColumn = pqTable.Columns["InstitutionCode"];
            DataColumn pq_MaterialColumn = pqTable.Columns["MaterialType"];
            DataColumn pq_CountColumn = pqTable.Columns["ItemCount"];
            DataColumn pq_SubmittedColumn = pqTable.Columns["DateSubmitted"];
            DataColumn pq_RejectedColumn = pqTable.Columns["DateRejected"];


            // Values used when displaying 
            int setid = -1;
            string previously_rejected = String.Empty;

            // Step through each row
            foreach (DataRow thisRow in pqTable.Rows)
            {
                // Get the set id
                setid = Int32.Parse(thisRow[pq_PrimaryKey].ToString());

                // Create this row
                TableRow rw = new TableRow();
                
                // Checkbox cell
                TableCell cell = new TableCell();
                CheckBox box = new CheckBox();
                box.ID = "ChkBox" + setid;
                cell.Controls.Add(box);
                pendingCheckBoxes.Add(box);
                rw.Cells.Add(cell);

                // Previously rejected
                previously_rejected = String.Empty;
                DateTime tempRejectDate = DateTime.Parse(thisRow[pq_RejectedColumn].ToString());
                if (tempRejectDate.Year > 1900)
                    previously_rejected = tempRejectDate.ToShortDateString();
                TableCell prevCell = new TableCell();
                prevCell.Text = previously_rejected;
                prevCell.Attributes.Add("onclick", "window.location='SingleQc.aspx?setid=" + setid + "'");
                rw.Cells.Add(prevCell);

                // ALEPH number
                TableCell alephCell = new TableCell();
                alephCell.Text = thisRow[pq_AlephColumn].ToString();
                alephCell.Attributes.Add("onclick", "window.location='SingleQc.aspx?setid=" + setid + "'");
                rw.Cells.Add(alephCell);

                // OCLC number
                TableCell oclcCell = new TableCell();
                oclcCell.Text = thisRow[pq_OclcColumn].ToString();
                oclcCell.Attributes.Add("onclick", "window.location='SingleQc.aspx?setid=" + setid + "'");
                rw.Cells.Add(oclcCell);

                // Worker
                TableCell workerCell = new TableCell();
                workerCell.Text = thisRow[pq_WorkersColumn].ToString();
                workerCell.Attributes.Add("onclick", "window.location='SingleQc.aspx?setid=" + setid + "'");
                rw.Cells.Add(workerCell);

                // Institution
                TableCell institutionCell = new TableCell();
                institutionCell.Text = thisRow[pq_InstitutionColumn].ToString().ToUpper();
                institutionCell.Attributes.Add("onclick", "window.location='SingleQc.aspx?setid=" + setid + "'");
                rw.Cells.Add(institutionCell);

                // Material Type
                TableCell materialCell = new TableCell();
                materialCell.Text = thisRow[pq_MaterialColumn].ToString();
                materialCell.Attributes.Add("onclick", "window.location='SingleQc.aspx?setid=" + setid + "'");
                rw.Cells.Add(materialCell);

                // Last updated
                TableCell updatedCell = new TableCell();
                updatedCell.Text = DateTime.Parse(thisRow[pq_SubmittedColumn].ToString()).ToShortDateString();
                updatedCell.Attributes.Add("onclick", "window.location='SingleQc.aspx?setid=" + setid + "'");
                rw.Cells.Add(updatedCell);

                // Item count
                TableCell itemCountCell = new TableCell();
                itemCountCell.Text = thisRow[pq_CountColumn].ToString();
                itemCountCell.Attributes.Add("onclick", "window.location='SingleQc.aspx?setid=" + setid + "'");
                rw.Cells.Add(itemCountCell);

                // Add this table
                PendingQcTable.Rows.Add(rw);
            } 
        }

        private void show_qc_rejected_data()
        {
            // Are the items pending QC?
            if (qcDashboardData.Tables[1].Rows.Count == 0)
            {
                // Show message for no pending items
                Literal noItemsLiteral = new Literal();
                noItemsLiteral.Text = "<br /><br /> &nbsp; &nbsp; &nbsp; There are no items pending worker correction at this time.<br /><br />";
                PendingCorrectionItems.Controls.Add(noItemsLiteral);
                return;
            }

            // Start the table now
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<table id=\"dashboard_item_table\"><tr><th>Rejected<br />Date</th><th>Bib Number</th><th>OCLC</th><th>Worker</th><th>Institution</th><th>Material Type</th><th>Last Updated</th><th># Processed</th></tr>");


            // Get the column names for the items pending QC
            DataTable pqTable = qcDashboardData.Tables[1];
            DataColumn pq_PrimaryKey = pqTable.Columns["ItemWorkSetID"];
            DataColumn pq_AlephColumn = pqTable.Columns["AlephNum"];
            DataColumn pq_OclcColumn = pqTable.Columns["OCLC"];
            DataColumn pq_WorkersColumn = pqTable.Columns["WorkerName"];
            DataColumn pq_InstitutionColumn = pqTable.Columns["InstitutionCode"];
            DataColumn pq_MaterialColumn = pqTable.Columns["MaterialType"];
            DataColumn pq_CountColumn = pqTable.Columns["ItemCount"];
            DataColumn pq_SubmittedColumn = pqTable.Columns["DateSubmitted"];
            DataColumn pq_RejectedColumn = pqTable.Columns["DateRejected"];

            // Will need to pull all the data to display here
            string lastAlephNum = String.Empty;
            string alephNum;
            string oclc = String.Empty;
            string worker = String.Empty;
            SortedList<string, string> institutions = new SortedList<string, string>();
            SortedList<string, string> materials = new SortedList<string, string>();
            int itemCount = 0;
            DateTime lastUpdated = DateTime.Now;
            int setid = -1;
            int last_setid = -1;

            // Values used when displaying 
            bool first_user;
            bool first_institution;
            bool first_material;
            string previously_rejected = String.Empty;

            // Step through each row
            foreach (DataRow thisRow in pqTable.Rows)
            {
                // Get the set id
                setid = Int32.Parse(thisRow[pq_PrimaryKey].ToString());

                // Get this aleph
                alephNum = thisRow[pq_AlephColumn].ToString();

                // Is this a new ALEPH?  In which case display the last
                if (setid != last_setid)
                {
                    // Show the dataa, if there was a last aleph number
                    if (last_setid > 0)
                    {
                        // Show the data
                        builder.AppendLine("  <tr onclick=\"window.location='SingleQc.aspx?setid=" + last_setid + "'\">");
 
                        builder.AppendLine("    <td>" + previously_rejected + "</td>");
                        builder.AppendLine("    <td>" + lastAlephNum + "</td>");
                        builder.AppendLine("    <td>" + oclc + "</td>");

                        // Show the worker
                        builder.Append("    <td>" + worker + "</td>");

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
                    oclc = String.Empty;
                    worker = String.Empty;
                    institutions.Clear();
                    materials.Clear();
                    itemCount = 0;
                    previously_rejected = String.Empty;

                    // Now we are onto this row
                    last_setid = setid;
                    lastAlephNum = alephNum;

                    // Set some simple values from this row
                    lastUpdated = DateTime.Parse(thisRow[pq_SubmittedColumn].ToString());
                    oclc = thisRow[pq_OclcColumn].ToString();
                    DateTime tempRejectDate = DateTime.Parse(thisRow[pq_RejectedColumn].ToString());
                    if (tempRejectDate.Year > 1900)
                        previously_rejected = tempRejectDate.ToShortDateString();
                }

                // Add this worker to the worker list, if not there
                worker = thisRow[pq_WorkersColumn].ToString();

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
            builder.AppendLine("  <tr onclick=\"window.location='SingleQc.aspx?setid=" + last_setid + "'\">");
            builder.AppendLine("    <td>" + previously_rejected + "</td>");
            builder.AppendLine("    <td>" + lastAlephNum + "</td>");
            builder.AppendLine("    <td>" + oclc + "</td>");

            // Show the worker
            builder.Append("    <td>" + worker + "</td>");

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


            // Close the table
            builder.AppendLine("</table>");

            // Add the table 
            Literal tableLiteral = new Literal();
            tableLiteral.Text = builder.ToString();
            PendingCorrectionItems.Controls.Add(tableLiteral);
        }

        protected void SelectAllButton_Click(object sender, EventArgs e)
        {
            foreach( CheckBox thisBox in pendingCheckBoxes)
            {
                thisBox.Checked = true;
            }
        }

        protected void ApproveAllButton_Click(object sender, EventArgs e)
        {
            foreach(CheckBox chkBox in pendingCheckBoxes )
            {
                if ( chkBox.Checked )
                {
                    string id_string = chkBox.ID.Replace("ChkBox", "");
                    int id = -1;
                    if (Int32.TryParse(id_string, out id))
                    {
                        bool result = DatabaseGateway.Approve_Workset(id, String.Empty, currentUser.PrimaryKey);
                        if ( !result )
                        {
                            return;
                        }
                    }
                }
            }


            // Pull the QC dashboard data
            try
            {
                string location = FilterLocationsDropDown.SelectedItem.Text;
                string user = FilterUsersDropDown.SelectedItem.Text;
                qcDashboardData = DatabaseGateway.Get_QC_Dashboard_Data(location, user, currentUser.PrimaryKey);
            }
            catch (Exception ee)
            {
                FormError.Text = "Exception caught while pulling the QC dashboard data.<br /><br />" + ee.Message;
                return;
            }

            // Show pending qc items
            PendingQcTable.Rows.Clear();
            show_qc_pending_data();
        }

        protected void FilterButton_Click(object sender, EventArgs e)
        {
            currentUser.Recents.LastQcLocationSelected = FilterLocationsDropDown.SelectedItem.Text; 
            currentUser.Recents.LastQcUserSelected = FilterUsersDropDown.SelectedItem.Text;
        }
    }
}