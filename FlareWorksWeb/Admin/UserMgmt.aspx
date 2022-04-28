<%@ Page Title="User Administration" Language="C#" MasterPageFile="~/FlareworksNavBar.Master" AutoEventWireup="true" CodeBehind="UserMgmt.aspx.cs" Inherits="FlareworksWeb.Admin.UserMgmt" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" Runat="Server">
    <style media="screen" type="text/css">
        #content-table-navbar
        {
            width: 1200px;
        }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <!-- Hidden field is used for postbacks to indicate what to save and reset -->
    <input type="hidden" id="admin_user_id" name="admin_user_id" value="" />
    <input type="hidden" id="admin_user_action" name="admin_user_action" value="" />

    <h2>FlareWorks User Administration</h2>
    
    <asp:Label ID="ErrorLabel" runat="server" Text="" ></asp:Label>
    
    <% Add_User_Table(); %>

</asp:Content>