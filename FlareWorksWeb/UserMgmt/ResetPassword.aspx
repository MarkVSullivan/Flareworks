<%@ Page Title="Flareworks Reset Password" Language="C#" MasterPageFile="~/Flareworks.Master" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="FlareworksWeb.UserMgmt.ResetPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <h2>New Password Required</h2>
    
    <div style="padding-left:50px; padding-right:50px; padding-bottom: 200px;">
    
         <p>You are logged onto FlareWorks, but you need to change your temporary password.</p>
    
        <asp:Label ID="ErrorLabel" runat="server" Text="" ></asp:Label>
    
        <table id="register_table">
            <tr><td class="register_heading" colspan="4">Account Information</td></tr>
            <tr>
                <td style="width:20px;"></td>
                <td style="width:170px">UserName:</td>
                <td><asp:Label ID="UserNameLabel" runat="server"></asp:Label></td>
                <td></td>
            </tr>
            <tr>
                <td></td>
                <td>New Password:</td>
                <td>
                    <asp:TextBox ID="PasswordTextBox" runat="server" Width="180px" TextMode ="Password"></asp:TextBox>
                </td>
                <td class="register_hint">(minimum of eight digits, different than username)</td>
            </tr>
            <tr>
                <td></td>
                <td>Confirm Password:</td>
                <td id="Password2TextBox">
                    <asp:TextBox ID="PasswordTextBox2" runat="server" Width="180px" TextMode="Password"></asp:TextBox>
                </td>
                <td class="register_hint">(minimum of eight digits, different than username)</td>
            </tr>
        </table>
        
        <div id="login_register_buttons">
            <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
        </div>
        
    </div>
    
    
</asp:Content>