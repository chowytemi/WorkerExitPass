<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EarlyExitPermitTK.aspx.cs" Inherits="WorkerExitPass.WebForm2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Early Exit Permit Form</title>
        <link href="https://fonts.googleapis.com/css2?family=Open+Sans&display=swap" rel="stylesheet"/>
        <link href="https://fonts.googleapis.com/css2?family=Rubik:wght@700&display=swap" rel="stylesheet" />
        <link href="Content/main.css" rel="stylesheet" type="text/css" />
        <link href="Content/StyleSheet2.css" rel="stylesheet" type="text/css" />
        <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>

    <script>
        $(document).ready( function() {
            var now = new Date();
 
            var day = ("0" + now.getDate()).slice(-2);
            var month = ("0" + (now.getMonth() + 1)).slice(-2);
            var today = now.getFullYear()+"-"+(month)+"-"+(day) ;

            $('#dateInput').val(today);
        });
    </script>
</head>

<body>
    <form id="form1" runat="server">
        <h1>Early Exit Permit Form</h1>
        <div class="container">
            
            <div class="submitAs">
                <div class="submitLbl">
                    <asp:Label ID="lblSubmitFor" runat="server" Text="Submit for"></asp:Label>
                </div>
                <div class="submitBtns">
                    <asp:Button ID="SoloBtn" runat="server" Text="Myself" OnClick="SoloBtn_Click" CssClass="activeBtn" />
                    <asp:Button ID="TeamBtn" runat="server" Text="Team" OnClick="TeamBtn_Click" CssClass="submitAsButton"/>
                </div>
                
            </div>

            <div class="form">
                <div class="dateTimeRow">

                    <div class="dateCol">
                        <asp:Label class="label" ID="date" runat="server" Text="Date"></asp:Label>
                        <input class="input" id="dateInput" type="date"/>
                    </div>
     
                    <div class="timeCol">
                        <asp:Label class="label" ID="time" runat="server" Text="Time"></asp:Label>
                        <input class="input" id="timeInput" type="time" runat="server"/>
                    </div>

                </div>

                <div class="row">
                    <div class="col-12">
                            <asp:Label class="label" ID="project" runat="server" Text="Project"></asp:Label>
                            <asp:DropDownList class="dropdown" ID="projectddl" runat="server" AutoPostBack="true">
                            </asp:DropDownList>       
                    </div>
                </div>
            
                <div class="row">
                    <div class="col-12">
                            <asp:Label class="label" ID="name" runat="server" Text="Name(s) of DM worker/contractor"></asp:Label>
                            <asp:TextBox class="inputFields" ID="nametb" runat="server" ReadOnly="true"></asp:TextBox>
                        <asp:DropDownCheckBoxes ID="namesddl" runat="server" Visible="false" CssClass="dropdown" UseButtons="True" OnSelectedIndexChanged="namesddl_SelectedIndexChanged">
       
                            <Style SelectBoxWidth="" DropDownBoxBoxWidth="" DropDownBoxBoxHeight="" SelectBoxCssClass="dropdown"></Style>

                            <Style2 SelectBoxWidth="" DropDownBoxBoxWidth="" DropDownBoxBoxHeight="" SelectBoxCssClass="dropdown"></Style2>
                             
                            <Texts SelectAllNode="Select all"></Texts>
       
                        </asp:DropDownCheckBoxes>
                    </div>
                </div>
           
                <div class="row">
                    <div class="col-12">
                            <asp:Label class="label" ID="companyName" runat="server" Text="Name of Company"></asp:Label>
                            <asp:TextBox class="inputFields" ID="companytb" runat="server" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            
                <div class="row">
                    <div class="col-12">
                            <asp:Label class="label" ID="reason" runat="server" Text="Reason for Leaving Yard"></asp:Label>

                                <asp:DropDownList class="dropdown" ID="ReasonDropdown" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ReasonDropdown_SelectedIndexChanged">
                                <%--<asp:ListItem Text="Select" Value="Select"></asp:ListItem>
                                <asp:ListItem Text="Medical injury" Value="Medical Injury"></asp:ListItem>
                                <asp:ListItem Text="Weather conditions" Value="Weather Conditions"></asp:ListItem>
                                <asp:ListItem Text="Emergency" Value="Emergency"></asp:ListItem>
                                <asp:ListItem Text="Go office" Value="Go office"></asp:ListItem>
                                <asp:ListItem Text="Others" Value="Others"></asp:ListItem>--%>
                            </asp:DropDownList>
                    </div>
                </div>
            
                <div class="row">
                    <div class="col-12">
                            <asp:Label class="label" ID="lblremarks" runat="server" Text="Remarks" Visible="false"></asp:Label>
                           <asp:TextBox class="inputFields" ID="remarkstb" runat="server" Visible="false"></asp:TextBox>
                    </div>
                </div>

                <div class="rowButtons">
                        <asp:Button class="button" ID="cancelBtn" runat="server" Text="Cancel" OnClick="CancelBtn_Click" />
<%--                        <asp:Button class="button" ID="submitBtn" runat="server" Text="Submit" OnClick="Submit" />--%>
                        <asp:Button class="button" ID="submitAsTeam" runat="server" Text="Submit" Visible="false" OnClick="SubmitAsTeam_Click" />
                        <asp:Button class="button" ID="submitAsSolo" runat="server" Text="Submit" OnClick="SubmitAsSolo_Click" />
                </div>
            </div>   
        </div>
    </form>
</body>
</html>
