<%@ Page Title="Perform Quality Control" Language="C#" MasterPageFile="~/Flareworks.Master" AutoEventWireup="true" CodeBehind="SingleQc.aspx.cs" Inherits="FlareworksWeb.SingleQc" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" Runat="Server">

</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h2>Perform Quality Control</h2>
    
    <fieldset>
        <legend>Title Record Information</legend>
    
        <table class="form_display_table_smaller" style="width: 500px; float: left;">
            <tr id="BibNumTableRow" runat="server">
                <td class="form_display_col1_smaller" style="width:140px;">Bib Number:</td>
                <td><asp:Label ID="bibNumberLabel" runat="server" ></asp:Label></td>
            </tr>
            <tr id="OclcTableRow" runat="server">
                <td class="form_display_col1_smaller" style="width:140px;">OCLC:</td>
                <td><asp:Label ID="oclcLabel" runat="server" ></asp:Label></td>
            </tr>
           <tr>
                <td class="form_display_col1_smaller">Record Type:</td>
                <td><asp:Label ID="typeLabel" runat="server" ></asp:Label></td>
            </tr>
           <tr id="BibLevelTableRow" runat="server">
                <td class="form_display_col1_smaller">Bib Level:</td>
                <td><asp:Label ID="bibLevelLabel" runat="server" ></asp:Label></td>
            </tr>
            <tr>
                <td class="form_display_col1_smaller">Title:</td>
                <td><asp:Label ID="titleLabel" runat="server" ></asp:Label></td>
            </tr>
            <tr id="IssnTableRow" runat="server">
                <td class="form_display_col1_smaller">ISSN:</td>
                <td><asp:Label ID="issnLabel" runat="server" ></asp:Label></td>
            </tr>
            <tr id="DocumentTypeTableRow" runat="server">
                <td class="form_display_col1_smaller">Document Type:</td>
                <td><asp:Label ID="documentTypeLabel" runat="server" ></asp:Label></td>
            </tr>
            <tr id="FedAgencyTableRow" runat="server">
                <td class="form_display_col1_smaller">Federal Agency:</td>
                <td><asp:Label ID="federalAgencyLabel" runat="server" ></asp:Label></td>
            </tr>
        </table>
        
        <table class="form_display_table_smaller" style="width: 400px; float: right;">
            <tr>
                <td class="form_display_col1_smaller" style="width:140px;">Worker:</td>
                <td><asp:Label ID="workerLabel" runat="server" ></asp:Label></td>
            </tr>
            <tr>
                <td class="form_display_col1_smaller" style="width:140px;">Project:</td>
                <td><asp:Label ID="projectLabel" runat="server" ></asp:Label></td>
            </tr>
           <tr id="lastCopyRow" runat="server">
                <td class="form_display_col1_smaller">Last Copy:</td>
                <td><asp:Label ID="lastCopyLabel" runat="server" ></asp:Label></td>
            </tr>
           <tr>
                <td class="form_display_col1_smaller">Material Type:</td>
                <td><asp:Label ID="materialTypeLabel" runat="server" ></asp:Label></td>
            </tr>
            <tr id="catalogingTypeRow" runat="server">
                <td class="form_display_col1_smaller">Cataloging Type:</td>
                <td><asp:Label ID="catalogingLabel" runat="server" ></asp:Label></td>
            </tr>
        </table>

        <table class="form_display_table_smaller">
             <tr id="NotesTableRow" runat="server">
                <td class="form_display_col1_smaller">Title Notes:</td>
                <td>
                    <div style="width:720px">
                        <asp:Label ID="titleNotesLabel" runat="server" ></asp:Label>
                    </div>
                </td>
            </tr>
        </table>
    </fieldset>
    

    <fieldset>
        <legend>Item Information</legend>
        
        <asp:PlaceHolder ID="ItemInfoPlaceHolder" runat="server" ></asp:PlaceHolder>

           <table class="form_display_table_smaller">
            <tr id="ProcessorNotesTableRow" runat="server">
                <td class="form_display_col1_smaller" style="width:190px;">Processor Notes:</td>
                <td>
                    <div style="width:720px">
                        <asp:Label ID="processorNotesLabel" runat="server" ></asp:Label>
                    </div>
                </td>
            </tr>
        </table>
    </fieldset>
    
    <fieldset>
        
           <legend>Quality Control Results</legend>
        
           <table class="form_display_table_smaller">
            <tr>
                <td class="form_display_col1_smaller" style="width:140px;">Last QC'd:</td>
                <td>No QC history</td>
            </tr>
            <tr>
                <td class="form_display_col1_smaller">QC Notes:</td>
                <td><asp:TextBox ID="QcNotesTextBox" TextMode="MultiLine" Height="100px" Width="670px" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="form_display_col1_smaller">Error Types:</td>
                <td>
                    <div style="width:720px">
                        <asp:PlaceHolder ID="ErrorTypesPlaceHolder" runat="server">
                            
                        </asp:PlaceHolder>
                    </div>
                </td>
            </tr>
        </table>
    
        <table id="form_button_table">
            <tr>
                <td><asp:Button ID="CancelButton" runat="server" Text="CANCEL" Width="120px" OnClick="CancelButton_Click" /></td>
                <td><asp:Button ID="SaveButton" runat="server" Text="SAVE" Width="120px" OnClick="SaveButton_Click" /></td>
                <td><asp:Button ID="RejectButton" runat="server" Text="REJECT" Width="120px" OnClick="RejectButton_Click" /></td>
                <td><asp:Button ID="ApproveButton" runat="server" Text="APPROVE" Width="120px" OnClick="ApproveButton_Click" /></td>
            </tr>
        </table>
    </fieldset>
    
    
    <br />
    <br />

</asp:Content>