﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm5.aspx.cs" Inherits="WorkerExitPass.WebForm5" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Early Exit Permit Approval</title>
    <link href="https://fonts.googleapis.com/css2?family=Open+Sans&display=swap" rel="stylesheet"/>
    <link href="https://fonts.googleapis.com/css2?family=Rubik:wght@700&display=swap" rel="stylesheet" />
    <link href="Content/main2.css" rel="stylesheet" type="text/css" />
    <link href="Content/StyleSheet4.css" rel="stylesheet" type="text/css" />
    <link href="Content/font-awesome.css" rel="stylesheet" type="text/css" />
    <link href="Content/font-awesome.min.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <h1>Early Exit Permit Approval</h1>
        <div class="container">
            <div class="row">            
               <asp:DropDownList ID="ddlReason" class="dropdown" runat="server" AutoPostBack="true">
                    <asp:ListItem Text="Reason" Value=""></asp:ListItem>
                    <asp:ListItem Text="Weather conditions" Value="Weather Conditions"></asp:ListItem>
                    <asp:ListItem Text="Emergency" Value="Emergency"></asp:ListItem>
                    <asp:ListItem Text="Go office" Value="Go office"></asp:ListItem>
                    <asp:ListItem Text="Others" Value="Others"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <asp:GridView ID="GridView1" GridLines="None" CssClass="table" HeaderStyle-CssClass="thead" RowStyle-CssClass="tr" runat="server" AutoGenerateColumns="False" BackColor="#EBF6FE" AllowPaging="True" DataKeyNames="exitID" OnRowDataBound="GridView1_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="exitID" HeaderText="ID" />
                    <asp:BoundField DataField="createddate" HeaderText="Requested Date" />
                    <asp:BoundField DataField="exittime" HeaderText="Requested Time" />
                    <asp:BoundField DataField="reason" HeaderText="Reason" />
                </Columns>
                <PagerStyle HorizontalAlign = "Right" CssClass = "GridPager" />
            </asp:GridView>

            <asp:Label ID="lblHidden" runat="server" Text=""></asp:Label>
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            
        <ajaxToolkit:ModalPopupExtender ID="mpeApproval" runat="server" TargetControlID="lblHidden" PopupControlID="Panel1" BackgroundCssClass="modalBackground">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="Panel1" runat="server">
               <div class="rowIcon">
                    <asp:LinkButton ID="btnBack" runat="server" Text="<i class='fa fa-times fa-2x' aria-hidden='true'></i>" OnClick="btnBack_Click"/>
               </div>     
                <div class="content">
                        <div class="rowModal">
                            <asp:Label ID="lblexitID" runat="server"></asp:Label>
                        </div>
                        <div class="dateTimeRow">
                            <div class="col">
                                    <asp:Label class="label" ID="date" runat="server" Text="Date"></asp:Label>
                                    <asp:TextBox class="textbox" ID="tbDate" runat="server" ReadOnly="True"></asp:TextBox>
                            </div>
                            <div class="col">
                                    <asp:Label class="label" ID="time" runat="server" Text="Time"></asp:Label>
                                    <asp:TextBox class="textbox" ID="tbTime" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                                <asp:Label class="label" ID="project" runat="server" Text="Project"></asp:Label>
                                <asp:TextBox class="textbox" ID="tbProject" runat="server" ReadOnly="True"></asp:TextBox>
                        </div> 
                        <div class="row">
                                <asp:Label class="label" ID="name" runat="server" Text="Name(s) of DM worker/contractor"></asp:Label>
                                <asp:TextBox class="textbox" ID="tbName" runat="server" ReadOnly="True"></asp:TextBox>
                        </div>
                        <div class="row">
                                <asp:Label class="label" ID="companyName" runat="server" Text="Name of Company"></asp:Label>
                                <asp:TextBox class="textbox" ID="tbCompany" runat="server" ReadOnly="True"></asp:TextBox>
                        </div>
                        <div class="row">
                                <asp:Label class="label" ID="reason" runat="server" Text="Reason for Leaving Yard"></asp:Label>
                                <asp:TextBox class="textbox" ID="tbReason" runat="server" ReadOnly="True"></asp:TextBox>
                        </div>
                        <div class="row">
                                <asp:Label class="label" ID="lblRemarks" runat="server" Text="Remarks"></asp:Label>
                                <asp:TextBox class="textbox" ID="tbRemarks" runat="server" ReadOnly="True"></asp:TextBox>
                        </div>
                            

                    <div class="btnRow">
                        <asp:Button id="btnApprove" class="button" runat="server" text="Approve" OnClick="ApproveBtn_Click"/>
                        <asp:Button id="btnReject" class="button" runat="server" text="Reject" OnClick="RejectBtn_Click" />
                    </div>
                </div>   
            </asp:Panel>
        </div>
    </form>
</body>
</html>