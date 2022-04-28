using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class TitleEntry : System.Web.UI.Page
    {
        private TitleInfo titleInfo;
        private UserInfo currentUser;
        private List<RadioButton> recordTypeButtons;
        private List<RadioButton> bibLevelButtons;

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



            // Is there a bibnumber in the URL?
            if (!String.IsNullOrEmpty(Request.QueryString["id"]))
            {
                string bibnumber = Request.QueryString["id"];

                // Is this the special bib number 'new'?
                if (bibnumber == "new")
                {
                    Session["CurrentWork"] = null;
                    Response.Redirect("TitleEntry.aspx");
                    return;
                }

                // See if we can lookup a title based on this
                int titleid;
                if (Int32.TryParse(bibnumber, out titleid))
                {
                    TitleInfo requestedTitle = DatabaseGateway.Get_Title(titleid, String.Empty, String.Empty, currentUser.PrimaryKey);
                    if (requestedTitle == null)
                        Response.Redirect("TitleEntry.aspx");
                    else
                    {
                        titleInfo = requestedTitle;
                        Session["CurrentWork"] = titleInfo;
                        Response.Redirect("TitleEntry.aspx");
                    }
                }
                else
                {
                    Response.Redirect("TitleEntry.aspx");
                }
            }

            // Look for some existing work in the session
            titleInfo = Session["CurrentWork"] as TitleInfo;


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

 


            //// Add the record cleanup options
            //cleanUpButtons = new List<CheckBox>();
            //CleanUpTypeInfo otherCleanUp = null;
            //int count = 0;
            //foreach (CleanUpTypeInfo cleanupType in ApplicationCache.CleanupTypes)
            //{
            //    // Don't add the OTHER yet
            //    if (String.Equals(cleanupType.Text, "other", StringComparison.OrdinalIgnoreCase))
            //    {
            //        otherCleanUp = cleanupType;
            //        continue;
            //    }

            //    // Time for a new line?
            //    if ((count > 0) && (count % 2 == 0))
            //    {
            //        Literal newLine = new Literal {Text = "<br />"};
            //        CleanupArea.Controls.Add(newLine);
            //    }

            //    CheckBox cleanupBox = new CheckBox();
            //    cleanupBox.ID = "CleanUp" + cleanupType.ID;
            //    cleanupBox.Text = cleanupType.Text;
            //    cleanupBox.ToolTip = cleanupType.Description;
            //    cleanupBox.Width = new Unit("200px");
            //    cleanupBox.Style.Add("height", "30px;");

            //    CleanupArea.Controls.Add(cleanupBox);

            //    cleanUpButtons.Add(cleanupBox);

            //    count++;
            //}

            //if (otherCleanUp != null)
            //{
            //    Literal newLine2 = new Literal {Text = "<br />"};
            //    CleanupArea.Controls.Add(newLine2);

            //    // Add the OTHER option (with textbox)
            //    otherCheckBox = new CheckBox();
            //    otherCheckBox.ID = "CleanUp" + otherCleanUp.ID;
            //    otherCheckBox.Text = "Other";
            //    otherCheckBox.ToolTip = otherCleanUp.Description;
            //    //  otherCheckBox.Width = new Unit("200px");
            //    otherCheckBox.Style.Add("height", "30px;");
            //    CleanupArea.Controls.Add(otherCheckBox);
            //    cleanUpButtons.Add(otherCheckBox);

            //    // Add the OTHER text box
            //    otherTextBox = new TextBox();
            //    otherTextBox.Width = new Unit("420px");
            //    otherTextBox.Style.Add("margin-left", "10px;");
            //    otherTextBox.ID = "OtherCleanUp";
            //    CleanupArea.Controls.Add(otherTextBox);
            //}

            // If this is not a post-back, add all the controls
            if (!IsPostBack)
            {
                // Add the document types
                DocTypeDropDown.Items.Add(new ListItem(String.Empty, "-1"));
                foreach (DocumentTypeInfo docType in ApplicationCache.DocumentTypes)
                {
                    DocTypeDropDown.Items.Add(new ListItem(docType.Text + "      ", docType.ID.ToString()));
                }



                // If it is not postback, and there is a current work title, display it
                if (titleInfo != null)
                {
                    TitleSearchPanel.Visible = false;
                    TitleInfoPanel.Visible = true;
                    SaveButton.Visible = true;
                    ItemsButton.Visible = true;
                    show_title_info(titleInfo);
                }
                else
                {
                    TitleSearchPanel.Visible = true;
                    TitleInfoPanel.Visible = false;
                    SaveButton.Visible = false;
                    ItemsButton.Visible = false;
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

        protected void OclcNumTextBox_TextChanged(object sender, EventArgs e)
        {
            // Get the new aleph number
            string oclc = OclcNumTextBox.Text.Trim();

            // If this matches the data already in memory, do nothing
            if ((titleInfo != null) && (titleInfo.OCLC == oclc))
                return;

            // This pretty much trumps any information already entered
            TitleLabel.Text = String.Empty;
            BibLevelLabel.Text = String.Empty;
            DocTypeLabel.Text = String.Empty;
            RecordTypeLabel.Text = String.Empty;
            FormError.Text = String.Empty;

            // If empty, add a prompt
            if (String.IsNullOrEmpty(oclc))
            {
                Clear_Form();
                titleInfo = null;
                Session["CurrentWork"] = null;
                AlephNumLabel.Text = "Enter a valid OCLC number";
                return;
            }

            // Was this found?  (Look in the database here)
            TitleInfo foundInfo = DatabaseGateway.Get_Title(-1, String.Empty, oclc, currentUser.PrimaryKey);

            // Was this new?
            if (foundInfo.PrimaryKey < 0)
            {
                AlephNumLabel.Text = "New Title";
                AlephNumLabel2.Text = "New Title";
            }
            else
            {
                // Show the title information
                AlephNumLabel.Text = "Existing Title";
                AlephNumLabel2.Text = "Existing Title";
            }

            TitleSearchPanel.Visible = false;
            TitleInfoPanel.Visible = true;
            SaveButton.Visible = true;
            ItemsButton.Visible = true;
            show_title_info(foundInfo);

            // Save this item to the cache
            titleInfo = foundInfo;
            Session["CurrentWork"] = titleInfo;
        }

        protected void AlephNumTextBox_TextChanged(object sender, EventArgs e)
        {
            // Get the new aleph number
            string aleph = AlephNumTextBox.Text.Trim();

            // If this matches the data already in memory, do nothing
            if ((titleInfo != null) && (titleInfo.AlephNum == aleph))
                return;

            // This pretty much trumps any information already entered
            TitleLabel.Text = String.Empty;
            BibLevelLabel.Text = String.Empty;
            DocTypeLabel.Text = String.Empty;
            RecordTypeLabel.Text = String.Empty;
            FormError.Text = String.Empty;

            // If empty, add a prompt
            if ((String.IsNullOrEmpty(aleph)) || ( aleph.Length < 8))
            {
                Clear_Form();
                titleInfo = null;
                Session["CurrentWork"] = null;
                AlephNumLabel.Text = "Enter a valid ALEPH number";
                return;
            }

            // Was this found?  (Look in the database here)
            TitleInfo foundInfo = DatabaseGateway.Get_Title(-1, aleph, String.Empty, currentUser.PrimaryKey);

            // Was this new?
            if (foundInfo.PrimaryKey < 0)
            {
                AlephNumLabel.Text = "New Title";
                AlephNumLabel2.Text = "New Title";
            }
            else
            {
                // Show the title information
                AlephNumLabel.Text = "Existing Title";
                AlephNumLabel2.Text = "Existing Title";
            }

            TitleSearchPanel.Visible = false;
            TitleInfoPanel.Visible = true;
            SaveButton.Visible = true;
            ItemsButton.Visible = true;
            show_title_info(foundInfo);

            // Save this item to the cache
            titleInfo = foundInfo;
            Session["CurrentWork"] = titleInfo;
        }

        private void Clear_Form()
        {
            // Clear the form here
            TitleInfoPanel.Visible = false;
            SaveButton.Visible = false;
            ItemsButton.Visible = false;
        }

        private void show_title_info(TitleInfo foundInfo)
        {
            AlephNumTextBox.Text = foundInfo.AlephNum;
            OclcNumTextBox.Text = foundInfo.OCLC;
            TitleTextBox.Text = foundInfo.Title;
            DocTypeDropDown.SelectedValue = (foundInfo.DocumentType != null) ? foundInfo.DocumentType.ID.ToString() : "-1";
            FederalAgencyTextBox.Text = foundInfo.FederalAgency;
            CatalogingCheckBox.Checked = foundInfo.SendToCataloging;
            NotesTextBox.Text = foundInfo.GeneralNotes;

            if ( !String.IsNullOrEmpty(foundInfo.AlephNum))
            {
                AlephNumEntryLabel.Visible = true;
                AlephNumEntryLabel.Text = foundInfo.AlephNum;
                AlephNumEntryTextBox.Visible = false;
                AlephNumEntryTextBox.Text = foundInfo.AlephNum;
            }
            else
            {
                AlephNumEntryLabel.Visible = false;
                AlephNumEntryLabel.Text = String.Empty;
                AlephNumEntryTextBox.Visible = true;
                AlephNumEntryTextBox.Text = String.Empty;
            }

            if (!String.IsNullOrEmpty(foundInfo.OCLC))
            {
                OclcNumEntryLabel.Visible = true;
                OclcNumEntryLabel.Text = foundInfo.OCLC;
                OclcNumEntryTextBox.Visible = false;
                OclcNumEntryTextBox.Text = foundInfo.OCLC;
            }
            else
            {
                OclcNumEntryLabel.Visible = false;
                OclcNumEntryLabel.Text = String.Empty;
                OclcNumEntryTextBox.Visible = true;
                OclcNumEntryTextBox.Text = String.Empty;
            }

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

            //// Add the record clean-up
            //Dictionary<int, CheckBox> checkboxes = new Dictionary<int, CheckBox>();
            //foreach (CheckBox thisCheckbox in cleanUpButtons)
            //{
            //    checkboxes.Add(int.Parse(thisCheckbox.ID.Replace("CleanUp", "")), thisCheckbox);
            //}
            //foreach (TitleInfo_CleanUp cleanUp in foundInfo.Tasks)
            //{
            //    if (checkboxes.ContainsKey(cleanUp.PrimaryKey))
            //    {
            //        checkboxes[cleanUp.PrimaryKey].Checked = true;

            //        if (cleanUp.CleanUpType == "Other")
            //            otherTextBox.Text = cleanUp.OtherDescription;
            //    }
            //}

            if (( foundInfo.RecordType != null ) && (foundInfo.RecordType.ID == 1))
            {
                DocumentTypeTableRow.Visible = true;
                IssnTableRow.Visible = false;

                if ((foundInfo.DocumentType != null ) && ( foundInfo.DocumentType.Text.IndexOf("Federal", StringComparison.Ordinal) >= 0 ))
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

            // Check the aleph number
            bool found_identifier = false;
            if (!String.IsNullOrWhiteSpace(AlephNumEntryTextBox.Text))
            {
                titleInfo.AlephNum = AlephNumEntryTextBox.Text;
                found_identifier = true;
            }

            if (!String.IsNullOrWhiteSpace(OclcNumEntryTextBox.Text))
            {
                titleInfo.OCLC = OclcNumEntryTextBox.Text;
                found_identifier = true;
            }

            // Was one of the two identifiers found?
            if (!found_identifier)
            {
                AlephNumLabel.Text = "Either an Aleph or OCLC number is required.";
                valid = false;
            }

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
            if (DocumentTypeTableRow.Visible)
            {
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
            }

            // Add the federal agency (no validation currently)
            if (FedAgencyTableRow.Visible)
                titleInfo.FederalAgency = FederalAgencyTextBox.Text;
            else
                titleInfo.FederalAgency = String.Empty;


            // Save the general notes
            titleInfo.GeneralNotes = NotesTextBox.Text.Trim();

            // Save the ISSN info
            if ((CatalogingCheckBox.Visible) && ( IssnTableRow.Visible))
                titleInfo.SendToCataloging = CatalogingCheckBox.Checked;

            // Save this to the session state either way
            Session["CurrentWork"] = titleInfo;

            return valid;
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Session["CurrentWork"] = null;
            Response.Redirect("Default.aspx");
        }

        protected void ClearButton_Click(object sender, EventArgs e)
        {
            Session["CurrentWork"] = null;
            Response.Redirect("TitleEntry.aspx");
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            save_title();
        }

        protected void ItemsButton_Click(object sender, EventArgs e)
        {
            if (!save_title())
            {
                FormError.Text = "Error encountered saving to database.  Contact system admin.";
            }
            else
            {
                Response.Redirect("ItemEntry.aspx");
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
                AlephNumLabel.Text = returnValue.Item2;
                titleInfo.PrimaryKey = titleId;
                Session["CurrentWork"] = titleInfo;
            }

            //// Also save each record clean-up, comparing the original with the current
            //Dictionary<int, bool> original_tasks = titleInfo.OriginalTasks.ToDictionary(original => original.PrimaryKey, original => false);
            //foreach (TitleInfo_CleanUp new_task in titleInfo.Tasks)
            //{
            //    // Is this an existing task, or a new one?
            //    if (original_tasks.ContainsKey(new_task.PrimaryKey))
            //    {
            //        // This already exists, so just mark it as still present
            //        original_tasks[new_task.PrimaryKey] = true;

            //        // If this is the OTHER option, also need to look at the other descriptoins
            //        if (new_task.CleanUpType == "Other")
            //        {
            //            // Did the descriptions match?
            //            string existing_desc = String.Empty;
            //            foreach (TitleInfo_CleanUp origTask in titleInfo.OriginalTasks)
            //            {
            //                if (origTask.PrimaryKey == new_task.PrimaryKey)
            //                {
            //                    existing_desc = origTask.OtherDescription.Trim();
            //                }
            //            }

            //            // If the description is different, save anyway
            //            if (new_task.OtherDescription.Trim() != existing_desc)
            //            {
            //                DatabaseGateway.Save_Title_CleanUp(titleInfo.PrimaryKey, new_task.PrimaryKey, new_task.OtherDescription, currentUser.PrimaryKey);
            //            }

            //        }
            //    }
            //    else
            //    {
            //        // This is a new task
            //        DatabaseGateway.Save_Title_CleanUp(titleInfo.PrimaryKey, new_task.PrimaryKey, new_task.OtherDescription, currentUser.PrimaryKey);
            //    }
            //}

            //// We have now added all the new ones.. are there any old ones to REMOVE?
            //foreach (KeyValuePair<int, bool> original in original_tasks)
            //{
            //    // Was this not found?
            //    if (!original.Value)
            //    {
            //        // Clear this
            //        DatabaseGateway.Remove_Title_CleanUp(titleInfo.PrimaryKey, original.Key, currentUser.PrimaryKey);
            //    }
            //}

            //// Since we have saved everything to the database, set the new tasks
            //titleInfo.OriginalTasks.Clear();
            //foreach (TitleInfo_CleanUp newTask in titleInfo.Tasks)
            //{
            //    titleInfo.OriginalTasks.Add(newTask);
            //}

            // Saved, so say so
            FormSavedLabel.Text = "Title saved";

            return true;
        }

        protected void DocTypeDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((DocTypeDropDown.SelectedItem != null) && (DocTypeDropDown.SelectedItem.Text.IndexOf("Federal", StringComparison.OrdinalIgnoreCase) >= 0))
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