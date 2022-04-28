<%@ Page Title="Title Edit" Language="C#" MasterPageFile="~/Flareworks.Master" AutoEventWireup="true" CodeBehind="TitleEdit.aspx.cs" Inherits="FlareworksWeb.TitleEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h2>FlareWorks Title Information</h2>
   
   <asp:ScriptManager ID="MainScriptManager" runat="server" />
   <asp:UpdatePanel ID="MainFormPanel" runat="server">
       <ContentTemplate>
    <fieldset>
        <legend>Title Record Information</legend>

        <table class="form_display_table">
           <tr>
                <td class="form_display_col1">Bib Number:</td>
                <td><asp:Label ID="AlephNumLabel" runat="server" /></td>
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
                    <asp:TextBox ID="TitleTextBox" runat="server" Width="400px" ></asp:TextBox>
                    <asp:Label ID="TitleLabel" runat="server" CssClass="form_help_label" />
                </td>
            </tr>
            <tr id="IssnTableRow" runat="server">
                <td>ISSN:</td>
                <td>
                    <asp:TextBox ID="IssnTextBox" runat="server" Width="180px"></asp:TextBox> <br />
                    <asp:CheckBox ID="CatalogingCheckBox" runat="server" Text="Send to Cataloging ( for Serials without ISSN in catalog record )" CssClass="form_checkbox" />
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
                <td>Cataloging Type:</td>
                <td>
                    <asp:DropDownList ID="CatalogingTypeDropDown" runat="server"></asp:DropDownList>
                    <asp:Label ID="CatalogingTypeLabel" runat="server" CssClass="form_help_label" />
                </td>
            </tr>
            <tr>
                <td>Record Cleanup:</td>
                <td>
                    <div style="width:620px">
                        <asp:PlaceHolder ID="CleanupArea" runat="server">
                            
                        </asp:PlaceHolder>
                    </div>
                </td>
            </tr>
            <tr>
                <td>Title Notes:</td>
                <td>
                    <asp:TextBox ID="NotesTextBox" TextMode="MultiLine" Height="100px" Width="550px" runat="server"></asp:TextBox></td>
            </tr>
        </table>
        
        <table id="form_button_table">
            <tr>
                <td><asp:Button ID="CancelButton" runat="server" Text="CANCEL" OnClick="CancelButton_Click" Width="120px" /></td>
                <td><asp:Button ID="SaveButton" runat="server" Text="SAVE" OnClick="SaveButton_Click" Width="120px" /></td>
            </tr>
        </table>
        
        <asp:Label ID="FormError" runat="server" CssClass="form_error_label" />
        <asp:Label ID="FormSavedLabel" runat="server" CssClass="form_saved_label" />
    </fieldset>

       
       </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>