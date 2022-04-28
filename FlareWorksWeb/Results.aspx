<%@ Page Title="Results" Language="C#" MasterPageFile="~/FlareworksFullScreen.Master" AutoEventWireup="true" CodeBehind="Results.aspx.cs" Inherits="FlareworksWeb.Results" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" Runat="Server">

      <script type="text/javascript" src="includes/jquery/jquery-3.3.1.min.js" ></script>
      <script type="text/javascript" src="includes/datatables/datatables.min.js" ></script>      <link href="includes/datatables/datatables.min.css" rel="stylesheet" type="text/css" />

</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h2>Search Results</h2>

    
    <br /><br />
    

    <div style="float:right;"><asp:Button ID="ReturnButton" runat="server" Text="Modify Search" OnClick="ReturnButton_Click" /></div>
    <div style="float:left; font-weight:bold;"><% Add_Results_Count(); %></div>





    <br /><br />

    <% Add_Results_Table(); %>

    

    <br /><br />

    <% Add_DataTable_Script(); %>

</asp:Content>
