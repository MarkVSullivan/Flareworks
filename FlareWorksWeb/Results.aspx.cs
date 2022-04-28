using FlareWorks.Library.Models.Search;
using FlareWorks.Library.Search;
using FlareWorks.Models.Users;
using System;
using System.Data;
using FlareworksWeb.UserMgmt;

namespace FlareworksWeb
{
    public partial class Results : System.Web.UI.Page
    {
        private UserInfo currentUser;
        private SearchInfo search;
        private DataTable results;

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

            // Take the query options and find the search
            search = new SearchInfo(Request.QueryString);

            // Perform the search
            //if ((search.CriteriaCount > 0) || ( search.DateRange_Start.HasValue ) || ( search.DateRange_End.HasValue ))
            //{
                results = SearchHelper.PerformSearch(search);
            //}

            if (results == null )
            {
                Response.Redirect("AdminSearch.aspx");
            }

        }


        protected void Add_Results_Count()
        {
            if ( results == null )
            {
                Response.Output.Write("NULL VALUE");
            }
            else
            {
                Response.Output.Write(results.Rows.Count + " RESULTS" );
            }
 
            
        }

        protected void Add_Results_Table()
        {
            Response.Output.WriteLine("<table id=\"results_table\">");
            Response.Output.Write("<thead><tr>");
            foreach (DataColumn cols in results.Columns)
            {
                Response.Output.Write("<th>" + cols.ColumnName.Replace("_", " ") + "</th>");
            }
            Response.Output.Write("</tr>");
            Response.Output.WriteLine("</thead>");

            Response.Output.WriteLine("<tbody>");
            foreach (DataRow thisRow in results.Rows)
            {
                Response.Output.Write("<tr>");

                foreach (object value in thisRow.ItemArray)
                {
                    Response.Output.Write("<td>" + value.ToString() + "</td>");
                }

                Response.Output.WriteLine("</tr>");
            }

            Response.Output.WriteLine("</tbody>");

            Response.Output.WriteLine("</table>");
        }

        protected void Add_DataTable_Script()
        {
            Response.Output.WriteLine("<script type=\"text/javascript\" >");
            Response.Output.WriteLine("    $(document).ready(function() {");
            Response.Output.WriteLine("        var table = $('#results_table').DataTable({");
            Response.Output.WriteLine("            \"searching\": false, ");
            Response.Output.WriteLine("            \"lengthMenu\": [ [50, 100, -1], [50, 100, \"All\"] ], ");
            Response.Output.WriteLine("            \"pageLength\":  50, ");  
            Response.Output.WriteLine("            \"order\":   [[ 0, \"desc\" ]] ");  //  Removed comma here 
            //Response.Output.WriteLine("            initComplete: function() {");
            //Response.Output.WriteLine("                var api = this.api();");
            //Response.Output.WriteLine("                api.columns().indexes().flatten().each(function(i)  {");
            //Response.Output.WriteLine("                    if ((i == 2 || i == 4 || i == 5 || i == 6))");
            //Response.Output.WriteLine("                    {");
            //Response.Output.WriteLine("                        var column = api.column(i);");
            //Response.Output.WriteLine("                        var select = $('<select><option value=\"\"></option></select>').appendTo( $(column.footer()).empty()).on('change', function() {");
            //Response.Output.WriteLine("                            var val = $.fn.dataTable.util.escapeRegex($(this).val());");
            //Response.Output.WriteLine("                            column.search(val ? '^' + val + '$' : '', true, false).draw();   } );");
            //Response.Output.WriteLine("                        column.data().unique().sort().each(function(d, j) { select.append('<option value=\"' + d + '\">' + d + '</option>')  } );");
            //Response.Output.WriteLine("                    }");
            //Response.Output.WriteLine("                } );");
            //Response.Output.WriteLine("            }");
            Response.Output.WriteLine("        });");
            Response.Output.WriteLine("        $('#adminMgmtCodeSearch').on( 'keyup change', function () { table.column(1).search(this.value).draw(); } );");
            Response.Output.WriteLine("        $('#adminMgmtNameSearch').on( 'keyup change', function () { table.column(3).search(this.value).draw(); } );");
            Response.Output.WriteLine("    } );");
            Response.Output.WriteLine("</script>");
            Response.Output.WriteLine();

        }

        protected void ReturnButton_Click(object sender, EventArgs e)
        {
            // Get the query string
            string query_string = search.QueryString;

            if (currentUser.Permissions.CanAdvancedSearch)
                Response.Redirect("AdminSearch.aspx" + query_string);
            else
                Response.Redirect("Search.aspx" + query_string);
        }
    }
}