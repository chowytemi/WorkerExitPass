<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm4.aspx.cs" Inherits="WorkerExitPass.WebForm4" EnableEventValidation = "false"%>
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
<%--            <div class="btnRows">
                <asp:Button ID="btnShowAll" class="button" runat="server" Text="All" OnClick="btnShowAll_Click" />
                <asp:Button ID="btnShowPending" class="button" runat="server" Text="Pending" OnClick="btnShowPending_Click" />
                <asp:Button ID="btnShowApproved" class="button" runat="server" Text="Approved" OnClick="btnShowApproved_Click" />
                <asp:Button ID="btnShowRejected" class="button" runat="server" Text="Rejected" OnClick="btnShowRejected_Click" />
            </div>--%>
<%--            <asp:MultiView ID="MultiView1" runat="server" OnActiveViewChanged="MultiView1_ActiveViewChanged">
                <asp:View ID="View1" runat="server">
                    <asp:GridView ID="GridView2" GridLines="None" CssClass="table" HeaderStyle-CssClass="thead" RowStyle-CssClass="tr" runat="server" AutoGenerateColumns="False" BackColor="#EBF6FE" OnRowDataBound="GridView2_RowDataBound">
                        <Columns>
                            <asp:BoundField DataField="exitID" HeaderText="ID" />
                            <asp:BoundField DataField="createddate" HeaderText="Requested Date" />
                            <asp:BoundField DataField="exittime" HeaderText="Requested Time" />
                            <asp:BoundField DataField="reason" HeaderText="Reason" />
                            <asp:BoundField DataField="approve" HeaderText="Status" />
                        </Columns>
                    </asp:GridView>
                </asp:View>
                <asp:View ID="View2" runat="server">--%>
                    <div class="row">            
                        <asp:DropDownList ID="ddlReason" class="dropdown" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlReason_SelectedIndexChanged">
                            <asp:ListItem Text="Reason" Value=""></asp:ListItem>
                            <asp:ListItem Text="Weather conditions" Value="Weather Conditions"></asp:ListItem>
                            <asp:ListItem Text="Emergency" Value="Emergency"></asp:ListItem>
                            <asp:ListItem Text="Go office" Value="Go office"></asp:ListItem>
                            <asp:ListItem Text="Others" Value="Others"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <asp:GridView ID="GridView1" GridLines="None" CssClass="table" HeaderStyle-CssClass="thead" RowStyle-CssClass="tr" runat="server" AutoGenerateColumns="False" OnRowDataBound="GridView1_RowDataBound" OnSelectedIndexChanged="GridView1_SelectedIndexChanged" BackColor="#EBF6FE" AllowPaging="True" OnPageIndexChanging="GridView1_PageIndexChanging" DataKeyNames="exitID">
                        <Columns>
                            <asp:BoundField DataField="exitID" HeaderText="ID" />
                            <asp:BoundField DataField="createddate" HeaderText="Requested Date" />
                            <asp:BoundField DataField="exittime" HeaderText="Requested Time" />
                            <asp:BoundField DataField="reason" HeaderText="Reason" />
                        </Columns>
                        <PagerStyle HorizontalAlign = "Right" CssClass = "GridPager" />
                    </asp:GridView>
                <%--</asp:View>
                <asp:View ID="View3" runat="server">
                    <asp:GridView ID="GridView3" GridLines="None" CssClass="table" HeaderStyle-CssClass="thead" RowStyle-CssClass="tr" runat="server" AutoGenerateColumns="False" BackColor="#EBF6FE">
                        <Columns>
                            <asp:BoundField DataField="exitID" HeaderText="ID" />
                            <asp:BoundField DataField="createddate" HeaderText="Requested Date" />
                            <asp:BoundField DataField="exittime" HeaderText="Requested Time" />
                            <asp:BoundField DataField="reason" HeaderText="Reason" />
                        </Columns>
                    </asp:GridView>
                </asp:View>
                <asp:View ID="View4" runat="server">
                   <asp:GridView ID="GridView4" GridLines="None" CssClass="table" HeaderStyle-CssClass="thead" RowStyle-CssClass="tr" runat="server" AutoGenerateColumns="False" BackColor="#EBF6FE">
                        <Columns>
                            <asp:BoundField DataField="exitID" HeaderText="ID" />
                            <asp:BoundField DataField="createddate" HeaderText="Requested Date" />
                            <asp:BoundField DataField="exittime" HeaderText="Requested Time" />
                            <asp:BoundField DataField="reason" HeaderText="Reason" />
                        </Columns>
                    </asp:GridView>
                </asp:View>
            </asp:MultiView>--%>
            
    <%--        <asp:LinkButton ID="LinkButton1" runat="server">LinkButton</asp:LinkButton>--%>
            
<%--            <asp:Label ID="lblHidden" runat="server" Text=""></asp:Label>
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>--%>
            
        </div>
<%--        <ajaxToolkit:ModalPopupExtender ID="mpeApproval" runat="server" TargetControlID="lblHidden" PopupControlID="Panel1" BackgroundCssClass="modalBackground" CancelControlID="btnBack">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="Panel1" runat="server">
               <div class="rowIcon">
                <span id="btnBack">
                    <i class="fa fa-times fa-2x" aria-hidden="true"></i>
                </span>
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
                        <asp:Button id="btnApprove" class="button" runat="server" text="Approve" OnClick="ApproveBtn_Click" />
                        <asp:Button id="btnReject" class="button" runat="server" text="Reject" />
                    </div>
                </div>   
            </asp:Panel>--%>

        <asp:Label ID="Label1" runat="server"></asp:Label>
    </form>
</body>
</html>
