<%@ Page Title="User Registration" Language="C#" MasterPageFile="~/Flareworks.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="FlareworksWeb.UserMgmt.Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h2>Register for FlareWorks</h2>
    
    <div style="padding-left:50px; padding-right:50px; padding-bottom: 200px;">
      
        <p>Account information, name, and email are required for each new account.</p>
    
        <p>Already registered? <a href="Login.aspx">Log on</a>.</p>
        
        <asp:Label ID="ErrorLabel" runat="server" Text="" ></asp:Label>
    
        <table id="register_table">
            <tr><td class="register_heading" colspan="4">Account Information</td></tr>
            <tr>
                <td style="width:20px;"></td>
                <td style="width:170px">UserName:</td>
                <td><asp:TextBox ID="UserNameTextBox" runat="server" Width="180px"></asp:TextBox></td>
                <td class="register_hint">(minimum of 8 characters)</td>
            </tr>
            <tr>
                <td></td>
                <td>Password:</td>
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
            <tr><td class="register_heading" colspan="4">Personal Information</tr>
            <tr>
                <td></td>
                <td>First Name:</td>
                <td>
                    <asp:TextBox ID="FirstNameTextBox" runat="server" Width="180px"></asp:TextBox>
                </td>
                <td class="register_hint"></td>
            </tr>
            <tr>
                <td></td>
                <td>Last Name:</td>
                <td>
                    <asp:TextBox ID="LastNameTextBox" runat="server" Width="180px"></asp:TextBox>
                </td>
                <td class="register_hint"></td>
            </tr>
            <tr>
                <td></td>
                <td>Email:</td>
                <td colspan="2">
                    <asp:TextBox ID="EmailTextBox" runat="server" Width="280px"></asp:TextBox>
                </td>
            </tr>
            <tr><td class="register_heading" colspan="4">Work Information</tr>
            <tr>
                <td></td>
                <td>Location:</td>
                <td>
                    <asp:DropDownList ID="LocationDropDownList" runat="server" Width="180px">
                    </asp:DropDownList>
                </td>
                <td class="register_hint">(Select the location you will be working, if you know)
            </tr>
        </table>
        
        <div id="login_register_buttons">
            <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" /> &nbsp; &nbsp; 
            <asp:Button ID="RegisterButton" runat="server" Text="Register" OnClick="RegisterButton_Click" />
        </div>
    
    </div>
    
    
</asp:Content>