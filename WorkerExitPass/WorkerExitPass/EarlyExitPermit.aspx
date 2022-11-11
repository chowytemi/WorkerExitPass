<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EarlyExitPermit.aspx.cs" Inherits="WorkerExitPass.WebForm1" EnableEventValidation = "false"%>
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
    <link href="Content/StyleSheet1.css" rel="stylesheet" type="text/css" />
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

            <div class="rowIcon">
<%--                <asp:LinkButton ID="btnHelp" runat="server" Text="<i class='fa fa-question-circle fa-2x' aria-hidden='true'></i>" OnClick="btnHelp_Click"/>--%>
                    <asp:LinkButton ID="btnHelp" runat="server" Text="<i class='fa fa-question-circle fa-2x' aria-hidden='true'></i>"/>
            </div>   

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
                    <asp:Button class="button" ID="submitBtn" runat="server" Text="Submit" OnClick="SubmitBtn_Click" />
            </div>
        </div>
                    
<%--         <asp:Label ID="lblHidden" runat="server" Text=""></asp:Label>--%>
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <ajaxToolkit:ModalPopupExtender ID="mpePopUp" runat="server" TargetControlID="btnHelp" PopupControlID="Panel1" CancelControlID="btnBack" BackgroundCssClass="modalBackground">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="Panel1" runat="server">
                <div class="rowIcon">
                    <asp:LinkButton ID="btnBack" runat="server" Text="<i class='fa fa-times fa-2x' aria-hidden='true'></i>"/>
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

         <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
            <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender1" runat="server" TargetControlID="Label1" PopupControlID="Panel2" CancelControlID="btnContinue" BackgroundCssClass="modalBackground1">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="Panel2" runat="server">  
                <div class="rowIcon">
                    <asp:LinkButton ID="btnContinue" runat="server" Text="<i class='fa fa-times fa-2x' aria-hidden='true'></i>"/>
                </div>  
                <div class="contentRow">
                    <asp:Label ID="labelSuccess" runat="server" Text="Success!"></asp:Label>
                    <asp:Label class="labelMsg" ID="valid" runat="server"></asp:Label>
                    <asp:Button ID="viewStatusBtn" runat="server" Text="View Status" OnClick="viewStatus_Click" />
                </div>
        </asp:Panel>   
    </form>
    <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
</body>
</html>
