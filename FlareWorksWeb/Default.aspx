<%@ Page Title="Home" Language="C#" MasterPageFile="~/FlareworksNavBar.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="FlareworksWeb.Default" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" Runat="Server">

</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h2>FlareWorks Dashboard</h2>
    
    <asp:PlaceHolder ID="UserDashboardData" runat="server" ></asp:PlaceHolder>

    <fieldset>
        <legend><strong>Common Tasks</strong></legend>
        <table id="form_button_table">
            <tr>
                <td><asp:Button ID="AddItemButton" runat="server" Text="ADD AN ITEM" OnClick="AddItemButton_Click" Width="180px" Height="45px" /></td>
                <td><asp:Button ID="SearchButton" runat="server" Text="SEARCH" OnClick="SearchButton_Click" Width="180px" Height="45px" /></td>
            </tr>
        </table>
    </fieldset>
    
    <asp:Label ID="FormError" runat="server" CssClass="form_error_label" />
</asp:Content>