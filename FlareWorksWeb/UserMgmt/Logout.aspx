<%@ Page Title="Flareworks Goodbye" Language="C#" MasterPageFile="~/Flareworks.Master" AutoEventWireup="true" CodeBehind="Logout.aspx.cs" Inherits="FlareworksWeb.UserMgmt.Logout" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <h2>Goodbye</h2>
    
    <div style="padding-left:50px; padding-right:50px; padding-bottom: 200px;">
    
         <p>You have been logged out of FlareWorks.</p>
    
         <p><a href="Login.aspx">Click here to log back on.</a>.</p>
        
    </div>
    
    
</asp:Content>