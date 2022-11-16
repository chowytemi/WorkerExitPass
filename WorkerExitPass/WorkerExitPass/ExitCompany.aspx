<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExitCompany.aspx.cs" Inherits="WorkerExitPass.ExitCompany" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Exit Company</title>
    <link href="https://fonts.googleapis.com/css2?family=Open+Sans&display=swap" rel="stylesheet"/>
    <link href="https://fonts.googleapis.com/css2?family=Rubik:wght@700&display=swap" rel="stylesheet" />
    <link href="Content/StyleSheet5.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>Exit Company</h1>

            <div class="container">
                <div class="row">
                    <div class="col-12">
                     <asp:Label class="label" ID="label" runat="server" Text="Employee ID"></asp:Label>
                     <input class="input" id="lblEmpID" type="text"/>
                    </div>
                </div>
                <div class="row">
                      <div class="col-12">
                      <asp:Label class="label" ID="label1" runat="server" Text="Name of Company"></asp:Label>
                      <asp:DropDownCheckBoxes ID="companyddl" runat="server" CssClass="dropdown" UseButtons="True" OnSelectedIndexChanged="companyddl_SelectedIndexChanged">
       
                                <Style SelectBoxWidth="" DropDownBoxBoxWidth="" DropDownBoxBoxHeight="" SelectBoxCssClass="dropdown"></Style>

                                <Style2 SelectBoxWidth="" DropDownBoxBoxWidth="" DropDownBoxBoxHeight="" SelectBoxCssClass="dropdown"></Style2>
                                    
                     </asp:DropDownCheckBoxes>
                     </div>
                </div>
                <div class="row">
                    <div class="col-12">
                    <asp:Button class="button" ID="submitBtn" runat="server" Text="Submit" />
                    </div>
                </div>

            </div>
        </div>
    </form>
</body>
</html>
