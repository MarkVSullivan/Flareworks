<%@ Page Title="Single User Administration" Language="C#" MasterPageFile="~/FlareworksNavBar.Master" AutoEventWireup="true" CodeBehind="UserSingleMgmt.aspx.cs" Inherits="FlareworksWeb.Admin.UserSingleMgmt" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <h2>FlareWorks Single User</h2>
    
    <div style="padding-left:50px; padding-right:50px; padding-bottom: 200px;">
    
    <asp:Label ID="UserNotesLabel" runat="server" ></asp:Label>
    
    <asp:Label ID="ErrorLabel" runat="server" Text="" ></asp:Label>
    
    <table id="single_user_table">
        <tr><td class="single_user_heading" colspan="3">Personal Information</td></tr>
        <tr>
            <td style="width: 30px;"></td>
            <td style="width:180px;">UserName:</td>
            <td><asp:Label ID="UserNameLabel" runat="server" Text="Label"></asp:Label></td>
        </tr>
        <tr>
            <td></td>
            <td>Full Name:</td>
            <td><asp:Label ID="FullNameLabel" runat="server" Text="Label"></asp:Label></td>
        </tr>
        <tr>
            <td></td>
            <td>Email:</td>
            <td><asp:TextBox ID="EmailTextBox" runat="server" Width="280px"></asp:TextBox></td>
        </tr>
        <tr><td class="single_user_heading" colspan="3">Account Information</td></tr>
        <tr>
            <td></td>
            <td>Display Name:</td>
            <td><asp:TextBox ID="DisplayNameTextBox" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td></td>
            <td>Location:</td>
            <td><asp:DropDownList ID="LocationDropDownList" runat="server" ></asp:DropDownList></td>
        </tr>
        <tr>
            <td></td>
            <td colspan="2"><asp:CheckBox ID="PccCategoryActive" runat="server" Text="Is Cataloging Specialist (access to authority info)" /></td>
        </tr>
        <tr><td class="single_user_heading" colspan="3">Permissions Information</td></tr>
        <tr>
            <td></td>
            <td colspan="2"><asp:CheckBox ID="ActiveCheckBox" runat="server" Text="Is Active" /></td>
        </tr>
        <tr>
            <td></td>
            <td colspan="2"><asp:CheckBox ID="CanQcCheckbox" runat="server" Text="Can Perform Quality Control" /></td>
        </tr>
        <tr>
            <td></td>
            <td colspan="2"><asp:CheckBox ID="CanPullReportsCheckBox" runat="server" Text="Can Pull Reports" /></td>
        </tr>
        <tr>
            <td></td>
            <td colspan="2"><asp:CheckBox ID="CanAddToPullListCheckBox" runat="server" Text="Can Add to Pull List" /></td>
        </tr>
        <tr>
            <td></td>
            <td colspan="2"><asp:CheckBox ID="IsPullListAdminCheckBox" runat="server" Text="Is Pull List Admin" /></td>
        </tr>
        <tr>
            <td></td>
            <td colspan="2"><asp:CheckBox ID="IsSystemAdminCheckBox" runat="server" Text="Is System Administrator" /></td>
        </tr>
    </table>
    
    <div id="single_user_buttons">
            <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" /> &nbsp; &nbsp; 
            <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
    </div>
    
    </div>

</asp:Content>