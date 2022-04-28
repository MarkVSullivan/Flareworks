<%@ Page Title="Title Entry" Language="C#" MasterPageFile="~/Flareworks.Master" AutoEventWireup="true" CodeBehind="TitleEntry.aspx.cs" Inherits="FlareworksWeb.TitleEntry" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h2>FlareWorks Title Information</h2>

    
   <asp:ScriptManager ID="MainScriptManager" runat="server" />
   <asp:UpdatePanel ID="MainFormPanel" runat="server">
       <ContentTemplate>

   <asp:Panel runat="server" ID="TitleSearchPanel">
      <fieldset>
        <legend>Title Identifier</legend>
        <table class="form_display_table">
            <tr>
                <td class="form_display_col1">Bib Number:</td>
                <td style="width:350px;">
                    <asp:TextBox ID="AlephNumTextBox" runat="server" Width="160px" OnTextChanged="AlephNumTextBox_TextChanged" AutoPostBack="True" onkeydown="return numbers_only();" ></asp:TextBox>
                </td>
                <td style="width:200px;">OCLC Number:</td>
                <td>
                    <asp:TextBox ID="OclcNumTextBox" runat="server" Width="160px" OnTextChanged="OclcNumTextBox_TextChanged" AutoPostBack="True" onkeydown="return numbers_only();" ></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td></td>
                <td colspan="2"><asp:Label ID="AlephNumLabel" runat="server" CssClass="form_help_label" /></td>
                <td><asp:Button ID="ClearButton" runat="server" Text="CLEAR" OnClick="ClearButton_Click" Width="100px" /></td>
            </tr>
        </table>
      </fieldset>

   </asp:Panel>

   <asp:Panel runat="server" ID="TitleInfoPanel">
    <fieldset>
        <legend>Title Record Information</legend>

        <table class="form_display_table">
           <tr>
               <td></td>
               <td><asp:Label ID="AlephNumLabel2" runat="server" CssClass="form_help_label" /></td>
           </tr>
           <tr>
                <td class="form_display_col1">Record Type:</td>
                <td>
                    <asp:PlaceHolder ID="RecordTypeArea" runat="server">
                    </asp:PlaceHolder>
                    <asp:Label ID="RecordTypeLabel" runat="server" CssClass="form_help_label" />
                </td>
            </tr>
            <tr>
                <td>Bibliographic Level:</td>
                <td>
                    <asp:PlaceHolder ID="BibLevelArea" runat="server">
                    </asp:PlaceHolder>
                    <asp:Label ID="BibLevelLabel" runat="server" CssClass="form_help_label" />
                </td>
            </tr>
            <tr>
                <td>Title:</td>
                <td>
                    <asp:TextBox ID="TitleTextBox" runat="server" Width="600px" ></asp:TextBox>
                    <asp:Label ID="TitleLabel" runat="server" CssClass="form_help_label" />
                </td>
            </tr>
            <tr id="IssnTableRow" runat="server">
                <td>ISSN:</td>
                <td>
                    <asp:CheckBox ID="CatalogingCheckBox" runat="server" Text="Send to Cataloging ( for Serials without ISSN in catalog record )" CssClass="form_checkboxCataloging" />
                </td>
            </tr>
            <tr>
                <td>Bib Number:</td>
                <td>
                    <asp:TextBox ID="AlephNumEntryTextBox" runat="server" Width="160px" onkeydown="return numbers_only();" ></asp:TextBox>
                    <asp:Label ID="AlephNumEntryLabel" runat="server" />
                </td>
            </tr>
            <tr>
                <td>OCLC Number:</td>
                <td>
                    <asp:TextBox ID="OclcNumEntryTextBox" runat="server" Width="160px" onkeydown="return numbers_only();" ></asp:TextBox>
                    <asp:Label ID="OclcNumEntryLabel" runat="server" />
                </td>
            </tr>
            <tr id="DocumentTypeTableRow" runat="server">
                <td>Document Type:</td>
                <td>
                    <asp:DropDownList ID="DocTypeDropDown" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DocTypeDropDown_SelectedIndexChanged"></asp:DropDownList>
                    <asp:Label ID="DocTypeLabel" runat="server" CssClass="form_help_label" />
                </td>
            </tr>
            <tr id="FedAgencyTableRow" runat="server">
                <td>Federal Agency:</td>
                <td>
                    <asp:TextBox ID="FederalAgencyTextBox" runat="server" Width="400px" ></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>Title Notes:</td>
                <td>
                    <asp:TextBox ID="NotesTextBox" TextMode="MultiLine" Height="100px" Width="550px" runat="server"></asp:TextBox></td>
            </tr>
        </table>
        

        
        <asp:Label ID="FormError" runat="server" CssClass="form_error_label" />
        <asp:Label ID="FormSavedLabel" runat="server" CssClass="form_saved_label" />
    </fieldset>
        
    
       </asp:Panel>  
           
           <br />
        <table id="form_button_table">
            <tr>
                <td><asp:Button ID="CancelButton" runat="server" Text="CANCEL" OnClick="CancelButton_Click" Width="120px" /></td>
                <td><asp:Button ID="SaveButton" runat="server" Text="SAVE" OnClick="SaveButton_Click" Width="120px" /></td>
                <td><asp:Button ID="ItemsButton" runat="server" Text="NEXT" OnClick="ItemsButton_Click" Width="120px" /></td>
            </tr>
        </table>
       <br /><br />
       </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>