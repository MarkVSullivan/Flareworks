using FlareWorks.Library.Models.ControlledValues;
using FlareWorks.Library.Models.Search;
using FlareWorks.MemoryMgmt;
using FlareWorks.Models.ControlledValues;
using FlareWorks.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FlareworksWeb.UserMgmt;

namespace FlareworksWeb
{
    public partial class AdminSearch : System.Web.UI.Page
    {
        private UserInfo currentUser;
        private int search_rows_existing;
        private List<DropDownList> searchFields;
        private List<TextBox> searchParametersUncontrolled;
        private List<DropDownList> searchParametersControlled;
        private SearchInfo search;

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
            if (!currentUser.Permissions.CanAdvancedSearch)
            {
                Response.Redirect("Search.aspx");
                return;
            }

            // If there is a query string here, there may be an existing search we are modifying
            Dictionary<string, SingleSearchCriterion> searchCriteria = new Dictionary<string, SingleSearchCriterion>();
            if ( Request.QueryString.Count > 0 )
            {
                search = new SearchInfo(Request.QueryString);
                foreach( SingleSearchCriterion criterion in search.Criteria )
                {
                    searchCriteria.Add(criterion.FieldCode.ToUpper(), criterion);
                }
            }

            // If this is the first time to this page, fill in the default filters
            if (!IsPostBack)
            {
                // Populate the worker/user drop down
                WorkerDropDown.Items.Add(new ListItem("", "0"));
                foreach (BriefWorkerInfo thisWorker in ApplicationCache.Workers)
                {
                    WorkerDropDown.Items.Add(new ListItem(thisWorker.Name, thisWorker.ID.ToString()));
                }

                // Is there an existing user/worker search criteria?
                if (( searchCriteria.ContainsKey("US")) && ( searchCriteria["US"].ControlledMatch > 0 ))
                {
                    WorkerDropDown.SelectedValue = searchCriteria["US"].ControlledMatch.ToString();
                    searchCriteria.Remove("US");
                }

                // Populate the location drop down
                LocationDropDown.Items.Add(new ListItem("", "0"));
                foreach (LocationInfo thisLocation in ApplicationCache.Locations)
                {
                    LocationDropDown.Items.Add(new ListItem(thisLocation.Code, thisLocation.ID.ToString()));
                }

                // Is there an existing location search criteria?
                if ((searchCriteria.ContainsKey("LO")) && (searchCriteria["LO"].ControlledMatch > 0))
                {
                    LocationDropDown.SelectedValue = searchCriteria["LO"].ControlledMatch.ToString();
                    searchCriteria.Remove("LO");
                }

                // Populate the institution drop down
                InstitutionDropDown.Items.Add(new ListItem("", "0"));
                foreach (InstitutionInfo thisInstitution in ApplicationCache.Institutions)
                {
                    InstitutionDropDown.Items.Add(new ListItem(thisInstitution.Code, thisInstitution.ID.ToString()));
                }

                // Is there an existing institution search criteria?
                if ((searchCriteria.ContainsKey("IN")) && (searchCriteria["IN"].ControlledMatch > 0))
                {
                    InstitutionDropDown.SelectedValue = searchCriteria["IN"].ControlledMatch.ToString();
                    searchCriteria.Remove("IN");
                }

                // Populate the form type drop down
                FormTypeDropDown.Items.Add(new ListItem("", "0"));
                foreach (RecordTypeInfo thisType in ApplicationCache.RecordTypes)
                {
                    FormTypeDropDown.Items.Add(new ListItem(thisType.Text, thisType.ID.ToString()));
                }

                // Is there an existing form type search criteria?
                if ((searchCriteria.ContainsKey("TY")) && (searchCriteria["TY"].ControlledMatch > 0))
                {
                    FormTypeDropDown.SelectedValue = searchCriteria["TY"].ControlledMatch.ToString();
                    searchCriteria.Remove("TY");
                }

                if ((search != null) && (!String.IsNullOrEmpty(search.Grouping)))
                {
                    if (search.Grouping.Contains('T'))
                        TitleGrouping.Checked = true;
                    if (search.Grouping.Contains('U'))
                        UsersGrouping.Checked = true;
                    if (search.Grouping.Contains('D'))
                        DateGrouping.Checked = true;
                
                }
            }

            // Build the lists to hold all the search parameter controls
            searchFields = new List<DropDownList>();
            searchParametersUncontrolled = new List<TextBox>();
            searchParametersControlled = new List<DropDownList>();

            // Add the initial (always there row) to the lists
            searchFields.Add(SearchField1);
            searchParametersUncontrolled.Add(SearchByText1);
            searchParametersControlled.Add(SearchControlled1);

            // Ensure the options exist on the very first field
            if ( !IsPostBack)
            {
                populate_search_options(SearchField1);
            }

            // Get the number of rows
            search_rows_existing = 1;
            if ( IsPostBack )
            {
                search_rows_existing = Convert.ToInt32(SearchRowsCount.Value);
            }
            else if ( searchCriteria.Count > 1 )
            {
                search_rows_existing = searchCriteria.Count;
            }

            // Build any rows that are needed
            for (int i = 2; i <= search_rows_existing; i++)
            {
                add_search_row(i, IsPostBack);
            }

            // Now, if there are search criteria from the query string in the URL, set those
            if (( !IsPostBack ) && ( searchCriteria.Count > 0 ))
            {
                int index = 0;
                foreach( string key in searchCriteria.Keys )
                {
                    // Get the next criterion
                    SingleSearchCriterion criterion = searchCriteria[key];

                    // Choose the field in the field drop down
                    searchFields[index].SelectedValue = criterion.FieldCode.ToUpper();

                    // Is this controlled or not?
                    if ( is_controlled(criterion.FieldCode.ToUpper()))
                    {
                        // Hide the text box and show the drop down list
                        searchParametersControlled[index].Visible = true;
                        searchParametersUncontrolled[index].Visible = false;

                        // Populate the controlled values in the drop down
                        populate_controlled_values(searchParametersControlled[index], criterion.FieldCode.ToUpper());

                        // Now, select the one in the criterion again
                        searchParametersControlled[index].SelectedValue = criterion.ControlledMatch.ToString();
                    }
                    else
                    {
                        // Just show the original entered text again
                        searchParametersUncontrolled[index].Text = criterion.Parameter.Trim();
                    }

                    index++;
                }
            }

        }

        private void add_search_row(int count, bool postback)
        {
            // Start this row
            TableRow newRow = new TableRow();
            newRow.ID = "SearchRow" + count;

            // Add the first cell
            TableCell firstCell = new TableCell();
            firstCell.ID = "FieldCell" + count;
            newRow.Cells.Add(firstCell);

            // Add the drop down
            DropDownList fieldDropDown = new DropDownList();
            fieldDropDown.ID = "SearchField" + count;
            fieldDropDown.TextChanged += SearchField_TextChanged;
            fieldDropDown.AutoPostBack = true;
            firstCell.Controls.Add(fieldDropDown);

            // Populate this
            populate_search_options(fieldDropDown);

            // Start the second cell
            TableCell secondCell = new TableCell();
            secondCell.ID = "ParameterCell" + count;
            newRow.Cells.Add(secondCell);

            // Add the free text box
            TextBox paramUncontrolledBox = new TextBox();
            paramUncontrolledBox.ID = "SearchByText" + count;
            secondCell.Controls.Add(paramUncontrolledBox);

            // Add the controlled entry (initially hidden)
            DropDownList paramControlledBox = new DropDownList();
            paramControlledBox.ID = "SearchControlled" + count;
            paramControlledBox.Visible = false;
            secondCell.Controls.Add(paramControlledBox);

            // Add the final cell
            TableCell thirdCell = new TableCell();
            newRow.Cells.Add(thirdCell);

            // Move the button here
            thirdCell.Controls.Add(AddAnotherSearchButton);


            // Add this row to the table
            SearchParametersTable.Rows.Add(newRow);

            // Save these controls
            searchFields.Add(fieldDropDown);
            searchParametersUncontrolled.Add(paramUncontrolledBox);
            searchParametersControlled.Add(paramControlledBox);
        }

        protected void populate_search_options(DropDownList SearchByDropDown)
        {
            if (SearchByDropDown.Items.Count == 0 )
            {
                SearchByDropDown.Items.Add(new ListItem("Aleph", "AL"));
                SearchByDropDown.Items.Add(new ListItem("Bibliographic Level", "BL"));
                SearchByDropDown.Items.Add(new ListItem("Cataloging Type", "CT"));
                SearchByDropDown.Items.Add(new ListItem("Document Type", "DT"));
                SearchByDropDown.Items.Add(new ListItem("Federal Agency", "FA"));
                SearchByDropDown.Items.Add(new ListItem("ISSN", "IS"));
                SearchByDropDown.Items.Add(new ListItem("Item Hol Action Type", "HO"));
                SearchByDropDown.Items.Add(new ListItem("OCLC", "OC"));
                SearchByDropDown.Items.Add(new ListItem("PCC Category", "PC"));          
                SearchByDropDown.Items.Add(new ListItem("Title", "TI"));
            }
        }

        private bool is_controlled( string FieldCode )
        {
            if ((FieldCode == "TI") || (FieldCode == "IS") || (FieldCode == "OC") || (FieldCode == "AL") || (FieldCode == "FA"))
                return false;

            return true;
        }

        private void populate_controlled_values(DropDownList ListBox, string FieldCode )
        {
            ListBox.Items.Clear();
            ListBox.Items.Add(new ListItem("", "0"));

            switch( FieldCode )
            {
                case "BL":
                    foreach(BibliographicLevelInfo bibInfo in ApplicationCache.BibliographicLevels)
                    {
                        ListBox.Items.Add(new ListItem(bibInfo.Level, bibInfo.ID.ToString()));
                    }
                    break;

                case "CT":
                    foreach (CatalogingTypeInfo catTypeInfo in ApplicationCache.CatalogingTypes)
                    {
                        ListBox.Items.Add(new ListItem(catTypeInfo.Text, catTypeInfo.ID.ToString()));
                    }
                    break;

                case "DT":
                    foreach (DocumentTypeInfo docInfo in ApplicationCache.DocumentTypes)
                    {
                        ListBox.Items.Add(new ListItem(docInfo.Text, docInfo.ID.ToString()));
                    }
                    break;

                case "HO":
                    foreach (ItemHolActionTypeInfo holInfo in ApplicationCache.ItemHolActionTypes)
                    {
                        ListBox.Items.Add(new ListItem(holInfo.ActionType, holInfo.ID.ToString()));
                    }
                    break;

                case "PC":
                    foreach (PccCategoryInfo pccInfo in ApplicationCache.PccCategoryTypes)
                    {
                        ListBox.Items.Add(new ListItem(pccInfo.Category, pccInfo.ID.ToString()));
                    }
                    break;
            }
        }

        protected void AddAnotherSearchButton_Click(object sender, EventArgs e)
        {
            // Add this row
            add_search_row(search_rows_existing + 1, false);

            // Increase the count on the rows
            SearchRowsCount.Value = (search_rows_existing + 1).ToString();
            
        }

        protected void SearchField_TextChanged(object sender, EventArgs e)
        {
            // Get the drop down list
            DropDownList senderList = (DropDownList) sender;

            // Get the ID that changed
            int row_number = Int32.Parse(senderList.ID.Replace("SearchField", ""));

            // Determine if this should be controlled, or uncontrolled
            bool controlled = is_controlled(senderList.Text);

            // Show or hide the text box and drop down
            if ( controlled )
            {
                // Hide the text box and show the drop down list
                searchParametersControlled[row_number - 1].Visible = true;
                searchParametersUncontrolled[row_number - 1].Visible = false;

                // Populate the controlled values
                populate_controlled_values(searchParametersControlled[row_number - 1], senderList.Text);
            }
            else
            {
                // Hide the drop down list and show the free text box
                searchParametersControlled[row_number - 1].Visible = false;
                searchParametersControlled[row_number - 1].Items.Clear();
                searchParametersUncontrolled[row_number - 1].Visible = true;
            }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            // Build the search object
            SearchInfo search = new SearchInfo();

            // Check for worker/user search criteria
            if (( WorkerDropDown.SelectedItem != null ) && ( WorkerDropDown.SelectedItem.Value != "0" ))
            {
                int workerid = Int32.Parse(WorkerDropDown.SelectedItem.Value);
                search.Add_Criteria("US", workerid);
            }

            // Check for location search criteria
            if ((LocationDropDown.SelectedItem != null) && (LocationDropDown.SelectedItem.Value != "0"))
            {
                int locid = Int32.Parse(LocationDropDown.SelectedItem.Value);
                search.Add_Criteria("LO", locid);
            }

            // Check for institution search criteria
            if ((InstitutionDropDown.SelectedItem != null) && (InstitutionDropDown.SelectedItem.Value != "0"))
            {
                int locid = Int32.Parse(InstitutionDropDown.SelectedItem.Value);
                search.Add_Criteria("IN", locid);
            }

            // Check for form type search criteria
            if ((FormTypeDropDown.SelectedItem != null) && (FormTypeDropDown.SelectedItem.Value != "0"))
            {
                int locid = Int32.Parse(FormTypeDropDown.SelectedItem.Value);
                search.Add_Criteria("TY", locid);
            }
            
            // Now, step through each search criteria row
            for( int i = 0; i < search_rows_existing; i++ )
            {
                // Get the field code
                string field_code = searchFields[i].Text;
                if ( is_controlled(field_code))
                {
                    // controlled search
                    if (( searchParametersControlled[i].SelectedItem != null ) && (searchParametersControlled[i].SelectedItem.Value != "0"))
                    {
                        int search_id = Int32.Parse(searchParametersControlled[i].SelectedItem.Value);
                        search.Add_Criteria(field_code, search_id);
                    }
                }
                else
                {
                    // free text search
                    if ( !String.IsNullOrWhiteSpace(searchParametersUncontrolled[i].Text))
                    {
                        string search_param = searchParametersUncontrolled[i].Text.Trim();
                        search.Add_Criteria(field_code, search_param);
                    }
                }
            }

            // Add any grouping
            search.Grouping = String.Empty;
            if (DateGrouping.Checked) search.Grouping = "D";
            if (TitleGrouping.Checked) search.Grouping = search.Grouping + "T";
            if (UsersGrouping.Checked) search.Grouping = search.Grouping + "U";


            // Add the date range, if there was some
            string date_start = DateRangeFirstDate.Text;
            string date_end = DateRangeEndDate.Text;
            DateTime testDate;
            if (DateTime.TryParse(date_start, out testDate))
                search.DateRange_Start = testDate;
            if (DateTime.TryParse(date_end, out testDate))
                search.DateRange_End = testDate;


            // Get the query options URL from the search object
            string query_options = search.QueryString;

            // Forward to the results URL
            Response.Redirect("Results.aspx" + query_options);

        }
    }
}