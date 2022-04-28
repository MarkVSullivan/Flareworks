<%@ Page Title="Item Entry" Language="C#" MasterPageFile="~/Flareworks.Master" AutoEventWireup="true" CodeBehind="ItemEntry.aspx.cs" Inherits="FlareworksWeb.ItemEntry" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <h2>Work Information</h2>

    <asp:ScriptManager ID="MainScriptManager" runat="server" />
    <asp:UpdatePanel ID="MainFormPanel" runat="server">
        <ContentTemplate>

            <asp:HiddenField ID="AuthorityRowsCount" runat="server" value="1" />


            <fieldset>
                <legend>Cataloging Information</legend>

                <table class="form_display_table">
                    <tr>
                        <td>Title:</td>
                        <td colspan="2">
                            <asp:Label ID="TitleLabel" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="form_display_col1">Project:</td>
                        <td style="width: 180px">
                            <asp:DropDownList ID="InstitutionDropDown" runat="server" AutoPostBack="true"></asp:DropDownList>
                            <asp:Label ID="InstitutionLabel" runat="server" CssClass="form_help_label" />
                        </td>
                        <td>
                            <asp:CheckBox ID="LastCopyCheckBox" runat="server" Text="Last Copy" AutoPostBack="True" OnCheckedChanged="LastCopyCheckBox_OnCheckedChanged" />
                            <asp:TextBox runat="server" ID="LastCopyInstitutionBox" Width="300px" Visible="False"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>Material Type:</td>
                        <td colspan="2">
                            <asp:DropDownList ID="MaterialTypeDropDown" runat="server"></asp:DropDownList>
                            <asp:Label ID="MaterialTypeLabel" runat="server" CssClass="form_help_label" />
                        </td>
                    </tr>
                    <tr>
                        <td>Cataloging Type:</td>
                        <td colspan="2">
                            <asp:PlaceHolder ID="CatalogingTypePlaceHolder" runat="server"></asp:PlaceHolder>
                            <asp:Label ID="CatalogingTypeLabel" runat="server" CssClass="form_help_label" />
                        </td>
                    </tr>
                    <tr id="PccCategoryRow" runat="server">
                        <td>PCC Category:</td>
                        <td colspan="2">
                            <asp:DropDownList ID="PccCategoryMainDropDown" runat="server" AutoPostBack="True" OnSelectedIndexChanged="PccCategoryMain_OnSelectedChanged">
                                <asp:ListItem Value="-1" Text="None"></asp:ListItem>
                                <asp:ListItem Value="2" Text="BibCo"></asp:ListItem>
                                <asp:ListItem Value="3" Text="CONSER"></asp:ListItem>
                            </asp:DropDownList>

                            <asp:PlaceHolder ID="PccCategoryPlaceHolder" runat="server"></asp:PlaceHolder>
                            <asp:Label ID="PccCategoryLabel" runat="server" CssClass="form_help_label" />
                        </td>
                    </tr>
                    <tr>
                        <td>Item/HOL Action Type:</td>
                        <td colspan="2">
                            <asp:RadioButton runat="server" ID="ItemHol_None" GroupName="ItemHolGroup" Text="None" AutoPostBack="True" OnCheckedChanged="ItemHol_None_OnCheckedChanged" />
                            <asp:RadioButton runat="server" ID="ItemHol_Process" GroupName="ItemHolGroup" Text="Process" AutoPostBack="True" OnCheckedChanged="ItemHol_Process_OnCheckedChanged" />
                            <asp:RadioButton runat="server" ID="ItemHol_Edit" GroupName="ItemHolGroup" Text="Edit" AutoPostBack="True" OnCheckedChanged="ItemHol_Edit_OnCheckedChanged" />

                            <asp:Label runat="server" ID="ItemHol_EditCountLabel" Text="Items Edited:"></asp:Label>
                            <asp:TextBox runat="server" ID="ItemHol_EditCount" Width="100px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr id="AuthorityCatalogingTable" runat="server">
                        <td>Authority Cataloging Type:</td>
                        <td colspan="2">
                            <asp:Table ID="AuthParametersTable" runat="server" CssClass="form_subdisplay_table">
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:DropDownList ID="Authority_Type0" runat="server"></asp:DropDownList>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:RadioButtonList ID="Authority_Action0" runat="server" RepeatDirection="Horizontal">
                                            <asp:ListItem Value="original" Selected="True">Original</asp:ListItem>
                                            <asp:ListItem Value="edit">Edit</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:Button ID="AddAnotherAuthButton" runat="server" OnClick="AddAnotherAuthButton_Click" Text="Add" CssClass="add_search_button"></asp:Button>
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>

                        </td>
                    </tr>
                    <tr>
                        <td>Processor Notes:</td>
                        <td colspan="2">
                            <asp:TextBox ID="NotesTextBox" TextMode="MultiLine" Height="60px" Width="420px" runat="server" AutoPostBack="true" OnTextChanged="NotesTextBox_OnTextChanged"></asp:TextBox></td>
                    </tr>
                </table>

            </fieldset>

            <asp:Panel ID="NewItemPanel" runat="server">
                <fieldset>
                    <legend>Add New Item Information for Today</legend>
                    <table class="form_display_table">
                    </table>
                    <table class="form_display_table" style="padding-top: 0;">
                        <tr>
                            <td style="width: 280px;">Number of items sent to tray:</td>
                            <td colspan="2">
                                <asp:TextBox ID="TrayedItemsTextBox" runat="server" Width="100px" onkeydown="return (!((event.keyCode>=65 && event.keyCode <= 95) || event.keyCode >= 106) && event.keyCode!=32);" />
                                <asp:Label ID="TrayedItemsLabel" runat="server" CssClass="form_help_label" />
                            </td>
                        </tr>
                        <tr>
                            <td>Number of items withdrawn/dupes:</td>
                            <td colspan="2">
                                <asp:TextBox ID="WithdrawnItemsTextBox" runat="server" Width="100px" onkeydown="return (!((event.keyCode>=65 && event.keyCode <= 95) || event.keyCode >= 106) && event.keyCode!=32);" />
                                <asp:Label ID="WithdrawnItemsLabel" runat="server" CssClass="form_help_label" />
                            </td>
                        </tr>
                        <tr>
                            <td>Number of damaged items:</td>
                            <td style="width: 250px">
                                <asp:TextBox ID="DamagedItemsTextBox" runat="server" Width="100px" onkeydown="return (!((event.keyCode>=65 && event.keyCode <= 95) || event.keyCode >= 106) && event.keyCode!=32);" />
                                <asp:Label ID="DamagedItemsLabel" runat="server" CssClass="form_help_label" />
                            </td>
                            <td>
                                <asp:Button ID="AddItemButton" runat="server" Text="ADD" OnClick="AddItemButton_Click" Width="120px" />
                            </td>
                        </tr>
                    </table>


                </fieldset>
            </asp:Panel>

            <asp:Panel ID="ItemInfoPanel" runat="server">
                <fieldset>
                    <legend>Item Information</legend>

                    <asp:Table ID="ItemInfoTable" CssClass="item_work_table" runat="server">

                    </asp:Table>
                </fieldset>
            </asp:Panel>



            <br />
            <table id="form_button_table">
                <tr>
                    <td>
                        <asp:Button ID="TitleButton" runat="server" Text="PREVIOUS" OnClick="TitleButton_Click" Width="140px" /></td>
                    <td>
                        <asp:Button ID="CompleteButton" runat="server" Text="SAVE" OnClick="CompleteButton_Click" Width="140px" /></td>
                    <td>
                        <asp:Button ID="SubmitToQcButton" runat="server" Text="SUBMIT TO QC" OnClick="SubmitToQcButton_Click" Width="140px" /></td>
                </tr>
            </table>
            <asp:Label ID="FormError" runat="server" CssClass="form_error_label" />

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
