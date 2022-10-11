<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm2.aspx.cs" Inherits="WorkerExitPass.WebForm2" %>

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
</head>

<body>
    <form id="form1" runat="server">
        <h1>Early Exit Permit Form</h1>
        <div class="container">
            
            <div class="submitAs">
                <asp:Button class="submitAsButton" ID="SoloBtn" runat="server" Text="Solo" OnClick="SoloBtn_Click" />
                <asp:Button class="submitAsButton" ID="TeamBtn" runat="server" Text="Team" OnClick="TeamBtn_Click" />
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
    <%--                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*" ControlToValidate="projectddl" CssClass="rfv"></asp:RequiredFieldValidator>--%>
                            <asp:DropDownList class="dropdown" ID="projectddl" runat="server" AutoPostBack="true">
                            </asp:DropDownList>       
                    </div>
                </div>
            
                <div class="row">
                    <div class="col-12">
                            <asp:Label class="label" ID="name" runat="server" Text="Name(s) of DM worker/contractor"></asp:Label>
                            <asp:TextBox class="inputFields" ID="nametb" runat="server" ReadOnly="true"></asp:TextBox>
                            <%--<asp:DropDownList class="dropdown" ID="namesddl" runat="server" Visible="false">

                            </asp:DropDownList>--%>
                        <%--<asp:ListBox ID="lstNames" runat="server" SelectionMode="Multiple"></asp:ListBox>--%>
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
    <%--                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*" ControlToValidate="ReasonDropdown" InitialValue="-1" CssClass="rfv"></asp:RequiredFieldValidator>--%>

                                <asp:DropDownList class="dropdown" ID="ReasonDropdown" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ReasonDropdown_SelectedIndexChanged">
                                <asp:ListItem Text="Select" Value="Select"></asp:ListItem>
                                <asp:ListItem Text="Medical injury" Value="Medical Injury"></asp:ListItem>
                                <asp:ListItem Text="Weather conditions" Value="Weather Conditions"></asp:ListItem>
                                <asp:ListItem Text="Emergency" Value="Emergency"></asp:ListItem>
                                <asp:ListItem Text="Go office" Value="Go office"></asp:ListItem>
                                <asp:ListItem Text="Others" Value="Others"></asp:ListItem>
                            </asp:DropDownList>
                    </div>
                </div>
            
                <div class="row">
                    <div class="col-12">
                            <asp:Label class="label" ID="lblremarks" runat="server" Text="Remarks" Visible="false"></asp:Label>
    <%--                       <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="*" ControlToValidate="remarkstb" CssClass="rfv"></asp:RequiredFieldValidator>--%>

                           <asp:TextBox class="inputFields" ID="remarkstb" runat="server" Visible="false"></asp:TextBox>
                    </div>
                </div>
            
                <%--<div class="row">
                   <asp:Button class="button" ID="cancelBtn" runat="server" Text="Cancel" />
                   <asp:Button class="button" ID="submitBtn" runat="server" Text="Submit" />
                </div>--%>
                <div class="rowButtons">
<%--                    <div class="colCancelBtn">--%>
                        <asp:Button class="button" ID="cancelBtn" runat="server" Text="Cancel" />
<%--                    </div>
                    <div class="colSubmitBtn">--%>
                        <asp:Button class="button" ID="submitBtn" runat="server" Text="Submit" OnClick="Submit" />
<%--                    </div>--%>
                </div>
            </div>   
        </div>
        
        

    </form>
</body>
</html>
