<%@ Page Title="Admin Search" Language="C#" MasterPageFile="~/FlareworksNavBar.Master" AutoEventWireup="true" CodeBehind="AdminSearch.aspx.cs" Inherits="FlareworksWeb.AdminSearch" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" Runat="Server">

</asp:Content>


<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h2>Administrator Search</h2>

   <asp:ScriptManager ID="MainScriptManager" runat="server" />
   <asp:UpdatePanel ID="MainFormPanel" runat="server">
       <ContentTemplate>
            <asp:HiddenField ID="SearchRowsCount" runat="server" value="1" />

            <fieldset>
                <legend><strong>Limit Search</strong></legend>
                <table class="search_display_table">
                    <tr>
                        <td style="width:100px;">Worker:</td>
                        <td style="width:300px;"><asp:DropDownList ID="WorkerDropDown" runat="server"></asp:DropDownList></td>
                        <td style="width:100px;">Location:</td>
                        <td><asp:DropDownList ID="LocationDropDown" runat="server"></asp:DropDownList></td>
                    </tr>
                    <tr>
                        <td>Form Type:</td>
                        <td><asp:DropDownList ID="FormTypeDropDown" runat="server" style="width:80px"></asp:DropDownList></td>
                        <td>Institution:</td>
                        <td><asp:DropDownList ID="InstitutionDropDown" runat="server" style="width:80px"></asp:DropDownList></td>  
                    </tr>
                </table>
            </fieldset>

            <fieldset>
                <legend><strong>Search Parameters</strong></legend>

                <table class="search_display_table">
                    <tr>
                        <td>Date Range:</td>
                        <td colspan="1">
                            <table>
                                <tr>
                                    <td><asp:TextBox ID="DateRangeFirstDate" runat="server" TextMode="Date"></asp:TextBox></td>
                                    <td>to</td>
                                    <td><asp:TextBox ID="DateRangeEndDate" runat="server" TextMode="Date"></asp:TextBox></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="vertical-align:top; margin-top: 10px;">Search By:</td>
                        <td>
                            <asp:table ID="SearchParametersTable" runat="server">
                                <asp:TableRow>
                                    <asp:TableCell><asp:DropDownList ID="SearchField1" runat="server" OnTextChanged="SearchField_TextChanged" AutoPostBack="True"></asp:DropDownList></asp:TableCell>
                                    <asp:TableCell>
                                        <asp:TextBox ID="SearchByText1" runat="server"></asp:TextBox>
                                        <asp:DropDownList ID="SearchControlled1" runat="server" Visible="false"></asp:DropDownList>
                                    </asp:TableCell>
                                    <asp:TableCell><asp:Button ID="AddAnotherSearchButton" runat="server" OnClick="AddAnotherSearchButton_Click" Text="Add" CssClass="add_search_button"></asp:Button></asp:TableCell>
                                </asp:TableRow>
                            </asp:table>
                        </td>
                    </tr>
                </table>
            </fieldset>

            <fieldset>
                <legend><strong>Group Results</strong></legend>
                <table  class="search_display_table" style="width: 700px;">
                    <tr>
                        <td><asp:CheckBox ID="TitleGrouping" runat="server" Value="T" Text="Titles" AutoPostBack="True" Width="70px" /></td>
                        <td><asp:CheckBox ID="UsersGrouping" runat="server" Value="U" Text="Users" AutoPostBack="True" Width="70px" /></td>
                        <td><asp:CheckBox ID="DateGrouping" runat="server" Value="D" Text="Date" AutoPostBack="True" /></td>
                    </tr>
                </table>
            </fieldset>


           <div id="search_button_div">
               <asp:Button ID="SearchButton" runat="server" OnClick="SearchButton_Click" Text="Search" CssClass="search_button"></asp:Button>
           </div>


        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
