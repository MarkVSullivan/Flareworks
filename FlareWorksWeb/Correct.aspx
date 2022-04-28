<%@ Page Title="Correct Rejections" Language="C#" MasterPageFile="~/Flareworks.Master" AutoEventWireup="true" CodeBehind="Correct.aspx.cs" Inherits="FlareworksWeb.Correct" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" Runat="Server">

</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h2>Correct Submission</h2>
    
        <fieldset>
            <legend>Quality Control Information</legend>
            
            <table class="form_display_table">
                <tr>
                    <td class="form_display_col1" style="width:140px;">Rejected Date:</td>
                    <td><asp:Label ID="RejectedDateLabel" runat="server" ></asp:Label></td>
                </tr>
                <tr>
                    <td class="form_display_col1">Errors:</td>
                    <td>
                        <div style="width:680px">
                            <asp:PlaceHolder ID="ErrorPlaceHolder" runat="server">
                            
                            </asp:PlaceHolder>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="form_display_col1">QC Notes:</td>
                    <td>
                        <div style="width:680px">
                            <asp:Label ID="RejectNotesLabel" runat="server" ></asp:Label>
                        </div>
                    </td>
                </tr>
            </table>

        </fieldset>
    
        <fieldset>
        <legend>Title Record Information</legend>
    
        <table class="form_display_table" style="width: 900px;">
            <tr>
                <td class="form_display_col1" style="width:140px;">Bib Number:</td>
                <td>
                    <asp:Label ID="bibNumberLabel" runat="server" ></asp:Label>
                    <div style="float:right"><asp:Button ID="EditTitleButton" runat="server" Text="Edit Title" OnClick="EditTitleButton_OnClick"/></div>
                </td>
            </tr>
           <tr>
                <td class="form_display_col1">Record Type:</td>
                <td><asp:Label ID="typeLabel" runat="server" ></asp:Label></td>
            </tr>
           <tr id="BibLevelTableRow" runat="server">
                <td class="form_display_col1">Bib Level:</td>
                <td><asp:Label ID="bibLevelLabel" runat="server" ></asp:Label></td>
            </tr>
            <tr>
                <td class="form_display_col1">Title:</td>
                <td><asp:Label ID="titleLabel" runat="server" ></asp:Label></td>
            </tr>
            <tr id="IssnTableRow" runat="server">
                <td class="form_display_col1">ISSN:</td>
                <td><asp:Label ID="issnLabel" runat="server" ></asp:Label></td>
            </tr>
            <tr id="DocumentTypeTableRow" runat="server">
                <td class="form_display_col1">Document Type:</td>
                <td><asp:Label ID="documentTypeLabel" runat="server" ></asp:Label></td>
            </tr>
            <tr id="FedAgencyTableRow" runat="server">
                <td class="form_display_col1">Federal Agency:</td>
                <td><asp:Label ID="federalAgencyLabel" runat="server" ></asp:Label></td>
            </tr>
           <tr>
                <td class="form_display_col1">Cataloging Type:</td>
                <td><asp:Label ID="catalogingLabel" runat="server" ></asp:Label></td>
            </tr>
            <tr>
                <td class="form_display_col1">Record Cleanup:</td>
                <td>
                    <div style="width:680px">
                        <asp:PlaceHolder ID="CleanupAreaPlaceHolder" runat="server">
                            
                        </asp:PlaceHolder>
                    </div>
                </td>
            </tr>
            <tr>
                <td class="form_display_col1">Title Notes:</td>
                <td>
                    <div style="width:680px">
                        <asp:Label ID="titleNotesLabel" runat="server" ></asp:Label>
                    </div>
                </td>
            </tr>
        </table>
    </fieldset>

    <fieldset>
        <legend>Item Information</legend>
        
        <asp:PlaceHolder ID="ItemInfoPlaceHolder" runat="server" ></asp:PlaceHolder>

           <table class="form_display_table">
            <tr>
                <td class="form_display_col1" style="width:140px;">Processor Notes:</td>
                <td>
                <td colspan="2">
                    <asp:TextBox ID="NotesTextBox" TextMode="MultiLine" Height="60px" Width="680px" runat="server"></asp:TextBox></td>
                </td>
            </tr>
        </table>
    </fieldset>
    
            <table id="form_button_table">
            <tr>
                <td><asp:Button ID="CancelButton" runat="server" Text="CANCEL" Width="120px" OnClick="CancelButton_Click" /></td>
                <td><asp:Button ID="SaveButton" runat="server" Text="SAVE" Width="120px" OnClick="SaveButton_Click" /></td>
                <td><asp:Button ID="ResubmitButton" runat="server" Text="RESUBMIT" Width="120px" OnClick="ResubmitButton_Click" /></td>
            </tr>
        </table>
    
    <br />
    <br />

</asp:Content>