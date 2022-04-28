<%@ Page Title="User Login" Language="C#" MasterPageFile="~/Flareworks.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="FlareworksWeb.UserMgmt.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h2>Login to FlareWorks</h2>
    
    <div style="padding-left:50px; padding-right:50px; padding-bottom: 200px;">
      
        <p>The feature you are trying to access requires a valid login.</p>
    
        <p>Not registered yet? <a href="Register.aspx">Register Now</a>.</p>
        
        <asp:Label ID="ErrorLabel" runat="server" Text="" ></asp:Label>
        
        <table id="login_table">
            <tr><td class="login_heading" colspan="3">Account Information</td></tr>
            <tr>
                <td style="width:20px;"></td>
                <td style="width:180px">UserName or Email:</td>
                <td>
                    <asp:TextBox ID="UserNameTextBox" runat="server" Width="180px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td></td>
                <td>Password:</td>
                <td>
                    <asp:TextBox ID="PasswordTextBox" runat="server" TextMode="Password" Width="180px"></asp:TextBox>
                </td>
            </tr>
        </table>
        
        <div id="login_register_buttons">
            <asp:Button ID="LogInButton" runat="server" Text="Enter" OnClick="LogInButton_Click" />
        </div>
        
    </div>
    
    
</asp:Content>