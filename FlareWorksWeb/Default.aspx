<%@ Page Title="Home" Language="C#" MasterPageFile="~/Flareworks.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="FlareworksWeb.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div id="web-billing-error">
        <br/><br/>
        <asp:Literal ID="errorLiteral" runat="server" ></asp:Literal>
        <br/><br/><br/>
        <a href="https://us-yellow.com">Click here to return to the search page.</a>
        <br/><br/>
    </div>
</asp:Content>