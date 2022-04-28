using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using FlareWorks.Library.Database;
using FlareWorks.Library.Models.QC;
using FlareWorks.MemoryMgmt;
using FlareWorks.Models.ControlledValues;
using FlareWorks.Models.Users;
using FlareWorks.Models.Work;
using FlareworksWeb.UserMgmt;

namespace FlareworksWeb
{
    public partial class SingleQc : Page
    {
        private UserInfo currentUser;
        private TitleInfo titleInfo;
        private WorkSet setInfo;

        private List<CheckBox> errorTypeButtons;
        private CheckBox otherErrorCheckBox;
        private TextBox otherErrorTextBox;

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

            // If no QC rights, then send back to the default
            if (!currentUser.Permissions.CanQC)
            {
                Response.Redirect("Default.aspx");
                return;
            }

            // Is there a bibnumber in the URL?
            int setid;
            if (!String.IsNullOrEmpty(Request.QueryString["setid"]))
            {
                string setid_string = Request.QueryString["setid"];
                if (Int32.TryParse(setid_string, out setid))
                {
                    // Get the workset
                    setInfo = DatabaseGateway.Get_Workset(setid);

                    if (setInfo == null)
                    {
                        Response.Redirect("QcDashboard.aspx");
                        return;
                    }

                    // Get the title info
                    TitleInfo requestedTitle = DatabaseGateway.Get_Title(setInfo.TitleID, String.Empty, String.Empty, currentUser.PrimaryKey);
                    if (requestedTitle == null)
                    {
                        Response.Redirect("QcDashboard.aspx");
                        return;
                    }
                    else
                    {
                        titleInfo = requestedTitle;
                    }
                }
                else
                {
                    Response.Redirect("QcDashboard.aspx");
                    return;
                }
            }
            else
            {
                Response.Redirect("QcDashboard.aspx");
                return;
            }

            // Show the basic information
            if (!String.IsNullOrEmpty(titleInfo.AlephNum))
                bibNumberLabel.Text = titleInfo.AlephNum;
            else
                BibNumTableRow.Visible = false;
            if (!String.IsNullOrEmpty(titleInfo.OCLC))
                oclcLabel.Text = titleInfo.OCLC;
            else
                OclcTableRow.Visible = false;

            typeLabel.Text = titleInfo.RecordType.Text;
            if (titleInfo.BibliographicLevel != null)
                bibLevelLabel.Text = titleInfo.BibliographicLevel.Level;
            else
                BibLevelTableRow.Visible = false;
            titleLabel.Text = titleInfo.Title;
            if (!String.IsNullOrEmpty(titleInfo.ISSN))
                issnLabel.Text = titleInfo.ISSN;
            else
                IssnTableRow.Visible = false;
            if (titleInfo.DocumentType != null)
                documentTypeLabel.Text = titleInfo.DocumentType.Text;
            else
                DocumentTypeTableRow.Visible = false;
            if (!String.IsNullOrEmpty(titleInfo.FederalAgency))
                federalAgencyLabel.Text = titleInfo.FederalAgency;
            else
                FedAgencyTableRow.Visible = false;

            if (setInfo.CatalogingType != null)
                catalogingLabel.Text = setInfo.CatalogingType.Text;

            if (!String.IsNullOrEmpty(titleInfo.GeneralNotes))
                titleNotesLabel.Text = titleInfo.GeneralNotes;
            else
                NotesTableRow.Visible = false;

            // Add the workset info
            workerLabel.Text = setInfo.Worker;
            projectLabel.Text = setInfo.Institution.Name;
            materialTypeLabel.Text = setInfo.MaterialType.Text;
            if (setInfo.LastCopy)
            {
                if (String.IsNullOrEmpty(setInfo.LastCopyInstitution))
                    lastCopyLabel.Text = "Last copy";
                else
                    lastCopyLabel.Text = setInfo.LastCopyInstitution;
            }
            else
            {
                lastCopyRow.Visible = false;
            }



            // Add the error types options
            errorTypeButtons = new List<CheckBox>();
            ErrorTypeInfo otherErrorType = null;
            int count = 0;
            foreach (ErrorTypeInfo errorType in ApplicationCache.ErrorTypes)
            {
                // Don't add the OTHER yet
                if (String.Equals(errorType.Text, "other", StringComparison.OrdinalIgnoreCase))
                {
                    otherErrorType = errorType;
                    continue;
                }

                // Time for a new line?
                if ((count > 0) && (count % 4 == 0))
                {
                    Literal newLine = new Literal { Text = "<br />" };
                    ErrorTypesPlaceHolder.Controls.Add(newLine);
                }

                CheckBox errorBox = new CheckBox();
                errorBox.ID = "Error" + errorType.ID;
                errorBox.Text = errorType.Text;
                errorBox.ToolTip = errorType.Description;
                errorBox.Width = new Unit("170px");
                errorBox.Style.Add("height", "30px;");

                ErrorTypesPlaceHolder.Controls.Add(errorBox);

                errorTypeButtons.Add(errorBox);

                count++;
            }

            if (otherErrorType != null)
            {
                Literal newLine3 = new Literal {Text = "<br />"};
                ErrorTypesPlaceHolder.Controls.Add(newLine3);

                // Add the OTHER option (with textbox)
                otherErrorCheckBox = new CheckBox();
                otherErrorCheckBox.ID = "Error" + otherErrorType.ID;
                otherErrorCheckBox.Text = "Other";
                otherErrorCheckBox.ToolTip = otherErrorType.Description;
                //  otherCleanUpCheckBox.Width = new Unit("200px");
                otherErrorCheckBox.Style.Add("height", "30px;");
                ErrorTypesPlaceHolder.Controls.Add(otherErrorCheckBox);
                errorTypeButtons.Add(otherErrorCheckBox);

                // Add the OTHER text box
                otherErrorTextBox = new TextBox();
                otherErrorTextBox.Width = new Unit("420px");
                otherErrorTextBox.Style.Add("margin-left", "10px;");
                otherErrorTextBox.ID = "OtherCleanUp";
                ErrorTypesPlaceHolder.Controls.Add(otherErrorTextBox);
            }

            show_itemwork();

            // SHow the processor notes
            if (!String.IsNullOrEmpty(setInfo.ProcessorNotes))
                processorNotesLabel.Text = setInfo.ProcessorNotes;
            else
                ProcessorNotesTableRow.Visible = false;

            // If this is not a post back, show the editable information
            if (!IsPostBack)
            {
                // Set the QC error
                QcNotesTextBox.Text = setInfo.QcNotes;

                // Set the error types
                Dictionary<int, CheckBox> errorControlDictionary = new Dictionary<int, CheckBox>();
                foreach (CheckBox thisError in errorTypeButtons)
                {
                    // This doesn't apply to the other error checkbox
                    if (thisError == otherErrorCheckBox) continue;

                    // Get the ID
                    int error_id = int.Parse(thisError.ID.Replace("Error", ""));

                    // Add to the dictionary
                    errorControlDictionary[error_id] = thisError;
                }

                // Now, step through errors
                string other_desc = String.Empty;
                foreach (WorkSet_Error thisError in setInfo.Errors)
                {
                    // Check this existing error
                    if (errorControlDictionary.ContainsKey(thisError.ErrorTypeID))
                        errorControlDictionary[thisError.ErrorTypeID].Checked = true;

                    // Get the other description
                    if (!String.IsNullOrEmpty(thisError.OtherDesription))
                        other_desc = thisError.OtherDesription;
                }

                // Was an other error listed?
                if (!String.IsNullOrEmpty(other_desc))
                {
                    otherErrorCheckBox.Checked = true;
                    otherErrorTextBox.Text = other_desc;
                }
            }

        }

        private void show_itemwork()
        {
            ItemInfoPlaceHolder.Controls.Clear();

            if ((titleInfo == null) || (titleInfo.Items.Count == 0))
            {
                return;
            }

            StringBuilder builder = new StringBuilder("<table id=\"item_work_table_brief\"><tr><th style=\"text-align:left; padding-left: 10px;\">Date</th><th>Worker</th><th>Trayed</th><th>Withdrawn/<br />Dupes</th><th>Damaged</th></tr>");

            int trayed = 0;
            int withdrawn = 0;
            int damaged = 0;

            foreach (WorkSetItem thisWork in setInfo.Items)
            {
                // Set color based on status
                string background_color = "#FFFFFF";

                builder.AppendLine("<tr style=\"background-color: " + background_color + ";\">");

                builder.AppendLine("  <td style=\"text-align:left; padding-left: 10px;\">" + thisWork.DateAdded.ToShortDateString() + "</td><td>" + setInfo.Worker + "</td>");
                builder.AppendLine("  <td>" + thisWork.ItemsSentToTray + "</td><td>" + thisWork.ItemsWithdrawn + "</td><td>" + thisWork.ItemsDamaged + "</td>");
                builder.AppendLine("</tr>");

                trayed += thisWork.ItemsSentToTray;
                withdrawn += thisWork.ItemsWithdrawn;
                damaged += thisWork.ItemsDamaged;
            }

            // Add a TOTAL row
            builder.AppendLine("<tr id=\"item_work_table_totalrow\"><td style=\"text-align:left; padding-left: 10px;\" colspan=\"2\">TOTAL</td><td>" + trayed + "</td><td>" + withdrawn + "</td><td>" + damaged + "</td></tr>");
            builder.AppendLine("</table>");

            // Show the existing items
            LiteralControl literal = new LiteralControl();
            literal.Text = builder.ToString();
            ItemInfoPlaceHolder.Controls.Add(literal);
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("QcDashboard.aspx");
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            // Get the QC notes
            string qc_notes = QcNotesTextBox.Text;

            // Get the other description
            string other_desc = otherErrorTextBox.Text;
            if (!otherErrorCheckBox.Checked) other_desc = String.Empty;

            // Build the list of errors
            List<int> errors_selected = new List<int>();
            foreach (CheckBox errorBox in errorTypeButtons)
            {
                // This doesn't apply to the other error checkbox
                if (errorBox == otherErrorCheckBox) continue;

                // Was this checked?
                if (errorBox.Checked)
                {
                    // Get the ID
                    int error_id = int.Parse(errorBox.ID.Replace("Error", ""));

                    // Add to error ist
                    errors_selected.Add(error_id);
                }
            }

            // Send to database
            bool result = DatabaseGateway.Save_Workset_QC(setInfo.PrimaryKey, qc_notes, currentUser.PrimaryKey, errors_selected, other_desc);
            if (result)
            {
                Response.Redirect("QcDashboard.aspx");
            }

        }

        protected void RejectButton_Click(object sender, EventArgs e)
        {
            // Get the QC notes
            string qc_notes = QcNotesTextBox.Text;

            // Get the other description
            string other_desc = otherErrorTextBox.Text;
            if (!otherErrorCheckBox.Checked) other_desc = String.Empty;

            // Build the list of errors
            List<int> errors_selected = new List<int>();
            foreach (CheckBox errorBox in errorTypeButtons)
            {
                // This doesn't apply to the other error checkbox
                if (errorBox == otherErrorCheckBox) continue;

                // Was this checked?
                if (errorBox.Checked)
                {
                    // Get the ID
                    int error_id = int.Parse(errorBox.ID.Replace("Error", ""));

                    // Add to error ist
                    errors_selected.Add(error_id);
                }
            }

            // Send to database
            bool result = DatabaseGateway.Reject_Workset(setInfo.PrimaryKey, qc_notes, currentUser.PrimaryKey, errors_selected, other_desc);
            if (result)
            {
                Response.Redirect("QcDashboard.aspx");
            }
        }

        protected void ApproveButton_Click(object sender, EventArgs e)
        {
            // Get the QC notes
            string qc_notes = QcNotesTextBox.Text;

            // Send to database
            bool result = DatabaseGateway.Approve_Workset(setInfo.PrimaryKey, qc_notes, currentUser.PrimaryKey);
            if (result)
            {
                Response.Redirect("QcDashboard.aspx");
            }
        }
    }
}