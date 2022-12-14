<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExitCompany.aspx.cs" Inherits="WorkerExitPass.ExitCompany" EnableEventValidation = "false"%>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Exit Company</title>
    <link href="https://fonts.googleapis.com/css2?family=Open+Sans&display=swap" rel="stylesheet"/>
    <link href="https://fonts.googleapis.com/css2?family=Rubik:wght@700&display=swap" rel="stylesheet" />
    <link href="Content/StyleSheet5.css" rel="stylesheet" type="text/css" />
    <link href="Content/font-awesome.css" rel="stylesheet" type="text/css" />
    <link href="Content/font-awesome.min.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>Exit Company</h1>

            <div class="container">
                <div class="form">
                    <div class="row">
                        <div class="col-12">
                            <div class="rowBtn">
                                <asp:Button ID="createBtn" runat="server" Text="Create New" OnClick="createBtn_Click" CssClass="activeBtn"/>
                                <asp:Button ID="updateDetailsBtn" runat="server" Text="Update Details" OnClick="updateDetailsBtn_Click" CssClass="inactiveBtn"/>
                            </div>                            
                        </div>
                     </div>
                    <asp:Panel ID="Panel1" runat="server" Visible="true">
                        <div class="row">
                            <div class="col">
                             <asp:Label class="label" runat="server" Text="Employee ID"></asp:Label>
                                <div class="row1">
                                 <asp:TextBox class="input" ID="lblEmpID" runat="server"></asp:TextBox>
                                 <asp:Button ID="NextBtn" runat="server" Text="Next" OnClick="NextBtn_Click"/>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                              <div class="col">
                              <asp:Label class="label" ID="lblCompanyName" runat="server" Text="Name of Company" Visible="false"></asp:Label>
                              <asp:DropDownCheckBoxes ID="companyddl" runat="server" CssClass="dropdown" Visible="false" UseButtons="True" OnSelectedIndexChanged="companyddl_SelectedIndexChanged">
       
                                        <Style SelectBoxWidth="" DropDownBoxBoxWidth="" DropDownBoxBoxHeight="" SelectBoxCssClass="dropdown"></Style>

                                        <Style2 SelectBoxWidth="" DropDownBoxBoxWidth="" DropDownBoxBoxHeight="" SelectBoxCssClass="dropdown"></Style2>
                                        <Texts SelectBoxCaption="Select Company" />
                             </asp:DropDownCheckBoxes>
                             </div>
                        </div>
                        <div class="row">
                            <div class="col-12">
                            <asp:Button class="button" ID="submitBtn" runat="server" Text="Submit" Visible="false" OnClick="submitBtn_Click"/>
                            </div>
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="Panel2" runat="server" Visible="false">
                        <div class="row">
                            <div class="col">
                             <asp:Label class="label" runat="server" Text="Employee ID"></asp:Label>
                                <div class="row1">
                                 <asp:TextBox class="input" ID="lblFindEmpID" runat="server" OnTextChanged="lblFindEmpID_TextChanged"></asp:TextBox>
                                 <asp:Button ID="SearchBtn" runat="server" Text="Search" OnClick="SearchBtn_Click"/>
                                </div>
                            </div>
                        </div>

                            <div class="row">
                                <div class="col">
                                <asp:Label class="label" ID="lblEmpName" runat="server" Text="Employee Name" Visible="false"></asp:Label>
                                    <asp:Label class="lblData" ID="lblDataEmpName" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col">
                                <asp:GridView ID="GridView1" GridLines="None" CssClass="table" HeaderStyle-CssClass="thead" RowStyle-CssClass="tr" runat="server" AutoGenerateColumns="False" OnRowDataBound="GridView1_RowDataBound" OnRowCommand="GridView1_RowCommand" DataKeyNames="Company,IsActive">
                                    <Columns>

                                        <asp:BoundField DataField="Company" HeaderText="Company" />
                                        <asp:BoundField DataField="IsActive" HeaderText="Status" />
                                        <asp:TemplateField>
                                            <HeaderTemplate>Update Status</HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Button CssClass="btnStatus" ID="btnStatus" CommandName="UpdateItem" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                </div>
                            </div>
                    </asp:Panel>
                    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                    <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                        <ajaxToolkit:ModalPopupExtender ID="mpePopUp" runat="server" TargetControlID="Label1" PopupControlID="Panel" CancelControlID="btnContinue" BackgroundCssClass="modalBackground">
                        </ajaxToolkit:ModalPopupExtender>
                        <asp:Panel ID="Panel" runat="server">  
                            <div class="rowIcon">
                                <asp:LinkButton ID="btnContinue" runat="server" Text="<i class='fa fa-times fa-2x' aria-hidden='true'></i>"/>
                            </div>  
                            <div class="contentRow">
                                <asp:Label ID="labelSuccess" runat="server"></asp:Label>
                                <asp:Label class="lblData" ID="valid" runat="server"></asp:Label>
                            </div>
                      </asp:Panel>   
                </div>
                

            </div>
        </div>
    </form>
<%--    <asp:Label ID="Label1" runat="server"></asp:Label>--%>
</body>
</html>
