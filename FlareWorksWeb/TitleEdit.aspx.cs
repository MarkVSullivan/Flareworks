using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FlareWorks.Library.Database;
using FlareWorks.Library.Models.ControlledValues;
using FlareWorks.MemoryMgmt;
using FlareWorks.Models.ControlledValues;
using FlareWorks.Models.Users;
using FlareWorks.Models.Work;
using FlareworksWeb.UserMgmt;

namespace FlareworksWeb
{
    public partial class TitleEdit : System.Web.UI.Page
    {
        private TitleInfo titleInfo;
        private UserInfo currentUser;
        private List<RadioButton> recordTypeButtons;
        private List<RadioButton> bibLevelButtons;
        private List<CheckBox> cleanUpButtons;
        private CheckBox otherCheckBox;
        private TextBox otherTextBox;

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

            if (!currentUser.Permissions.CanProcessItems)
            {
                Response.Redirect("Default.aspx");
                return;
            }

            // Is there a work set id in the URL?
            if (!String.IsNullOrEmpty(Request.QueryString["setid"]))
            {
                string setid = Request.QueryString["setid"];
                Session["CurrentTitleEditSet"] = setid;
            }

            // Is there a bibnumber in the URL?
            if (!String.IsNullOrEmpty(Request.QueryString["id"]))
            {
                string bibnumber = Request.QueryString["id"];
                int titleid;
                if (Int32.TryParse(bibnumber, out titleid))
                {
                    TitleInfo requestedTitle = DatabaseGateway.Get_Title(titleid, String.Empty, String.Empty, currentUser.PrimaryKey);
                    if (requestedTitle == null)
                        Response.Redirect("TitleEntry.aspx");
                    else
                    {
                        titleInfo = requestedTitle;
                        Session["CurrentTitleEdit"] = titleInfo;
                        Response.Redirect("TitleEdit.aspx");
                    }
                }
                else
                {
                    Response.Redirect("TitleEntry.aspx");
                }
            }



            // Look for some existing work in the session
            titleInfo = Session["CurrentTitleEdit"] as TitleInfo;

            // Add the record types options
            recordTypeButtons = new List<RadioButton>();
            foreach (RecordTypeInfo recordType in ApplicationCache.RecordTypes)
            {
                RadioButton recordTypeButton = new RadioButton();
                recordTypeButton.ID = "RecordType" + recordType.ID;
                recordTypeButton.Text = recordType.Text;
                recordTypeButton.ToolTip = recordType.Description;
                recordTypeButton.GroupName = "RecordTypeGroup";
                recordTypeButton.Style.Add("padding-right", "50px");
                recordTypeButton.AutoPostBack = true;

                //if ((!IsPostBack) && ( titleInfo != null ) && ( titleInfo.RecordType != null ))
                //{
                //    if (titleInfo.RecordType.ID == recordType.ID)
                //        recordTypeButton.Checked = true;
                //}

                recordTypeButton.CheckedChanged += recordTypeButton_CheckedChanged;

                RecordTypeArea.Controls.Add(recordTypeButton);

                recordTypeButtons.Add(recordTypeButton);
            }

            // Add the bibliographic level options
            bibLevelButtons = new List<RadioButton>();
            foreach (BibliographicLevelInfo bibLevel in ApplicationCache.BibliographicLevels)
            {
                RadioButton bibLevelButton = new RadioButton();
                bibLevelButton.ID = "BibLevel" + bibLevel.ID;
                bibLevelButton.Text = bibLevel.Level;
                bibLevelButton.GroupName = "BibLevelGroup";
                bibLevelButton.Style.Add("padding-right", "50px");

                BibLevelArea.Controls.Add(bibLevelButton);

                bibLevelButtons.Add(bibLevelButton);
            }

            // Add the record cleanup options
            cleanUpButtons = new List<CheckBox>();
            CleanUpTypeInfo otherCleanUp = null;
            int count = 0;
            foreach (CleanUpTypeInfo cleanupType in ApplicationCache.CleanupTypes)
            {
                // Don't add the OTHER yet
                if (String.Equals(cleanupType.Text, "other", StringComparison.OrdinalIgnoreCase))
                {
                    otherCleanUp = cleanupType;
                    continue;
                }

                // Time for a new line?
                if ((count > 0) && (count % 2 == 0))
                {
                    Literal newLine = new Literal { Text = "<br />" };
                    CleanupArea.Controls.Add(newLine);
                }

                CheckBox cleanupBox = new CheckBox();
                cleanupBox.ID = "CleanUp" + cleanupType.ID;
                cleanupBox.Text = cleanupType.Text;
                cleanupBox.ToolTip = cleanupType.Description;
                cleanupBox.Width = new Unit("200px");
                cleanupBox.Style.Add("height", "30px;");

                CleanupArea.Controls.Add(cleanupBox);

                cleanUpButtons.Add(cleanupBox);

                count++;
            }

            if (otherCleanUp != null)
            {
                Literal newLine2 = new Literal { Text = "<br />" };
                CleanupArea.Controls.Add(newLine2);

                // Add the OTHER option (with textbox)
                otherCheckBox = new CheckBox();
                otherCheckBox.ID = "CleanUp" + otherCleanUp.ID;
                otherCheckBox.Text = "Other";
                otherCheckBox.ToolTip = otherCleanUp.Description;
                //  otherCheckBox.Width = new Unit("200px");
                otherCheckBox.Style.Add("height", "30px;");
                CleanupArea.Controls.Add(otherCheckBox);
                cleanUpButtons.Add(otherCheckBox);

                // Add the OTHER text box
                otherTextBox = new TextBox();
                otherTextBox.Width = new Unit("420px");
                otherTextBox.Style.Add("margin-left", "10px;");
                otherTextBox.ID = "OtherCleanUp";
                CleanupArea.Controls.Add(otherTextBox);
            }

            // If this is not a post-back, add all the controls
            if (!IsPostBack)
            {
                // Add the cataloging types
                CatalogingTypeDropDown.Items.Add(new ListItem(String.Empty, "-1"));
                foreach (CatalogingTypeInfo catType in ApplicationCache.CatalogingTypes)
                {
                    CatalogingTypeDropDown.Items.Add(new ListItem(catType.Text + "  ", catType.ID.ToString()));
                }

                // Add the document types
                DocTypeDropDown.Items.Add(new ListItem(String.Empty, "-1"));
                foreach (DocumentTypeInfo docType in ApplicationCache.DocumentTypes)
                {
                    DocTypeDropDown.Items.Add(new ListItem(docType.Text + "      ", docType.ID.ToString()));
                }

                // If it is not postback, and there is a current work title, display it
                if (titleInfo != null)
                {
                    show_title_info(titleInfo);
                }
                else
                {

                    DocumentTypeTableRow.Visible = false;
                    FedAgencyTableRow.Visible = false;
                    IssnTableRow.Visible = false;
                }
            }
        }


        void recordTypeButton_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            if (checkbox.ID == "RecordType1")
            {
                DocumentTypeTableRow.Visible = true;
                FedAgencyTableRow.Visible = true;
                IssnTableRow.Visible = false;
            }
            else
            {
                DocumentTypeTableRow.Visible = false;
                FedAgencyTableRow.Visible = false;
                IssnTableRow.Visible = true;
            }
        }

        private void show_title_info(TitleInfo foundInfo)
        {
            AlephNumLabel.Text = foundInfo.AlephNum;
            TitleTextBox.Text = foundInfo.Title;
            DocTypeDropDown.SelectedValue = (foundInfo.DocumentType != null) ? foundInfo.DocumentType.ID.ToString() : "-1";
            FederalAgencyTextBox.Text = foundInfo.FederalAgency;
            IssnTextBox.Text = foundInfo.ISSN;
            CatalogingCheckBox.Checked = foundInfo.SendToCataloging;
            NotesTextBox.Text = foundInfo.GeneralNotes;
            if (foundInfo.RecordType != null)
            {
                foreach (RadioButton button in recordTypeButtons)
                {
                    if (button.ID == "RecordType" + foundInfo.RecordType.ID)
                    {
                        button.Checked = true;
                        break;
                    }
                }
            }

            if (foundInfo.BibliographicLevel != null)
            {
                foreach (RadioButton button in bibLevelButtons)
                {
                    if (button.ID == "BibLevel" + foundInfo.BibliographicLevel.ID)
                    {
                        button.Checked = true;
                        break;
                    }
                }
            }

            // Add the record clean-up
            Dictionary<int, CheckBox> checkboxes = new Dictionary<int, CheckBox>();
            foreach (CheckBox thisCheckbox in cleanUpButtons)
            {
                checkboxes.Add(int.Parse(thisCheckbox.ID.Replace("CleanUp", "")), thisCheckbox);
            }
            foreach (TitleInfo_CleanUp cleanUp in foundInfo.Tasks)
            {
                if (checkboxes.ContainsKey(cleanUp.PrimaryKey))
                {
                    checkboxes[cleanUp.PrimaryKey].Checked = true;

                    if (cleanUp.CleanUpType == "Other")
                        otherTextBox.Text = cleanUp.OtherDescription;
                }
            }

            if (foundInfo.RecordType.ID == 1)
            {
                DocumentTypeTableRow.Visible = true;
                IssnTableRow.Visible = false;

                if (foundInfo.DocumentType.Text.IndexOf("Federal") >= 0)
                    FedAgencyTableRow.Visible = true;
                else
                    FedAgencyTableRow.Visible = false;
            }
            else
            {
                DocumentTypeTableRow.Visible = false;
                FedAgencyTableRow.Visible = false;
                IssnTableRow.Visible = true;
            }
        }


        private bool load_validate_title_info()
        {
            // Ensure some title exists
            if (titleInfo == null)
                titleInfo = new TitleInfo();

            // Load data into the title and validate form
            bool valid = true;
           
            // Check the title
            if (String.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                TitleLabel.Text = "Required";
                valid = false;
            }
            else
            {
                titleInfo.Title = TitleTextBox.Text;
                TitleLabel.Text = String.Empty;
            }

            // Check the record type
            titleInfo.RecordType = null;
            foreach (CheckBox recordTypeBox in recordTypeButtons)
            {
                if (recordTypeBox.Checked)
                {
                    titleInfo.RecordType = new RecordTypeInfo(Int32.Parse(recordTypeBox.ID.Replace("RecordType", "")), recordTypeBox.Text, recordTypeBox.Text);
                    break;
                }
            }
            if (titleInfo.RecordType == null)
            {
                RecordTypeLabel.Text = "Required";
                valid = false;
            }
            else
            {
                RecordTypeLabel.Text = String.Empty;
            }

            // Check the bib level
            titleInfo.BibliographicLevel = null;
            foreach (CheckBox bibLevlBox in bibLevelButtons)
            {
                if (bibLevlBox.Checked)
                {
                    titleInfo.BibliographicLevel = new BibliographicLevelInfo(Int32.Parse(bibLevlBox.ID.Replace("BibLevel", "")), bibLevlBox.Text);
                    break;
                }
            }
            if (titleInfo.BibliographicLevel == null)
            {
                BibLevelLabel.Text = "Required";
                valid = false;
            }
            else
            {
                BibLevelLabel.Text = String.Empty;
            }

            // Check the document type drop down
            titleInfo.DocumentType = null;
            if (DocTypeDropDown.SelectedValue == "-1")
            {
                DocTypeLabel.Text = "Required";
                valid = false;
            }
            else
            {
                titleInfo.DocumentType = new DocumentTypeInfo(Int32.Parse(DocTypeDropDown.SelectedValue), DocTypeDropDown.SelectedItem.Text, String.Empty);
                DocTypeLabel.Text = String.Empty;
            }

            // Add the federal agency (no validation currently)
            if (FedAgencyTableRow.Visible)
                titleInfo.FederalAgency = FederalAgencyTextBox.Text;
            else
                titleInfo.FederalAgency = String.Empty;


            // Save all the record cleanups
            titleInfo.Tasks.Clear();
            foreach (CheckBox task in cleanUpButtons)
            {
                if (task.Checked)
                {
                    string task_name = task.Text;
                    int task_id = int.Parse(task.ID.Replace("CleanUp", ""));

                    // Was this the OTHER?
                    if (task.Text == "Other")
                    {
                        TitleInfo_CleanUp cleanUp = new TitleInfo_CleanUp(task_id, task_name);
                        cleanUp.OtherDescription = otherTextBox.Text.Trim();
                        if (cleanUp.OtherDescription.Length > 0)
                        {
                            titleInfo.Tasks.Add(cleanUp);
                        }
                    }
                    else
                    {
                        titleInfo.Tasks.Add(new TitleInfo_CleanUp(task_id, task_name));
                    }
                }
            }

            // Save the general notes
            titleInfo.GeneralNotes = NotesTextBox.Text.Trim();

            // Save the ISSN info
            titleInfo.ISSN = IssnTextBox.Text.Trim();
            if ((CatalogingCheckBox.Visible) && (IssnTableRow.Visible))
                titleInfo.SendToCataloging = CatalogingCheckBox.Checked;

            // Save this to the session state either way
            Session["CurrentTitleEdit"] = titleInfo;

            return valid;
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            // Is there a current set?
            string setid = Session["CurrentTitleEditSet"] as string;

            // Clear current work
            Session["CurrentTitleEditSet"] = null;
            Session["CurrentTitleEdit"] = null;
            
            // Send back differently
            if (!String.IsNullOrEmpty(setid))
            {
                Response.Redirect("Correct.aspx?setid=" + setid);
            }
            else
            {
                Response.Redirect("Default.aspx");
            }
            
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            // Try to save the title
            if (save_title())
            {
                // Is there a current set?
                string setid = Session["CurrentTitleEditSet"] as string;

                // Clear current work
                Session["CurrentTitleEditSet"] = null;
                Session["CurrentTitleEdit"] = null;

                // Send back differently
                if (!String.IsNullOrEmpty(setid))
                {
                    Response.Redirect("Correct.aspx?setid=" + setid);
                }
                else
                {
                    Response.Redirect("Default.aspx");
                }
            }
        }

        private bool save_title()
        {
            FormSavedLabel.Text = String.Empty;
            FormError.Text = String.Empty;

            // Was the title information valid?
            bool valid = load_validate_title_info();
            if (!valid)
            {
                FormError.Text = "Title is missing required fields";
                return false;
            }

            // Since the title info was complete, ensure it is saved
            Tuple<int, string> returnValue = DatabaseGateway.Save_Basic_Title_Info(titleInfo, currentUser.PrimaryKey);
            int titleId = returnValue.Item1;
            if (titleId <= 0)
            {
                FormError.Text = returnValue.Item2;
                return false;
            }
            else
            {
                titleInfo.PrimaryKey = titleId;
            }

            // Also save each record clean-up, comparing the original with the current
            Dictionary<int, bool> original_tasks = titleInfo.OriginalTasks.ToDictionary(original => original.PrimaryKey, original => false);
            foreach (TitleInfo_CleanUp new_task in titleInfo.Tasks)
            {
                // Is this an existing task, or a new one?
                if (original_tasks.ContainsKey(new_task.PrimaryKey))
                {
                    // This already exists, so just mark it as still present
                    original_tasks[new_task.PrimaryKey] = true;

                    // If this is the OTHER option, also need to look at the other descriptoins
                    if (new_task.CleanUpType == "Other")
                    {
                        // Did the descriptions match?
                        string existing_desc = String.Empty;
                        foreach (TitleInfo_CleanUp origTask in titleInfo.OriginalTasks)
                        {
                            if (origTask.PrimaryKey == new_task.PrimaryKey)
                            {
                                existing_desc = origTask.OtherDescription.Trim();
                            }
                        }

                        // If the description is different, save anyway
                        if (new_task.OtherDescription.Trim() != existing_desc)
                        {
                            DatabaseGateway.Save_Title_CleanUp(titleInfo.PrimaryKey, new_task.PrimaryKey, new_task.OtherDescription, currentUser.PrimaryKey);
                        }

                    }
                }
                else
                {
                    // This is a new task
                    DatabaseGateway.Save_Title_CleanUp(titleInfo.PrimaryKey, new_task.PrimaryKey, new_task.OtherDescription, currentUser.PrimaryKey);
                }
            }

            // We have now added all the new ones.. are there any old ones to REMOVE?
            foreach (KeyValuePair<int, bool> original in original_tasks)
            {
                // Was this not found?
                if (!original.Value)
                {
                    // Clear this
                    DatabaseGateway.Remove_Title_CleanUp(titleInfo.PrimaryKey, original.Key, currentUser.PrimaryKey);
                }
            }

            // Since we have saved everything to the database, set the new tasks
            titleInfo.OriginalTasks.Clear();
            foreach (TitleInfo_CleanUp newTask in titleInfo.Tasks)
            {
                titleInfo.OriginalTasks.Add(newTask);
            }

            // Saved, so say so
            FormSavedLabel.Text = "Title saved";

            return true;
        }

        protected void DocTypeDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((DocTypeDropDown.SelectedItem != null) && (DocTypeDropDown.SelectedItem.Text.IndexOf("Federal") >= 0))
            {
                FedAgencyTableRow.Visible = true;
            }
            else
            {
                FedAgencyTableRow.Visible = false;
            }
        }

    }
}