using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using FlareWorks.Library.Database;
using FlareWorks.Library.Models.ControlledValues;
using FlareWorks.Library.Models.QC;
using FlareWorks.MemoryMgmt;
using FlareWorks.Models.ControlledValues;
using FlareWorks.Models.Users;
using FlareWorks.Models.Work;
using FlareworksWeb.UserMgmt;

namespace FlareworksWeb
{
    public partial class ItemEntry : System.Web.UI.Page
    {
        private TitleInfo titleInfo;
        private UserInfo currentUser;
        private WorkSet currentWorkSet;
        private Dictionary<int, RadioButton> catalogTypeButtons;
        private RadioButton pccNewButton;
        private RadioButton pccMaintButton;

        private int authority_rows_existing;
        private List<DropDownList> authTypes;
        private List<RadioButtonList> authActions;

        // Item table buttons labels and text boxes, for editing and deleting ( items trayed, withdrawn, damaged )
        Dictionary<string, LinkButton> editItemsButtons;
        Dictionary<string, LinkButton> deleteItemsButtons;
        Dictionary<string, LinkButton> saveItemsButtons;
        Dictionary<string, LinkButton> cancelItemsButtons;
        Dictionary<string, TextBox> itemsTrayedBoxes;
        Dictionary<string, Label> itemsTrayedLabels;
        Dictionary<string, TextBox> itemsWithdrawnBoxes;
        Dictionary<string, Label> itemsWithdrawnLabels;
        Dictionary<string, TextBox> itemsDamagedBoxes;
        Dictionary<string, Label> itemsDamagedLabels;
        Dictionary<string, TableRow> itemsRows;


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

            if (!currentUser.Permissions.CanProcessItems)
            {
                Response.Redirect("Default.aspx");
                return;
            }

            // Look for some existing work in the session
            titleInfo = Session["CurrentWork"] as TitleInfo;

            // If no work, then this needs to be sent ot the title entry to select a title
            if (titleInfo == null)
            {
                Response.Redirect("TitleEntry.aspx");
                return;
            }

            // Is there an unsubmitted item work set already for this title/user?
            currentWorkSet = Session["CurrentWorkSet_" + titleInfo.PrimaryKey] as WorkSet;
            if (currentWorkSet == null)
            {
                currentWorkSet = DatabaseGateway.Get_Workset(currentUser.PrimaryKey, titleInfo.PrimaryKey);
                Session["CurrentWorkSet_" + titleInfo.PrimaryKey] = currentWorkSet;
            }


            // Show the items too
            show_itemwork();
        }

        protected void Page_Load(object sender, EventArgs e)
        {


            // Set the PCC Category row to display or not based on user settings
            if (!currentUser.Permissions.CatalogingSpecialist)
            {
                PccCategoryRow.Visible = false;
                AuthorityCatalogingTable.Visible = false;
            }

            TitleLabel.Text = titleInfo.Title;

            #region Add the cataloging types buttons

            catalogTypeButtons = new Dictionary<int, RadioButton>();
            RadioButton noneCatTypeButton = new RadioButton();
            noneCatTypeButton.ID = "CatType-1";
            noneCatTypeButton.Text = "None";
            noneCatTypeButton.GroupName = "CatTypeGroup";
            noneCatTypeButton.Style.Add("padding-right", "50px");
            noneCatTypeButton.AutoPostBack = true;
            noneCatTypeButton.CheckedChanged += General_OnCheckedChanged;
            CatalogingTypePlaceHolder.Controls.Add(noneCatTypeButton);
            foreach (CatalogingTypeInfo catType in ApplicationCache.CatalogingTypes)
            {
                RadioButton catTypeButton = new RadioButton();
                catTypeButton.ID = "CatType" + catType.ID;
                catTypeButton.Text = catType.Text;
                catTypeButton.GroupName = "CatTypeGroup";
                catTypeButton.Style.Add("padding-right", "50px");
                catTypeButton.AutoPostBack = true;
                catTypeButton.CheckedChanged += General_OnCheckedChanged;
                CatalogingTypePlaceHolder.Controls.Add(catTypeButton);

                catalogTypeButtons.Add(catType.ID, catTypeButton);
            }
            if (!IsPostBack)
                noneCatTypeButton.Checked = true;

            #endregion

            #region Add the PCC Category types buttons


           pccNewButton = new RadioButton();
           pccNewButton.ID = "PccCatNew";
           pccNewButton.Text = "New Authentication";
           pccNewButton.GroupName = "PccCatGroup";
           pccNewButton.Style.Add("padding-right", "50px");
              //  pccNewButton.AutoPostBack = true;
               // pccNewButton.CheckedChanged += General_OnCheckedChanged;
           PccCategoryPlaceHolder.Controls.Add(pccNewButton);

            pccMaintButton = new RadioButton();
            pccMaintButton.ID = "PccCatMaint";
            pccMaintButton.Text = "Maintenance";
            pccMaintButton.GroupName = "PccCatGroup";
            pccMaintButton.Style.Add("padding-right", "50px");
          //  pccMaintButton.AutoPostBack = true;
          //  pccMaintButton.CheckedChanged += General_OnCheckedChanged;
            PccCategoryPlaceHolder.Controls.Add(pccMaintButton);


            if (!IsPostBack)
            {
                PccCategoryMainDropDown.SelectedIndex = 0;
                pccNewButton.Checked = true;
            }

            #endregion

            #region Add the Item/HOL action types buttons

            ItemHol_None.Style.Add("padding-right", "50px");
            ItemHol_Process.Style.Add("padding-right", "50px");
            ItemHol_Edit.Style.Add("padding-right", "50px");
            if (!IsPostBack)
            {
                ItemHol_None.Checked = true;
            }

            #endregion

            // Build the lists to hold all the authority parameter controls
            authTypes = new List<DropDownList>();
            authActions = new List<RadioButtonList>();

            // Add the initial (always there row) to the lists
            authTypes.Add(Authority_Type0);
            authActions.Add(Authority_Action0);

            // Ensure the options exist on the very first field
            if (!IsPostBack)
            {
                Authority_Type0.Items.Add(new ListItem(String.Empty, "-1"));
                foreach (var authCatType in ApplicationCache.AuthorityRecordTypes)
                {
                    Authority_Type0.Items.Add(new ListItem(authCatType.RecordType, authCatType.ID.ToString()));
                }
            }

            // Get the number of rows
            authority_rows_existing = 1;
            if (IsPostBack)
            {
                authority_rows_existing = Convert.ToInt32(AuthorityRowsCount.Value);
            }

            // Ensure this number holds all the ones in the object
            if ( currentWorkSet.AuthorityWork.Count > authority_rows_existing )
            {
                authority_rows_existing = currentWorkSet.AuthorityWork.Count;
                AuthorityRowsCount.Value = authority_rows_existing.ToString();
            }

            // Build any rows that are needed
            for (int i = 2; i <= authority_rows_existing; i++)
            {
                add_authority_row(i, false);
            }

            // If this is not postback, add the values from the object
            if ( !IsPostBack)
            {
                int row = 0;
                foreach( WorkSet_AuthorityWork authWork in currentWorkSet.AuthorityWork )
                {
                    authTypes[row].SelectedValue = authWork.AuthorityRecordTypeID.ToString();
                    authActions[row].SelectedIndex = (authWork.OriginalWork) ? 0 : 1;
                    row++;
                }
            }



            // If this is not a post-back, add all the controls
            if (!IsPostBack)
            {
                // Add the institutions
                InstitutionDropDown.Items.Add(new ListItem(String.Empty, "-1"));
                foreach (InstitutionInfo institution in ApplicationCache.Institutions)
                {
                    InstitutionDropDown.Items.Add(new ListItem(institution.Code + "  ", institution.ID.ToString()));
                }

                // Add the material types
                MaterialTypeDropDown.Items.Add(new ListItem(String.Empty, "-1"));
                foreach (MaterialTypeInfo matType in ApplicationCache.MaterialTypes)
                {
                    MaterialTypeDropDown.Items.Add(new ListItem(matType.Text + "  ", matType.ID.ToString()));
                }

                // Set the institution from the workset, or the user last added
                if ( currentWorkSet.Institution != null )
                    InstitutionDropDown.SelectedValue = currentWorkSet.Institution.ID.ToString();
                else if (!String.IsNullOrEmpty(currentUser.Recents.LastAdded.Institution))
                    InstitutionDropDown.SelectedValue = ApplicationCache.Get_Institution(currentUser.Recents.LastAdded.Institution).ID.ToString();

                // Set the material type from the workset, or the user last added
                if ( currentWorkSet.MaterialType != null )
                    MaterialTypeDropDown.SelectedValue = currentWorkSet.MaterialType.ID.ToString();
                else if (!String.IsNullOrEmpty(currentUser.Recents.LastAdded.MaterialType))
                    MaterialTypeDropDown.SelectedValue = ApplicationCache.Get_Material_Type(currentUser.Recents.LastAdded.MaterialType).ID.ToString(); 




                // Set the notes
                if (!String.IsNullOrEmpty(currentWorkSet.ProcessorNotes))
                    NotesTextBox.Text = currentWorkSet.ProcessorNotes;

                // Set the item hol type
                if (currentWorkSet.ItemHolActionType != null)
                {
                    if ( String.Equals(currentWorkSet.ItemHolActionType.ActionType, "Edit", StringComparison.OrdinalIgnoreCase))
                    {
                        NewItemPanel.Visible = false;
                        ItemHol_Edit.Checked = true;
                        ItemHol_EditCount.Text = currentWorkSet.ItemHol_EditCount.ToString();
                    }
                    else if (String.Equals(currentWorkSet.ItemHolActionType.ActionType, "Process", StringComparison.OrdinalIgnoreCase))
                    {
                        NewItemPanel.Visible = true;
                        ItemHol_Process.Checked = true;
                        ItemHol_EditCount.Visible = false;
                        ItemHol_EditCountLabel.Visible = false;
                    }
                    else
                    {
                        NewItemPanel.Visible = false;
                        ItemHol_EditCount.Visible = false;
                        ItemHol_EditCountLabel.Visible = false;
                    }
                }
                else
                {
                    NewItemPanel.Visible = false;
                    ItemHol_EditCount.Visible = false;
                    ItemHol_EditCountLabel.Visible = false;
                }

                // Set the PCC category type 
                if (currentWorkSet.PccCategory != null)
                {
                    for( int i = 0; i < PccCategoryMainDropDown.Items.Count; i++ )
                    {
                        if ( PccCategoryMainDropDown.Items[i].Text.Equals(currentWorkSet.PccCategory.Category, StringComparison.OrdinalIgnoreCase))
                        {
                            PccCategoryMainDropDown.SelectedIndex = i;
                            break;
                        }
                    }

                    if (currentWorkSet.Pcc_Maintenance)
                    {
                        pccMaintButton.Checked = true;
                        pccNewButton.Checked = false;
                    }
                    else
                    {
                        pccMaintButton.Checked = false;
                        if (currentWorkSet.Pcc_NewAuthentication)
                            pccNewButton.Checked = true;
                        else
                            pccNewButton.Checked = false;
                    }
                    

                }
                else
                {
                    PccCategoryPlaceHolder.Visible = false;
                }


                // Set the cataloging type
                if (currentWorkSet.CatalogingType != null)
                {
                    catalogTypeButtons[currentWorkSet.CatalogingType.ID].Checked = true;
                }


                // Show the last copy info
                LastCopyCheckBox.Checked = currentWorkSet.LastCopy;
                if (currentWorkSet.LastCopy)
                {
                    LastCopyInstitutionBox.Text = currentWorkSet.LastCopyInstitution;
                    LastCopyInstitutionBox.Visible = true;
                }
            }
        }

        private void Clear_Form()
        {
            // Clear the form here
        }

        protected void AddItemButton_Click(object sender, EventArgs e)
        {
            // Validate the item information here
            bool valid = true;

            // Get the institution
            string institution = InstitutionDropDown.SelectedItem.Text;
            if (String.IsNullOrWhiteSpace(institution))
            {
                InstitutionLabel.Text = "Required";
                valid = false;
            }
            else
            {
                InstitutionLabel.Text = String.Empty;
            }

            // Get the material type
            string material_type = MaterialTypeDropDown.SelectedItem.Text;
            if (String.IsNullOrWhiteSpace(material_type))
            {
                MaterialTypeLabel.Text = "Required";
                valid = false;
            }
            else
            {
                MaterialTypeLabel.Text = String.Empty;
            }

            // Validate the drop downs for the item
            if (!valid)
            {
                FormError.Text = "Item information is incomplete";
                return;
            }

            // Since the rest of the form was valid get the numbers

            // Get the number of trayed items
            int trayed = 0;
            if (!String.IsNullOrWhiteSpace(TrayedItemsTextBox.Text))
            {
                if (!Int32.TryParse(TrayedItemsTextBox.Text, out trayed))
                {
                    valid = false;
                    TrayedItemsLabel.Text = "Invalid";
                }
                else
                {
                    TrayedItemsLabel.Text = String.Empty;
                }
            }
            else
            {
                TrayedItemsLabel.Text = String.Empty;
            }

            // Get the number of withdrawn/duped items
            int withdrawn = 0;
            if (!String.IsNullOrWhiteSpace(WithdrawnItemsTextBox.Text))
            {
                if (!Int32.TryParse(WithdrawnItemsTextBox.Text, out withdrawn))
                {
                    valid = false;
                    WithdrawnItemsLabel.Text = "Invalid";
                }
                else
                {
                    WithdrawnItemsLabel.Text = String.Empty;
                }
            }
            else
            {
                WithdrawnItemsLabel.Text = String.Empty;
            }

            // Get the number of damaged items
            int damaged = 0;
            if (!String.IsNullOrWhiteSpace(DamagedItemsTextBox.Text))
            {
                if (!Int32.TryParse(DamagedItemsTextBox.Text, out damaged))
                {
                    valid = false;
                    DamagedItemsLabel.Text = "Invalid";
                }
                else
                {
                    DamagedItemsLabel.Text = String.Empty;
                }
            }
            else
            {
                DamagedItemsLabel.Text = String.Empty;
            }

            // If this is invalid return
            if (!valid)
            {
                FormError.Text = "Invalid item information";
                return;
            }

            // If no item count entered
            if ((trayed == 0) && (withdrawn == 0) && (damaged == 0))
            {
                FormError.Text = "Missing item information";
                return;
            }

            // Save any updates to the item work info
            save_itemwork_set();

            // Since this was valid, clear any previous error
            FormError.Text = String.Empty;



            // Create the item work object
            ItemWork newWork = new ItemWork();
            newWork.DateAdded = DateTime.Now;
            newWork.Institution = new InstitutionInfo(int.Parse(InstitutionDropDown.SelectedValue), institution, institution);
            newWork.MaterialType = new MaterialTypeInfo(int.Parse(MaterialTypeDropDown.SelectedValue), material_type);
            newWork.Worker = currentUser.DisplayName;
            newWork.ItemsSentToTray = trayed;
            newWork.ItemsDamaged = damaged;
            newWork.ItemsWithdrawn = withdrawn;

            // Set this as the new user default too
            currentUser.Recents.LastAdded.Institution = institution.Trim();
            currentUser.Recents.LastAdded.MaterialType = material_type.Trim();



            // Add to the database
            if (DatabaseGateway.Save_ItemWork_Info(newWork, titleInfo.PrimaryKey))
            {
                titleInfo.Items.Add(newWork);

                // Save to the session now too
                Session["CurrentWork"] = titleInfo;

                // Clear the data from the item area
                TrayedItemsTextBox.Text = String.Empty;
                WithdrawnItemsTextBox.Text = String.Empty;
                DamagedItemsTextBox.Text = String.Empty;
            }
            else
            {
                FormError.Text = "Error saving the item work to the database";
            }

            // Show the item info again
            show_itemwork();
        }


        private void show_itemwork()
        {
            // Ensure the collections (lookup by primary key) are built
            if (editItemsButtons == null) editItemsButtons = new Dictionary<string, LinkButton>();
            if (deleteItemsButtons == null) deleteItemsButtons = new Dictionary<string, LinkButton>();
            if (saveItemsButtons == null) saveItemsButtons = new Dictionary<string, LinkButton>();
            if (cancelItemsButtons == null) cancelItemsButtons = new Dictionary<string, LinkButton>();
            if (itemsTrayedBoxes == null) itemsTrayedBoxes = new Dictionary<string, TextBox>();
            if (itemsTrayedLabels == null) itemsTrayedLabels = new Dictionary<string, Label>();
            if (itemsWithdrawnBoxes == null) itemsWithdrawnBoxes = new Dictionary<string, TextBox>();
            if (itemsWithdrawnLabels == null) itemsWithdrawnLabels = new Dictionary<string, Label>();
            if (itemsDamagedBoxes == null) itemsDamagedBoxes = new Dictionary<string, TextBox>();
            if (itemsDamagedLabels == null) itemsDamagedLabels = new Dictionary<string, Label>();
            if (itemsRows == null) itemsRows = new Dictionary<string, TableRow>();

            if (titleInfo == null) 
            {
                ItemInfoPanel.Visible = false;
                return;
            }

            #region Add the list of all the items linked to this title

            if (titleInfo.Items.Count != 0)
            {
                ItemInfoPanel.Visible = true;

                // Clear the old rows
             //   ItemInfoTable.Rows.Clear();

                // Add the header row
                TableHeaderRow headerRow = new TableHeaderRow();
                headerRow.Cells.Add(new TableHeaderCell { Text = "Date" });
                headerRow.Cells.Add(new TableHeaderCell { Text = "Worker" });
                headerRow.Cells.Add(new TableHeaderCell { Text = "Institution" });
                headerRow.Cells.Add(new TableHeaderCell { Text = "Type" });
                headerRow.Cells.Add(new TableHeaderCell { Text = "Trayed" });
                headerRow.Cells.Add(new TableHeaderCell { Text = "Withdrawn" });
                headerRow.Cells.Add(new TableHeaderCell { Text = "Damaged" });
                headerRow.Cells.Add(new TableHeaderCell { Text = "Actions" });
                ItemInfoTable.Rows.Add(headerRow);


                // Add each row (and keep track of the total)
                int trayed = 0;
                int withdrawn = 0;
                int damaged = 0;
                
                foreach (ItemWork thisWork in titleInfo.Items)
                {
                    // Add this row
                    ItemInfoTable.Rows.Add(Build_Table_Row(thisWork));

                    // Determine the total counts                
                    trayed += thisWork.ItemsSentToTray;
                    withdrawn += thisWork.ItemsWithdrawn;
                    damaged += thisWork.ItemsDamaged;
                }


                // Add a TOTAL row
                ItemInfoTable.Rows.Add(new TableRow());
                TableRow totalRow = ItemInfoTable.Rows[ItemInfoTable.Rows.Count - 1];
                totalRow.CssClass = "item_work_table_totalrow";

                // Add data to each column
                TableCell totalCell = new TableCell();
                totalCell.ColumnSpan = 4;
                totalCell.Text = "TOTAL";
                totalRow.Cells.Add(totalCell);

                totalRow.Cells.Add(new TableCell { Text = trayed.ToString() });
                totalRow.Cells.Add(new TableCell { Text = withdrawn.ToString() });
                totalRow.Cells.Add(new TableCell { Text = damaged.ToString() });
                totalRow.Cells.Add(new TableCell { Text = String.Empty });

            }
            else
            {
                ItemInfoPanel.Visible = false;
            }
            

            #endregion
        }

        private TableRow Build_Table_Row( ItemWork thisWork )
        {
            // Create a row for this table
            TableRow thisRow = new TableRow();
            itemsRows[thisWork.PrimaryKey.ToString()] = thisRow;

            // Set color based on status
            string background_color = "#FFFFFF";
            if ((thisWork.DateSubmitted.HasValue) && (!thisWork.DateApproved.HasValue))
                background_color = "#FFFFCC";
            if (!thisWork.DateSubmitted.HasValue)
                background_color = "#FFCCFF";
            thisRow.Style.Add("background-color", background_color);

            // Add data to each column
            thisRow.Cells.Add(new TableCell { Text = thisWork.DateAdded.ToShortDateString() });
            thisRow.Cells.Add(new TableCell { Text = thisWork.Worker });
            thisRow.Cells.Add(new TableCell { Text = thisWork.Institution.Code });
            thisRow.Cells.Add(new TableCell { Text = thisWork.MaterialType.Text });

            // Start the cell for the item count trayed
            TableCell itemsSentToTrayCell = new TableCell();
            thisRow.Cells.Add(itemsSentToTrayCell);

            // Add the editable textbox for the items sent to tray
            TextBox itemsSentToTrayTextBox = new TextBox();
            itemsSentToTrayTextBox.ID = thisWork.PrimaryKey + "_itemsSentToTrayTextBox";
            itemsSentToTrayTextBox.Text = thisWork.ItemsSentToTray.ToString();
            itemsSentToTrayTextBox.CssClass = "item_work_table_input";
            itemsSentToTrayTextBox.Visible = false;
            itemsSentToTrayCell.Controls.Add(itemsSentToTrayTextBox);
            itemsTrayedBoxes[thisWork.PrimaryKey.ToString()] = itemsSentToTrayTextBox;

            // Add the label for items sent to tray
            Label itemsSentToTrayLabel = new Label();
            itemsSentToTrayLabel.ID = thisWork.PrimaryKey + "_itemsSentToTrayLabel";
            itemsSentToTrayLabel.Text = thisWork.ItemsSentToTray.ToString();
            itemsSentToTrayCell.Controls.Add(itemsSentToTrayLabel);
            itemsTrayedLabels[thisWork.PrimaryKey.ToString()] = itemsSentToTrayLabel;

            // Add the cell for the items withdrawn
            TableCell itemsWithdrawnCell = new TableCell();
            thisRow.Cells.Add(itemsWithdrawnCell);

            // Add the editable textbox for the items withdrawn
            TextBox itemsWithdrawnTextBox = new TextBox();
            itemsWithdrawnTextBox.ID = thisWork.PrimaryKey + "_itemsWithdrawnTextBox";
            itemsWithdrawnTextBox.Text = thisWork.ItemsWithdrawn.ToString();
            itemsWithdrawnTextBox.CssClass = "item_work_table_input";
            itemsWithdrawnTextBox.Visible = false;
            itemsWithdrawnCell.Controls.Add(itemsWithdrawnTextBox);
            itemsWithdrawnBoxes[thisWork.PrimaryKey.ToString()] = itemsWithdrawnTextBox;

            // Add the label for items withdrawn
            Label itemsWithdrawnLabel = new Label();
            itemsWithdrawnLabel.ID = thisWork.PrimaryKey + "_itemsWithdrawnLabel";
            itemsWithdrawnLabel.Text = thisWork.ItemsWithdrawn.ToString();
            itemsWithdrawnCell.Controls.Add(itemsWithdrawnLabel);
            itemsWithdrawnLabels[thisWork.PrimaryKey.ToString()] = itemsWithdrawnLabel;

            // Add the cell for the items damaged
            TableCell itemsDamagedCell = new TableCell();
            thisRow.Cells.Add(itemsDamagedCell);

            // Add the editable textbox for the items damaged
            TextBox itemsDamagedTextBox = new TextBox();
            itemsDamagedTextBox.ID = thisWork.PrimaryKey + "_itemsDamagedTextBox";
            itemsDamagedTextBox.Text = thisWork.ItemsDamaged.ToString();
            itemsDamagedTextBox.CssClass = "item_work_table_input";
            itemsDamagedTextBox.Visible = false;
            itemsDamagedCell.Controls.Add(itemsDamagedTextBox);
            itemsDamagedBoxes[thisWork.PrimaryKey.ToString()] = itemsDamagedTextBox;

            // Add the label for items damaged
            Label itemsDamagedLabel = new Label();
            itemsDamagedLabel.ID = thisWork.PrimaryKey + "_itemsDamagedLabel";
            itemsDamagedLabel.Text = thisWork.ItemsDamaged.ToString();
            itemsDamagedCell.Controls.Add(itemsDamagedLabel);
            itemsDamagedLabels[thisWork.PrimaryKey.ToString()] = itemsDamagedLabel;


            // Add the links (if can be edited)
            bool canBeEdited = false;
            if (thisWork.Worker == currentUser.DisplayName)
                canBeEdited = true;
            if (canBeEdited)
            {
                TableCell actionsCell = new TableCell();

                // Add the edit buttons
                LinkButton editButton = new LinkButton();
                editButton.ID = thisWork.PrimaryKey + "_editButton";
                editButton.Text = "E";
                editButton.Click += EditButton_Click;
                actionsCell.Controls.Add(editButton);
                editItemsButtons[thisWork.PrimaryKey.ToString()] = editButton;

                // Add the trigger to to the AJAX panel for the edit button
                AsyncPostBackTrigger trigger_edit = new AsyncPostBackTrigger();
                trigger_edit.ControlID = editButton.UniqueID;
                trigger_edit.EventName = "Click";
                MainFormPanel.Triggers.Add(trigger_edit);
                MainScriptManager.RegisterAsyncPostBackControl(editButton);

                // Add a spacer
                actionsCell.Controls.Add(new Literal { Text = " &nbsp; " });

                // Add the delete button
                LinkButton deleteButton = new LinkButton();
                deleteButton.ID = thisWork.PrimaryKey + "_deleteButton";
                deleteButton.Text = "D";
                deleteButton.Click += DeleteButton_Click;
                deleteButton.OnClientClick = "if (!UserDeleteConfirmation()) return false;";
                actionsCell.Controls.Add(deleteButton);
                deleteItemsButtons[thisWork.PrimaryKey.ToString()] = deleteButton;

                // Add the trigger to the AJAX panel for the delete button
                AsyncPostBackTrigger trigger_delete = new AsyncPostBackTrigger();
                trigger_delete.ControlID = deleteButton.UniqueID;
                trigger_delete.EventName = "Click";
                MainFormPanel.Triggers.Add(trigger_delete);
                MainScriptManager.RegisterAsyncPostBackControl(deleteButton);

                // Add the save button
                LinkButton saveButton = new LinkButton();
                saveButton.ID = thisWork.PrimaryKey + "_saveButton";
                saveButton.Text = "S";
                saveButton.Click += SaveButton_Click;
                saveButton.Visible = false;
                actionsCell.Controls.Add(saveButton);
                saveItemsButtons[thisWork.PrimaryKey.ToString()] = saveButton;

                // Add the trigger to the AJAX panel for the delete button
                AsyncPostBackTrigger trigger_save = new AsyncPostBackTrigger();
                trigger_save.ControlID = saveButton.UniqueID;
                trigger_save.EventName = "Click";
                MainFormPanel.Triggers.Add(trigger_save);
                MainScriptManager.RegisterAsyncPostBackControl(saveButton);

                // Add a spacer
                actionsCell.Controls.Add(new Literal { Text = " &nbsp; " });

                // Add the cancel button
                LinkButton cancelButton = new LinkButton();
                cancelButton.ID = thisWork.PrimaryKey + "_cancelButton";
                cancelButton.Text = "C";
                cancelButton.Click += CancelButton_Click;
                cancelButton.Visible = false;
                actionsCell.Controls.Add(cancelButton);
                cancelItemsButtons[thisWork.PrimaryKey.ToString()] = cancelButton;

                // Add the trigger to the AJAX panel for the cancel button
                AsyncPostBackTrigger trigger_cancel = new AsyncPostBackTrigger();
                trigger_cancel.ControlID = cancelButton.UniqueID;
                trigger_cancel.EventName = "Click";
                MainFormPanel.Triggers.Add(trigger_cancel);
                MainScriptManager.RegisterAsyncPostBackControl(cancelButton);

                thisRow.Cells.Add(actionsCell);
            }
            else
            {
                thisRow.Cells.Add(new TableCell { Text = String.Empty });
            }

            return thisRow;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            // Find the related controls
            LinkButton cancelButton = (LinkButton)sender;
            string primarykey = cancelButton.ID.Replace("_cancelButton", "");

            // Copy the text from the labels back onto the textboxes
            itemsTrayedBoxes[primarykey].Text = itemsTrayedLabels[primarykey].Text;
            itemsWithdrawnBoxes[primarykey].Text = itemsWithdrawnLabels[primarykey].Text;
            itemsDamagedBoxes[primarykey].Text = itemsDamagedLabels[primarykey].Text;

            // Show the edit and delete buttons
            editItemsButtons[primarykey].Visible = true;
            deleteItemsButtons[primarykey].Visible = true;

            // Hide the save and cancel buttons
            saveItemsButtons[primarykey].Visible = false;
            cancelItemsButtons[primarykey].Visible = false;

            // Show the labels
            itemsTrayedLabels[primarykey].Visible = true;
            itemsWithdrawnLabels[primarykey].Visible = true;
            itemsDamagedLabels[primarykey].Visible = true;

            // Hide the boxes
            itemsTrayedBoxes[primarykey].Visible = false;
            itemsWithdrawnBoxes[primarykey].Visible = false;
            itemsDamagedBoxes[primarykey].Visible = false;

        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Find the related controls
            LinkButton saveButton = (LinkButton)sender;
            string primarykey = saveButton.ID.Replace("_saveButton", "");

            // Use a flag to determine validity
            bool validity = false;

            // Get the new values
            int trayed_count = 0;
            int withdrawn_count = 0;
            int damaged_count = 0;
            string new_trayed_value = itemsTrayedBoxes[primarykey].Text;
            string new_withdrawn_value = itemsWithdrawnBoxes[primarykey].Text;
            string new_damaged_value = itemsDamagedBoxes[primarykey].Text;
            if ((Int32.TryParse(new_trayed_value, out trayed_count)) && (Int32.TryParse(new_withdrawn_value, out withdrawn_count)) && (Int32.TryParse(new_damaged_value, out damaged_count)))
            {
                // Values are apparently valid.. check one more thing
                if ((trayed_count >= 0) && (withdrawn_count >= 0) && (damaged_count >= 0))
                    validity = true;
            }

            // Get the object to delete
            ItemWork toEdit = null;
            foreach (ItemWork thisWork in titleInfo.Items)
            {
                if (thisWork.PrimaryKey.ToString() == primarykey)
                {
                    toEdit = thisWork;
                    break;
                }
            }

            // Ensure one found
            if (toEdit == null)
                validity = false;

            // Were these valid?
            if (validity)
            {
                // Try to update
                ItemWork newWork = new ItemWork();
                newWork.PrimaryKey = toEdit.PrimaryKey;
                newWork.Worker = toEdit.Worker;
                newWork.ItemsSentToTray = trayed_count;
                newWork.ItemsWithdrawn = withdrawn_count;
                newWork.ItemsDamaged = damaged_count;

                if (!DatabaseGateway.Save_ItemWork_Info(newWork, currentWorkSet.TitleID))
                {
                    validity = false;
                }
                else
                {
                    // Save the new values to the object
                    toEdit.ItemsSentToTray = trayed_count;
                    toEdit.ItemsWithdrawn = withdrawn_count;
                    toEdit.ItemsDamaged = damaged_count;


                    // Save the changes to the session object
                    Session["CurrentWork"] = titleInfo;

                    // Update the labels
                    itemsTrayedLabels[primarykey].Text = trayed_count.ToString();
                    itemsWithdrawnLabels[primarykey].Text = withdrawn_count.ToString();
                    itemsDamagedLabels[primarykey].Text = damaged_count.ToString();

                }
            }

            // Show the edit and delete buttons again
            editItemsButtons[primarykey].Visible = true;
            deleteItemsButtons[primarykey].Visible = true;

            // Hide the save and cancel buttons
            saveItemsButtons[primarykey].Visible = false;
            cancelItemsButtons[primarykey].Visible = false;

            // Show the labels
            itemsTrayedLabels[primarykey].Visible = true;
            itemsWithdrawnLabels[primarykey].Visible = true;
            itemsDamagedLabels[primarykey].Visible = true;

            // Hide the boxes
            itemsTrayedBoxes[primarykey].Visible = false;
            itemsWithdrawnBoxes[primarykey].Visible = false;
            itemsDamagedBoxes[primarykey].Visible = false;

            // Update the boxes to remove any unaccepted changes and clear spaces, etc..
            itemsTrayedBoxes[primarykey].Text = itemsTrayedLabels[primarykey].Text;
            itemsWithdrawnBoxes[primarykey].Text = itemsWithdrawnLabels[primarykey].Text;
            itemsDamagedBoxes[primarykey].Text = itemsDamagedLabels[primarykey].Text;

        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            // Find the related controls
            LinkButton deleteButton = (LinkButton)sender;
            string primarykey = deleteButton.ID.Replace("_deleteButton", "");

            // Get the object to delete
            ItemWork toDelete = null;
            foreach (ItemWork thisWork in titleInfo.Items)
            {
                if (thisWork.PrimaryKey.ToString() == primarykey)
                {
                    toDelete = thisWork;
                    break;
                }
            }

            // If this found the item, try to delete it
            if (toDelete != null)
            {
                // First, try to delete from the database
                if (!DatabaseGateway.Delete_ItemWork_Info(toDelete.PrimaryKey))
                    return;

                // If that was successful, remove from the object
                titleInfo.Items.Remove(toDelete);

                // Save the changes to the session object
                Session["CurrentWork"] = titleInfo;
            }

            // Also remove this row from the UI
            TableRow thisRow = itemsRows[primarykey];
            ItemInfoTable.Rows.Remove(thisRow);

        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            // Find the related controls
            LinkButton editButton = (LinkButton)sender;
            string primarykey = editButton.ID.Replace("_editButton", "");

 
            // Hide the edit and delete buttons
            editButton.Visible = false;
            deleteItemsButtons[primarykey].Visible = false;

            // Show the save and cancel buttons
            saveItemsButtons[primarykey].Visible = true;
            cancelItemsButtons[primarykey].Visible = true;

            // Hide the labels
            itemsTrayedLabels[primarykey].Visible = false;
            itemsWithdrawnLabels[primarykey].Visible = false;
            itemsDamagedLabels[primarykey].Visible = false;

            // Show the boxes
            itemsTrayedBoxes[primarykey].Visible = true;
            itemsWithdrawnBoxes[primarykey].Visible = true;
            itemsDamagedBoxes[primarykey].Visible = true;
        }

        protected void TitleButton_Click(object sender, EventArgs e)
        {
            save_itemwork_set();

            Response.Redirect("TitleEntry.aspx");
        }

        private bool item_count_editing()
        {
            if (titleInfo.Items == null)
                return false;

            foreach( ItemWork itemWork in titleInfo.Items )
            {
                if (( itemsTrayedBoxes.ContainsKey(itemWork.PrimaryKey.ToString())) && (itemsTrayedBoxes[itemWork.PrimaryKey.ToString()].Visible))
                {
                    FormError.Text = "Finish editing a row in the item table, either by cancelling or saving.";
                    return true;
                }               
            }

            return false;            
        }

        protected void CompleteButton_Click(object sender, EventArgs e)
        {
            if (item_count_editing())
                return;

            save_itemwork_set();

            Session["CurrentWork"] = null;
            Session["CurrentWorkSet_" + titleInfo.PrimaryKey] = null;
            Response.Redirect("Default.aspx");
        }

        protected void SubmitToQcButton_Click(object sender, EventArgs e)
        {
            save_itemwork_set();

            string returnMessage = DatabaseGateway.Submit_For_QC(titleInfo.PrimaryKey, currentUser.PrimaryKey);

            if (returnMessage.IndexOf("Success", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Session["CurrentWork"] = null;
                Session["CurrentWorkSet_" + titleInfo.PrimaryKey] = null;
                Response.Redirect("Default.aspx");
                return;
            }

            // Since this was valid, clear any previous error
            FormError.Text = returnMessage;
        }

        protected void LastCopyCheckBox_OnCheckedChanged(object Sender, EventArgs E)
        {
            LastCopyInstitutionBox.Visible = LastCopyCheckBox.Checked;
        }

        protected void ItemHol_None_OnCheckedChanged(object Sender, EventArgs E)
        {
            NewItemPanel.Visible = false;
            ItemHol_EditCount.Visible = false;
            ItemHol_EditCountLabel.Visible = false;
            save_itemwork_set();
        }

        protected void ItemHol_Process_OnCheckedChanged(object Sender, EventArgs E)
        {
            NewItemPanel.Visible = true;
            ItemHol_EditCount.Visible = false;
            ItemHol_EditCountLabel.Visible = false;
            save_itemwork_set();
        }

        protected void ItemHol_Edit_OnCheckedChanged(object Sender, EventArgs E)
        {
            NewItemPanel.Visible = false;
            ItemHol_EditCount.Visible = true;
            ItemHol_EditCountLabel.Visible = true;
            save_itemwork_set();
        }

        protected void NotesTextBox_OnTextChanged(object Sender, EventArgs E)
        {
            save_itemwork_set();
        }

        protected void General_OnCheckedChanged(object Sender, EventArgs E)
        {
            save_itemwork_set();
        }

        private void save_itemwork_set()
        {
            // Since there is no workset yet, check for institution and material type
            int institutionid = int.Parse(InstitutionDropDown.SelectedValue);
            int materialtypeid = int.Parse(MaterialTypeDropDown.SelectedValue);

            // Do we have the primary key for this workset?
            if ((currentWorkSet == null) || (currentWorkSet.PrimaryKey < 0))
            {
                // Neither can be -1
                if ((institutionid < 0) || (materialtypeid < 0))
                {
                    if (institutionid < 0)
                    {
                        InstitutionLabel.Text = "Required";
                        InstitutionLabel.Visible = true;
                    }
                    else
                    {
                        InstitutionLabel.Visible = false;
                    }

                    if (materialtypeid < 0)
                    {
                        MaterialTypeLabel.Text = "Required";
                        MaterialTypeLabel.Visible = true;
                    }
                    else
                    {
                        MaterialTypeLabel.Visible = false;
                    }

                    return;
                }
                else
                {
                    InstitutionLabel.Visible = false;
                    MaterialTypeLabel.Visible = false;
                }

                int new_id = DatabaseGateway.Create_ItemWorkSet(currentUser.PrimaryKey, titleInfo.PrimaryKey, institutionid, materialtypeid);
                currentWorkSet = new WorkSet();
                currentWorkSet.PrimaryKey = new_id;
            }

            // Save all the values to the object
            currentWorkSet.ProcessorNotes = NotesTextBox.Text;
            currentWorkSet.ItemHol_EditCount = 0;
            if (ItemHol_None.Checked) currentWorkSet.ItemHolActionType = null;
            if (ItemHol_Process.Checked) currentWorkSet.ItemHolActionType = new ItemHolActionTypeInfo { ID = 1, ActionType = "Process" };
            if (ItemHol_Edit.Checked)
            {
                currentWorkSet.ItemHolActionType = new ItemHolActionTypeInfo { ID = 2, ActionType = "Edit" };
                int test_count;
                if (Int32.TryParse(ItemHol_EditCount.Text, out test_count))
                {
                    currentWorkSet.ItemHol_EditCount = test_count;
                }
            }

            // Look for the PCC category checked radio button
            currentWorkSet.PccCategory = null;
            currentWorkSet.Pcc_Maintenance = false;
            currentWorkSet.Pcc_NewAuthentication = false;
            string pcc = PccCategoryMainDropDown.SelectedItem.Text;
            string pcc_finder = pcc;
            if (pccNewButton.Checked)
            {
                pcc_finder = pcc_finder + "|new";
                currentWorkSet.Pcc_NewAuthentication = true;
            }
            else if (pccMaintButton.Checked)
            {
                pcc_finder = pcc_finder + "|maintenance";
                currentWorkSet.Pcc_Maintenance = true;

            }
            foreach( PccCategoryInfo catInfo in ApplicationCache.PccCategoryTypes)
            {
                if ( catInfo.Category.Equals(pcc_finder, StringComparison.OrdinalIgnoreCase))
                {
                    currentWorkSet.PccCategory = new PccCategoryInfo { Category = pcc, ID = catInfo.ID };
                }
            }
            

            // Look for the cataloging type checked radio button
            currentWorkSet.CatalogingType = null;
            foreach (RadioButton catButton in catalogTypeButtons.Values)
            {
                if (catButton.Checked)
                {
                    currentWorkSet.CatalogingType = new CatalogingTypeInfo { Text = catButton.Text, ID = Int32.Parse(catButton.ID.Replace("CatType", "")) };
                }
            }

            // Get the primary keys
            int holid = (currentWorkSet.ItemHolActionType != null) ? currentWorkSet.ItemHolActionType.ID : -1;
            int pccid = (currentWorkSet.PccCategory != null) ? currentWorkSet.PccCategory.ID : -1;
            int catid = (currentWorkSet.CatalogingType != null) ? currentWorkSet.CatalogingType.ID : -1;

            // Get the last copy information
            bool lastcopy_flag = LastCopyCheckBox.Checked;
            string lastcopy_institution = String.Empty;
            if (lastcopy_flag)
            {
                lastcopy_institution = LastCopyInstitutionBox.Text.Trim();
            }

            // Save to the database
            DatabaseGateway.Save_ItemWorkSet(currentWorkSet.PrimaryKey, institutionid, materialtypeid, holid, pccid, catid, currentWorkSet.ProcessorNotes, lastcopy_flag, lastcopy_institution, currentWorkSet.ItemHol_EditCount);

            // If the user has access to the authority stuff, deal with that
            if (AuthorityCatalogingTable.Visible)
            {
                // Get the authority work from the UI
                currentWorkSet.AuthorityWork.Clear();
                for (int i = 0; i < authority_rows_existing; i++)
                {
                    int id = int.Parse(authTypes[i].SelectedItem.Value);

                    if (id > 0)
                    {
                        string type = authTypes[i].SelectedItem.Text;
                        bool originalwork = (authActions[i].SelectedItem.Value == "original");

                        currentWorkSet.AuthorityWork.Add(new WorkSet_AuthorityWork(id, type, originalwork));
                    }
                }

                // Save the new set to the database
                DatabaseGateway.Clear_ItemWorkSet_Authority(currentWorkSet.PrimaryKey);
                foreach(WorkSet_AuthorityWork work in currentWorkSet.AuthorityWork)
                {
                    DatabaseGateway.Add_ItemWorkSet_Authority(currentWorkSet.PrimaryKey, work.AuthorityRecordTypeID, work.OriginalWork);
                }

            }

            // Save to the session
            Session["CurrentWorkSet_" + titleInfo.PrimaryKey] = currentWorkSet;
        }


        protected void PccCategoryMain_OnSelectedChanged(object Sender, EventArgs E)
        {
            if (PccCategoryMainDropDown.SelectedValue != "-1")
            {
                PccCategoryPlaceHolder.Visible = true;
            }
            else
            {
                PccCategoryPlaceHolder.Visible = false;
            }
        }

        protected void AddAnotherAuthButton_Click(object sender, EventArgs e)
        {
            // Add this row
            add_authority_row(authority_rows_existing + 1, true);

            // Increase the count on the rows
            AuthorityRowsCount.Value = (authority_rows_existing + 1).ToString();

        }

        private void add_authority_row(int count, bool set_default)
        {
            // Start this row
            TableRow newRow = new TableRow();
            newRow.ID = "AuthRow" + count;

            // Add the first cell
            TableCell firstCell = new TableCell();
            firstCell.ID = "TypeCell" + count;
            newRow.Cells.Add(firstCell);

            // Add the drop down
            DropDownList typeDropDown = new DropDownList();
            typeDropDown.ID = "Authority_Type" + count;
            firstCell.Controls.Add(typeDropDown);

            // Populate this - Add the authority catalog types and such
            typeDropDown.Items.Add(new ListItem(String.Empty, "-1"));
            foreach (var authCatType in ApplicationCache.AuthorityRecordTypes)
            {
                typeDropDown.Items.Add(new ListItem(authCatType.RecordType, authCatType.ID.ToString()));
            }

            // Start the second cell
            TableCell secondCell = new TableCell();
            secondCell.ID = "ActionCell" + count;
            newRow.Cells.Add(secondCell);

            // Create the radio button list
            RadioButtonList actionList = new RadioButtonList();
            actionList.ID = "Authority_Action" + count;
            actionList.RepeatDirection = RepeatDirection.Horizontal;
            actionList.Items.Add(new ListItem("Original", "original"));
            actionList.Items.Add(new ListItem("Edit", "edit"));
            if (set_default)
                actionList.SelectedIndex = 0;
            secondCell.Controls.Add(actionList);
                       
            // Add the final cell
            TableCell thirdCell = new TableCell();
            newRow.Cells.Add(thirdCell);

            // Move the button here
            thirdCell.Controls.Add(AddAnotherAuthButton);


            // Add this row to the table
            AuthParametersTable.Rows.Add(newRow);

            // Save these controls
            authTypes.Add(typeDropDown);
            authActions.Add(actionList);
        }
    }
}