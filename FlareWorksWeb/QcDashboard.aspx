<%@ Page Title="QC Dashboard" Language="C#" MasterPageFile="~/FlareworksNavBar.Master" AutoEventWireup="true" CodeBehind="QcDashboard.aspx.cs" Inherits="FlareworksWeb.QcDashboard" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" Runat="Server">

</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h2>Quality Control Administrative Dashboard</h2>

    <fieldset><legend><strong>Filter By Location / User</strong></legend>
        <div class="QcFilterBox1">Location: <asp:DropDownList ID="FilterLocationsDropDown" runat="server"></asp:DropDownList></div>
        <div class="QcFilterBox2">Worker: <asp:DropDownList ID="FilterUsersDropDown" runat="server"></asp:DropDownList></div>
        <div class="QcFilterButton"><asp:Button ID="FilterButton" runat="server" Text="Filter" Width="130px" Height="35px" OnClick="FilterButton_Click" /></div>
    </fieldset>
    
    <fieldset><legend><strong>Titles Pending Quality Control</strong></legend>

        <asp:Table ID="PendingQcTable" runat="server" CssClass="dashboard_item_table_style">

        </asp:Table>

        <asp:PlaceHolder ID="PendingQcItems" runat="server" ></asp:PlaceHolder>
        
        <br /><br />
        
        <asp:Button ID="SelectAllButton" runat="server" Text="Select All" Width="180px" Height="45px" OnClick="SelectAllButton_Click" />
        <asp:Button ID="ApproveAllButton" runat="server" Text="Approve Selected" Width="180px" Height="45px" OnClick="ApproveAllButton_Click" OnClientClick="if (!ApproveAllConfirmation()) return false;" />    

    </fieldset>
    
    
    <fieldset><legend><strong>QC'd Titles Pending Worker Correction</strong></legend>

    <asp:PlaceHolder ID="PendingCorrectionItems" runat="server" ></asp:PlaceHolder>

    </fieldset>
    
    <asp:Label ID="FormError" runat="server" CssClass="form_error_label" />
</asp:Content>