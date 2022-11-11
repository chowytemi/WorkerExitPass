<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EarlyExitPermitView.aspx.cs" Inherits="WorkerExitPass.WebForm4" EnableEventValidation = "false"%>
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
        <%--<h1>Early Exit Permit Approval</h1>
        <div class="container">--%>
        <div class="header1">
            <h1>Early Exit Permit Approval</h1>
        </div>
        <div class="header">
            <asp:Button ID="EserviceBtn" runat="server" Text="DM E-Services" OnClick="Eservice_Click" />          
            <asp:DropDownList ID="ddlStatus" class="dropdown" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                <asp:ListItem Text="Pending" Value=""></asp:ListItem>
                <asp:ListItem Text="Approved/Rejected"></asp:ListItem>     
            </asp:DropDownList>
        </div>
        <div>
<%--            <div class="row">            
                        <asp:DropDownList ID="ddlStatus" class="dropdown" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                            <asp:ListItem Text="Pending" Value=""></asp:ListItem>
                            <asp:ListItem Text="Approved/Rejected"></asp:ListItem>     
                        </asp:DropDownList>
            </div>--%>
            <asp:GridView ID="GridView1" GridLines="None" CssClass="table" HeaderStyle-CssClass="thead" RowStyle-CssClass="tr" runat="server" AutoGenerateColumns="False" OnRowDataBound="GridView1_RowDataBound" BackColor="#EBF6FE" AllowPaging="True" OnPageIndexChanging="GridView1_PageIndexChanging" DataKeyNames="exitID">
                <Columns>
                    <asp:BoundField DataField="exitID" HeaderText="ID" />
                    <%--<asp:BoundField DataField="createddate" HeaderText="Requested Date" />--%>
                    <asp:BoundField DataField="exittime" HeaderText="Requested Date/Time" />
                    <asp:BoundField DataField="projectdesc" HeaderText="Project" />
                    <asp:BoundField DataField="company" HeaderText="Company" />
                    <asp:BoundField DataField="no of emp" HeaderText="No of Employees" />
<%--                    <asp:BoundField DataField="reason" HeaderText="Reason" />                    --%>
                    </Columns>
                    <PagerStyle HorizontalAlign = "Right" CssClass = "GridPager" />
            </asp:GridView>

            <asp:GridView ID="GridView2" GridLines="None" CssClass="table" HeaderStyle-CssClass="thead" RowStyle-CssClass="tr" runat="server" AutoGenerateColumns="False" AllowPaging="True" OnRowDataBound="GridView2_RowDataBound" OnPageIndexChanging="GridView2_PageIndexChanging" OnSelectedIndexChanged="GridView2_SelectedIndexChanged">
                <Columns>
                    <asp:BoundField DataField="exitID" HeaderText="ID" />
<%--                    <asp:BoundField DataField="reason" HeaderText="Reason" />--%>
                 <%--   <asp:BoundField DataField="approveddate" HeaderText="Approved Date" />--%>
                    <asp:BoundField DataField="projectdesc" HeaderText="Project" />
                    <asp:BoundField DataField="company" HeaderText="Company" />
                    <asp:BoundField DataField="approve" HeaderText="Status" />
                </Columns>
                <HeaderStyle CssClass="thead" />
                <RowStyle CssClass="tr" />
                <PagerStyle HorizontalAlign = "Right" CssClass = "GridPager" />
            </asp:GridView>

            <asp:Label ID="lblHidden" runat="server" Text=""></asp:Label>
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <ajaxToolkit:ModalPopupExtender ID="mpePopUp" runat="server" TargetControlID="lblHidden" PopupControlID="Panel1" BackgroundCssClass="modalBackground">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="Panel1" runat="server">
                <div class="rowIcon">
                    <asp:LinkButton ID="btnBack" runat="server" Text="<i class='fa fa-times fa-2x' aria-hidden='true'></i>" OnClick="btnBack_Click"/>
                </div>                 
                <div class="content">
                        <div class="rowModal">
                            <asp:Label ID="lblexitID" runat="server" />
                        </div>
                        <div id="approval">                                
                            <asp:Label class="label" ID="lblApproval" runat="server" Text="Approval Details"></asp:Label>
                                <div id="statusWhenRow">
                                       <div class="col">
                                            <asp:Label class="label" ID="label" runat="server" Text="Status"></asp:Label>
                                            <asp:Label class="labelData" ID="lblStatus" runat="server" />
                                        </div>
                                        <div class="col">
                                            <asp:Label class="label" runat="server" Text="Approval Date"></asp:Label>
                                            <asp:Label class="labelData" ID="lblWhen" runat="server" />
                                        </div> 
                                    </div>
                                <div class="row">
                                     <asp:Label class="label" ID="empName" runat="server" Text="Name(s) of DM worker/contractor"></asp:Label>
                                     <asp:Label class="labelData" ID="lblEmpName" runat="server"></asp:Label>
                                </div>
                            <asp:Table ID="Table1" runat="server"></asp:Table>
                                <div class="row">
                                        <asp:Label class="label" ID="name" runat="server" Text="Approver"></asp:Label>
                                        <asp:Label class="labelData" ID="lblApprover" runat="server"></asp:Label>
                                 </div>     
                        </div>

                        <div id="details">
                            <asp:Label class="label" ID="lblDetails" runat="server" Text="Form Details"></asp:Label>
                            <div id="dateTimeRow">
                                <div class="col">
                                        <asp:Label class="label" ID="date" runat="server" Text="Date"></asp:Label>
<%--                                        <asp:TextBox class="textbox" ID="tbDate" runat="server" ReadOnly="True"></asp:TextBox>--%>
                                        <asp:Label class="labelData" ID="lblDate" runat="server"></asp:Label>

                                </div>
                                <div class="col">
                                        <asp:Label class="label" ID="time" runat="server" Text="Exit Time"></asp:Label>
                                        <%--<asp:TextBox class="textbox" ID="tbTime" runat="server"></asp:TextBox>--%>
                                        <asp:Label class="labelData" ID="lblTime" runat="server"></asp:Label>

                                </div>
                            </div>
                        <div class="row">
                                <asp:Label class="label" ID="project" runat="server" Text="Project"></asp:Label>
                                <%--<asp:TextBox class="textbox" ID="tbProject" runat="server" ReadOnly="True"></asp:TextBox>--%>
                                <asp:Label class="labelData" ID="lblProject" runat="server"></asp:Label>
                        </div> 
                        <div class="row">
                                <asp:Label class="label" ID="Label3" runat="server" Text="Name(s) of DM worker/contractor"></asp:Label>
<%--                                <asp:TextBox class="textbox" ID="tbName" runat="server" ReadOnly="True"></asp:TextBox>--%>
                                <asp:Label class="labelData" ID="lblName" runat="server"></asp:Label>
                        </div>
                        <div class="row">
                                <asp:Label class="label" ID="companyName" runat="server" Text="Name of Company"></asp:Label>
<%--                                <asp:TextBox class="textbox" ID="tbCompany" runat="server" ReadOnly="True"></asp:TextBox>--%>
                                <asp:Label class="labelData" ID="lblCompany" runat="server"></asp:Label>
                        </div>
                        <div class="row">
                                <asp:Label class="label" ID="reason" runat="server" Text="Reason for Leaving Yard"></asp:Label>
<%--                                <asp:TextBox class="textbox" ID="tbReason" runat="server" ReadOnly="True"></asp:TextBox>--%>
                                <asp:Label class="labelData" ID="lblReason" runat="server"></asp:Label>
                        </div>
                        <div class="row">
                                <asp:Label class="label" ID="remarks" runat="server" Text="Remarks"></asp:Label>
<%--                                <asp:TextBox class="textbox" ID="tbRemarks" runat="server" ReadOnly="True"></asp:TextBox>--%>
                                <asp:Label class="labelData" ID="lblRemarks" runat="server"></asp:Label>

                        </div>
                        </div>  
                </div> 
            </asp:Panel>
        </div>
    </form>

        <%--<asp:Label ID="Label1" runat="server"></asp:Label>--%>

</body>
</html>
