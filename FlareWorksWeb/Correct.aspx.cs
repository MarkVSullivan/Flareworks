using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
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
    public partial class Correct : System.Web.UI.Page
    {
        private UserInfo currentUser;
        private TitleInfo titleInfo;
        private WorkSet setInfo;

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
                        Response.Redirect("Default.aspx");
                        return;
                    }

                    // Get the title info
                    TitleInfo requestedTitle = DatabaseGateway.Get_Title(setInfo.TitleID, String.Empty, String.Empty, currentUser.PrimaryKey);
                    if (requestedTitle == null)
                    {
                        Response.Redirect("Default.aspx");
                        return;
                    }
                    else
                    {
                        titleInfo = requestedTitle;
                    }
                }
                else
                {
                    Response.Redirect("Default.aspx");
                    return;
                }
            }
            else
            {
                Response.Redirect("Default.aspx");
                return;
            }

            // Ensure this is the same user's work set
            if ( setInfo.Worker != currentUser.DisplayName )
            {
                Response.Redirect("Default.aspx");
                return;
            }

            // Show the basic information
            bibNumberLabel.Text = titleInfo.AlephNum;
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


            if ( !String.IsNullOrWhiteSpace(titleInfo.GeneralNotes))
                titleNotesLabel.Text = titleInfo.GeneralNotes;
            else
            {
                titleNotesLabel.Text = "(none)";
                titleNotesLabel.Font.Italic = true;
            }

            // Determine which record cleanups to show
            Dictionary<string, string> existingCleanups = new Dictionary<string, string>();
            foreach (TitleInfo_CleanUp cleanup in titleInfo.Tasks)
            {
                if (!cleanup.DateCleared.HasValue)
                {
                    existingCleanups[cleanup.CleanUpType] = cleanup.CleanUpType;
                }
            }

            // Add the record cleanup options
            CleanUpTypeInfo otherCleanUp = null;
            int count = 0;
            foreach (CleanUpTypeInfo cleanupType in ApplicationCache.CleanupTypes)
            {
                // Also, only add those that exist
                if (!existingCleanups.ContainsKey(cleanupType.Text))
                    continue;

                // Don't add the OTHER yet
                if (String.Equals(cleanupType.Text, "other", StringComparison.OrdinalIgnoreCase))
                {
                    otherCleanUp = cleanupType;
                    continue;
                }

                // Time for a new line?
                if ((count > 0) && (count % 4 == 0))
                {
                    Literal newLine = new Literal { Text = "<br />" };
                    CleanupAreaPlaceHolder.Controls.Add(newLine);
                }

                CheckBox cleanupBox = new CheckBox();
                cleanupBox.ID = "CleanUp" + cleanupType.ID;
                cleanupBox.Text = cleanupType.Text;
                cleanupBox.ToolTip = cleanupType.Description;
                cleanupBox.Width = new Unit("200px");
                cleanupBox.Style.Add("height", "30px;");
                cleanupBox.Checked = true;
                cleanupBox.Attributes.Add("onclick", "return false;");
                CleanupAreaPlaceHolder.Controls.Add(cleanupBox);

                count++;
            }

            if (otherCleanUp != null)
            {
                Literal newLine2 = new Literal { Text = "<br />" };
                CleanupAreaPlaceHolder.Controls.Add(newLine2);

                // Add the OTHER option (with textbox)
                CheckBox otherCleanUpCheckBox = new CheckBox();
                otherCleanUpCheckBox.ID = "CleanUp" + otherCleanUp.ID;
                otherCleanUpCheckBox.Text = "Other";
                otherCleanUpCheckBox.ToolTip = otherCleanUp.Description;
                //  otherCleanUpCheckBox.Width = new Unit("200px");
                otherCleanUpCheckBox.Style.Add("height", "30px;");
                otherCleanUpCheckBox.Attributes.Add("onclick", "return false;");
                CleanupAreaPlaceHolder.Controls.Add(otherCleanUpCheckBox);

                // Add the OTHER text box
                TextBox otherCleanUpTextBox = new TextBox();
                otherCleanUpTextBox.Width = new Unit("420px");
                otherCleanUpTextBox.Style.Add("margin-left", "10px;");
                otherCleanUpTextBox.ID = "OtherCleanUp";
                otherCleanUpTextBox.ReadOnly = true;
                CleanupAreaPlaceHolder.Controls.Add(otherCleanUpTextBox);

                count++;
            }

            // If no cleanup was added, show a message
            if (count == 0)
            {
                Literal noCleanup = new Literal();
                noCleanup.Text = "No record cleanups selected";
                CleanupAreaPlaceHolder.Controls.Add(noCleanup);
            }

            // Show the reject date and notes
            if (setInfo.DateRejected.HasValue)
            {
                RejectedDateLabel.Text = "Rejected by " + setInfo.QcWorker + " on " + setInfo.DateRejected.Value.ToShortDateString();
            }
            RejectNotesLabel.Text = setInfo.QcNotes;

            // Determine which QC erros to show
            Dictionary<string, string> existingErrors = new Dictionary<string, string>();
            foreach (WorkSet_Error error in setInfo.Errors)
            {
                existingCleanups[error.ErrorType] = error.ErrorType;
            }

            // Add the QC error options
            ErrorTypeInfo otherError = null;
            count = 0;
            foreach (ErrorTypeInfo errorType in ApplicationCache.ErrorTypes)
            {
                // Also, only add those that exist
                if (!existingCleanups.ContainsKey(errorType.Text))
                    continue;

                // Don't add the OTHER yet
                if (String.Equals(errorType.Text, "other", StringComparison.OrdinalIgnoreCase))
                {
                    otherError = errorType;
                    continue;
                }

                // Time for a new line?
                if ((count > 0) && (count % 4 == 0))
                {
                    Literal newLine = new Literal { Text = "<br />" };
                    ErrorPlaceHolder.Controls.Add(newLine);
                }

                CheckBox errorBox = new CheckBox();
                errorBox.ID = "Error" + errorType.ID;
                errorBox.Text = errorType.Text;
                errorBox.ToolTip = errorType.Description;
                errorBox.Width = new Unit("200px");
                errorBox.Style.Add("height", "30px;");
                errorBox.Checked = true;
                errorBox.Attributes.Add("onclick", "return false;");
                ErrorPlaceHolder.Controls.Add(errorBox);

                count++;
            }

            if (otherError != null)
            {
                Literal newLine2 = new Literal { Text = "<br />" };
                ErrorPlaceHolder.Controls.Add(newLine2);

                // Add the OTHER option (with textbox)
                CheckBox otherErrorCheckBox = new CheckBox();
                otherErrorCheckBox.ID = "CleanUp" + otherCleanUp.ID;
                otherErrorCheckBox.Text = "Other";
                otherErrorCheckBox.ToolTip = otherCleanUp.Description;
                //  otherCleanUpCheckBox.Width = new Unit("200px");
                otherErrorCheckBox.Style.Add("height", "30px;");
                otherErrorCheckBox.Attributes.Add("onclick", "return false;");
                ErrorPlaceHolder.Controls.Add(otherErrorCheckBox);

                // Add the OTHER text box
                TextBox otherErrorTextBox = new TextBox();
                otherErrorTextBox.Width = new Unit("420px");
                otherErrorTextBox.Style.Add("margin-left", "10px;");
                otherErrorTextBox.ID = "OtherCleanUp";
                otherErrorTextBox.ReadOnly = true;
                ErrorPlaceHolder.Controls.Add(otherErrorTextBox);

                count++;
            }

            // If no cleanup was added, show a message
            if (count == 0)
            {
                Literal noCleanup = new Literal();
                noCleanup.Text = "No errors selected";
                ErrorPlaceHolder.Controls.Add(noCleanup);
            }

            show_itemwork();

            // SHow the processor notes (which are editable)
            if ( !IsPostBack)
                NotesTextBox.Text = setInfo.ProcessorNotes; 
        }

        private void show_itemwork()
        {
            ItemInfoPlaceHolder.Controls.Clear();

            if ((titleInfo == null) || (titleInfo.Items.Count == 0))
            {
                return;
            }

            StringBuilder builder = new StringBuilder("<table id=\"item_work_table\"><tr><th style=\"text-align:left; padding-left: 10px;\">Date</th><th>Worker</th><th>Trayed</th><th>Withdrawn/<br />Dupes</th><th>Damaged</th></tr>");

            int trayed = 0;
            int withdrawn = 0;
            int damaged = 0;

            string processor = String.Empty;
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

        protected void EditTitleButton_OnClick(object Sender, EventArgs E)
        {
            Response.Redirect("TitleEdit.aspx?bibnumber=" + titleInfo.AlephNum + "&setid=" + setInfo.PrimaryKey);
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            // Save the notes
            string new_notes = NotesTextBox.Text;
            if ( DatabaseGateway.Save_Workset_Notes(setInfo.PrimaryKey, new_notes))
                Response.Redirect("Default.aspx");
        }

        protected void ResubmitButton_Click(object sender, EventArgs e)
        {
            // Resubmit this
            string new_notes = NotesTextBox.Text;
            if ( DatabaseGateway.Resubmit_Workset(setInfo.PrimaryKey, new_notes))
                Response.Redirect("Default.aspx");
        }
    }
}