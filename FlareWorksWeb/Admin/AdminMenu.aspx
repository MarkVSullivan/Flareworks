<%@ Page Title="Administration" Language="C#" MasterPageFile="~/FlareworksNavBar.Master" AutoEventWireup="true" CodeBehind="AdminMenu.aspx.cs" Inherits="FlareworksWeb.Admin.AdminMenu" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" Runat="Server">

</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h2>FlareWorks Admin</h2>
    
    <fieldset>
        <legend>Administrative Tasks</legend>
        <table id="form_button_table">
            <tr>
                <td><asp:Button ID="UserButton" runat="server" Text="User Administration" Width="240px" Height="45px" OnClick="UserButton_Click" /></td>
                <td><asp:Button ID="ProjectsButton" runat="server" Text="Projects List" Width="240px" Height="45px" /></td>
            </tr>
            <tr>                
                <td><asp:Button ID="CatalogTypeButton" runat="server" Text="Catalog Type List (GovDoc)" Width="240px" Height="45px" /></td>
                <td><asp:Button ID="MaterialTypesButton" runat="server" Text="Material Types List" Width="240px" Height="45px" /></td>
            </tr>
            <tr>
                <td><asp:Button ID="DocumentTypesButton" runat="server" Text="Document Type (GovDoc)" Width="240px" Height="45px" /></td>
                <td>&nbsp;</td>
            </tr>
        </table>
    </fieldset>
    
    <br /><br />

</asp:Content>