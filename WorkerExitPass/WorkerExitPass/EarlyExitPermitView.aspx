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
        <h1>Early Exit Permit Approval</h1>
        <div class="container">
                    <div class="row">            
                        <asp:DropDownList ID="ddlReason" class="dropdown" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlReason_SelectedIndexChanged">
                            <asp:ListItem Text="Reason" Value=""></asp:ListItem>
                            <asp:ListItem Text="Weather conditions" Value="Weather Conditions"></asp:ListItem>
                            <asp:ListItem Text="Emergency" Value="Emergency"></asp:ListItem>
                            <asp:ListItem Text="Go office" Value="Go office"></asp:ListItem>
                            <asp:ListItem Text="Others" Value="Others"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <asp:GridView ID="GridView1" GridLines="None" CssClass="table" HeaderStyle-CssClass="thead" RowStyle-CssClass="tr" runat="server" AutoGenerateColumns="False" OnRowDataBound="GridView1_RowDataBound" BackColor="#EBF6FE" AllowPaging="True" OnPageIndexChanging="GridView1_PageIndexChanging" DataKeyNames="exitID">
                        <Columns>
                            <asp:BoundField DataField="exitID" HeaderText="ID" />
                            <asp:BoundField DataField="createddate" HeaderText="Requested Date" />
                            <asp:BoundField DataField="exittime" HeaderText="Requested Time" />
                            <asp:BoundField DataField="reason" HeaderText="Reason" />
                        </Columns>
                        <PagerStyle HorizontalAlign = "Right" CssClass = "GridPager" />
                    </asp:GridView>
               
        </div>
       

    </form>
</body>
</html>
