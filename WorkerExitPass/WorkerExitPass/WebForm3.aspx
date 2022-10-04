<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm3.aspx.cs" Inherits="WorkerExitPass.WebForm3" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>View Past Early Exit Permits</title>
    <link href="https://fonts.googleapis.com/css2?family=Open+Sans&display=swap" rel="stylesheet"/>
    <link href="https://fonts.googleapis.com/css2?family=Rubik:wght@700&display=swap" rel="stylesheet" />
    <link href="Content/StyleSheet3.css" rel="stylesheet" type="text/css" />

</head>
<body>
    <form id="form1" runat="server">
        <h1>View Past Early Exit Permits</h1>
        <div>

            <asp:GridView ID="GridView1" GridLines="None" CssClass="table" HeaderStyle-CssClass="thead" RowStyle-CssClass="tr" runat="server" AutoGenerateColumns="False" OnRowDataBound="GridView1_RowDataBound" AllowPaging="True" OnPageIndexChanging="GridView1_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="createddate" HeaderText="Date" />
                    <asp:BoundField DataField="exittime" HeaderText="Time" />
                    <asp:BoundField DataField="approve" HeaderText="Status" />
                </Columns>
                <HeaderStyle CssClass="thead" />
                <RowStyle CssClass="tr" />
                <PagerStyle HorizontalAlign = "Right" CssClass = "GridPager" />
            </asp:GridView>
            
        </div>
    </form>
</body>
</html>
