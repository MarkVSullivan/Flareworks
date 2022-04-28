<%@ Page Title="FlareWorks No Permission "Language="C#" MasterPageFile="~/Flareworks.Master" AutoEventWireup="true" CodeBehind="NoPermissions.aspx.cs" Inherits="FlareworksWeb.UserMgmt.NoPermissions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <h2>No Permissions</h2>
    
    <div style="padding-left:50px; padding-right:50px; padding-bottom: 200px;">
    
         <p>You are logged onto FlareWorks, but you have no permissions assigned.</p>
    
         <p>If you just registered, let your manager know so they can grant you the appropriate permissions.</p>
        
        <p><a href="Logout.aspx">Log out of FlareWorks.</a>.</p>
        
    </div>
    
    
</asp:Content>