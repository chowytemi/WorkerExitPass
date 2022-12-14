<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EarlyExitPermitTK.aspx.cs" Inherits="WorkerExitPass.WebForm2" EnableEventValidation = "false"%>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
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
        <link href="Content/font-awesome.css" rel="stylesheet" type="text/css" />
        <link href="Content/font-awesome.min.css" rel="stylesheet" type="text/css" />
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
                <div class="rowIcon">
                        <asp:LinkButton ID="btnHelp" runat="server" Text="<i class='fa fa-question-circle fa-2x' aria-hidden='true'></i>"/>

<%--                    <asp:LinkButton ID="btnHelp" runat="server" Text="<i class='fa fa-question-circle fa-2x' aria-hidden='true'></i>" OnClick="btnHelp_Click"/>--%>
                </div>  
                <div class ="row">
                    <div class="col-12">
                        <asp:Label class="label" ID="msg" runat="server" Text="Please select date and time you want to exit at." Visible="false"></asp:Label>
                    </div>
                </div>
                <div class="dateTimeRow">

                    <div class="dateCol">
                        <asp:Label class="label" ID="date" runat="server" Text="Date"></asp:Label>
                        <input class="input" id="dateInput" type="date" runat="server" />
                        <asp:Label class="dateTimeLabel" ID="dateSubmit" runat="server" Visible="false"></asp:Label>
                    </div>
     
                    <div class="timeCol">
                        <asp:Label class="label" ID="time" runat="server" Text="Time"></asp:Label>
                        <input class="input" id="timeInput" type="time" runat="server"/>
                        <asp:Label class="dateTimeLabel" ID="timeSubmit" runat="server" Visible="false"></asp:Label>
                    </div>

                </div>
                <div class="row">
                <asp:Button class="button" ID="nextBtn" runat="server" Text="Next" Visible="false" OnClick="nextBtn_OnClick"/>
                </div>
                <asp:Panel ID="Panel3" runat="server">
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
                            <Texts SelectBoxCaption="Select Employees" />
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
                        <asp:Button class="button" ID="submitAsTeam" runat="server" Text="Submit" Visible="false" OnClick="SubmitAsTeam_Click" />
                        <asp:Button class="button" ID="submitAsSolo" runat="server" Text="Submit" OnClick="SubmitAsSolo_Click" />
                </div>
                </asp:Panel>  
            </div>   
        </div>
<%--           <asp:Label ID="lblHidden" runat="server" Text=""></asp:Label>--%>
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <ajaxToolkit:ModalPopupExtender ID="mpePopUp" runat="server" TargetControlID="btnHelp" PopupControlID="Panel1" CancelControlID="btnBack" BackgroundCssClass="modalBackground">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="Panel1" runat="server">
                <div class="rowIcon">
                    <asp:LinkButton ID="btnBack" runat="server" OnClick="btnBack_Click" Text="<i class='fa fa-times fa-2x' aria-hidden='true'></i>"/>
                </div>                 
                <div class="content">
                    <asp:Label ID="labelHelp" runat="server" Text="When to apply for Early Exit Permit?"></asp:Label>    
                    <div class="contentRow">
                        <ol>
                            <li>You cannot apply from 5pm to 6pm.</li>
                            <li>You cannot leave earlier than the exit timing you applied for.</li>
                            <li>Once exit permit is approved, you have to exit within an hour from the exit time.</li>
                            <li>If your early exit permit has been approved but you did not exit within the valid exit time, please apply again.</li>
                        </ol>
                    </div>
                </div>
        </asp:Panel>   

         <asp:Label ID="Label1" runat="server" style="display:none"></asp:Label>
            <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender1" runat="server" TargetControlID="Label1" PopupControlID="Panel2" CancelControlID="btnContinue" BackgroundCssClass="modalBackground1">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="Panel2" runat="server">   
                <div class="rowIcon">
                    <asp:LinkButton ID="btnContinue" runat="server" Text="<i class='fa fa-times fa-2x' aria-hidden='true'></i>"/>
                </div>  
                <div class="contentRow">
                    <asp:Label ID="labelSuccess" runat="server"></asp:Label>
                    <asp:Label class="labelMsg" ID="valid" runat="server"></asp:Label>
                    <asp:Button ID="viewStatusBtn" runat="server" Text="View Status" OnClick="viewStatus_Click" />
                </div>
        </asp:Panel>   
    </form>
                            <asp:Label ID="Label12" runat="server"></asp:Label>

</body>
</html>
