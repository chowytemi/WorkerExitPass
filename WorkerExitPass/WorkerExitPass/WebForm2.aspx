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
<body">
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
                        <div class="form-group">
                            <asp:Label class="label" ID="project" runat="server" Text="Project"></asp:Label>
    <%--                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*" ControlToValidate="projectddl" CssClass="rfv"></asp:RequiredFieldValidator>--%>
                            <asp:DropDownList class="dropdown" ID="projectddl" runat="server" AutoPostBack="true">
                            </asp:DropDownList>
                        </div>        
                    </div>
                </div>
            
                <div class="row">
                    <div class="col-12">
                        <div class="form-group">
                            <asp:Label class="label" ID="name" runat="server" Text="Name(s) of DM worker/contractor"></asp:Label>
                            <asp:TextBox class="inputFields" ID="nametb" runat="server" ReadOnly="true"></asp:TextBox>
                            <asp:DropDownList class="dropdown" ID="namesddl" runat="server" Visible="false"></asp:DropDownList></div>
                    </div>
                </div>
           
                <div class="row">
                    <div class="col-12">
                        <div class="form-group">
                            <asp:Label class="label" ID="companyName" runat="server" Text="Name of Company"></asp:Label>
                            <asp:TextBox class="inputFields" ID="companytb" runat="server" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>
                </div>
            
                <div class="row">
                    <div class="col-12">
                        <div class="form-group">
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
                </div>
            
                <div class="row">
                    <div class="col-12">
                        <div class="form-group">
                            <asp:Label class="label" ID="lblremarks" runat="server" Text="Remarks" Visible="false"></asp:Label>
    <%--                       <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="*" ControlToValidate="remarkstb" CssClass="rfv"></asp:RequiredFieldValidator>--%>

                           <asp:TextBox class="inputFields" ID="remarkstb" runat="server" Visible="false"></asp:TextBox>
                        </div>
                    </div>
                </div>
            
                <%--<div class="row">
                   <asp:Button class="button" ID="cancelBtn" runat="server" Text="Cancel" />
                   <asp:Button class="button" ID="submitBtn" runat="server" Text="Submit" />
                </div>--%>
                <div class="rowButtons">
                    <div class="colCancelBtn">
                        <asp:Button class="button" ID="cancelBtn" runat="server" Text="Cancel" />
                    </div>
                    <div class="colSubmitBtn">
                        <asp:Button class="button" ID="submitBtn" runat="server" Text="Submit" />
                    </div>
                </div>
            </div>
                <%--<asp:MultiView 
                        ID="MultiView1"
                        runat="server" OnActiveViewChanged="MultiView1_ActiveViewChanged">
                        <asp:View ID="SoloView" runat="server"  >
                            <div class="form">
                                
                                    <div class="dateTimeRow">
                                        <div class="dateCol">
                                                <asp:Label class="label" ID="date" runat="server" Text="Date"></asp:Label>
                                                <input class="inputFields" id="dateInput" type="date" />
                                        </div>
          
                                        <div class="timeCol">
                                                <asp:Label class="label" ID="time" runat="server" Text="Time"></asp:Label>
                                                <input class="inputFields" id="timeInput" type="time" />
                                        </div>
                                    </div>
                            
                                <div class="row">
                                    <div class="col-12">
                                        <div class="form-group">
                                            <asp:Label class="label" ID="project" runat="server" Text="Project"></asp:Label>
                                        
                                            <asp:DropDownList class="dropdown" ID="ProjectDropdown" runat="server" AutoPostBack="true">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
            
                                <div class="row">
                                    <div class="col-12">
                                        <div class="form-group">
                                            <asp:Label class="label" ID="name" runat="server" Text="Name(s) of DM worker/contractor"></asp:Label>
                                            <asp:TextBox class="inputFields" ID="nameInput" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
           
                                <div class="row">
                                    <div class="col-12">
                                        <div class="form-group">
                                            <asp:Label class="label" ID="companyName" runat="server" Text="Name of Company"></asp:Label>
                                            <input class="inputFields" id="companyInput" type="text" />
                                        </div>
                                    </div>
                                </div>
            
                                <div class="row">
                                    <div class="col-12">
                                        <div class="form-group">
                                            <asp:Label class="label" ID="reason" runat="server" Text="Reason for Leaving Yard"></asp:Label>
   
                                                <asp:DropDownList class="dropdown" ID="ReasonDropdown" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ReasonDropdown_SelectedIndexChanged">
                                                <asp:ListItem Text="Select" Value="-1"></asp:ListItem>
                                                <asp:ListItem Text="Medical injury" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Weather conditions" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Emergency" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Go office" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Others" Value="4"></asp:ListItem>
                                            </asp:DropDownList>

                                        </div>
                                    </div>
                                </div>
            
                                <div class="row">
                                    <div class="col-12">
                                        <div class="form-group">
                                           <asp:Label class="label" ID="remarks" runat="server" Text="Remarks" Visible="false"></asp:Label>

                                           <asp:TextBox class="inputFields" ID="remarksInput" runat="server" Visible="false"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            
                        </div>
                        </asp:View>
                        <asp:View ID="TeamView" runat="server">
                            <div class="form">
                                <div class="row">
                                        <div class="col">
                    
                                            <asp:Label class="label" ID="Label1" runat="server" Text="Date"></asp:Label>
                                            <input class="inputFields" id="dateInput" type="date" />
                                        </div>
          
                                        <div class="col">
                                            <asp:Label class="label" ID="Label2" runat="server" Text="Time"></asp:Label>
                                            <input class="inputFields" id="timeInput" type="time" />
                                        </div>
                                </div>

                                <div class="row">
                                    <div>
                                        <asp:Label class="label" ID="Label3" runat="server" Text="Project"></asp:Label>
                                        
                                        <asp:DropDownList class="dropdown" ID="DropDownList1" runat="server" AutoPostBack="true">
                                        </asp:DropDownList>
                                    </div>
                                </div>
            
                                <div class="row">
                                    <div>
                                        <asp:Label class="label" ID="Label4" runat="server" Text="Name(s) of DM worker/contractor"></asp:Label>
                                        <asp:DropDownList class="dropdown" ID="DropDownList3" runat="server" AutoPostBack="true">
                                        </asp:DropDownList>
                                    </div>
                                </div>
           
                                <div class="row">
                                    <div>
                                        <asp:Label class="label" ID="Label5" runat="server" Text="Name of Company"></asp:Label>
                                        <input class="inputFields" id="Text4" type="text" />
                                    </div>
                                </div>
            
                                <div class="row">
                                    <div>
                                        <asp:Label class="label" ID="Label6" runat="server" Text="Reason for Leaving Yard"></asp:Label>
   
                                            <asp:DropDownList class="dropdown" ID="DropDownList2" runat="server" AutoPostBack="true">
                                            <asp:ListItem Text="Select" Value="-1"></asp:ListItem>
                                            <asp:ListItem Text="Medical injury" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Weather conditions" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="Emergency" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="Go office" Value="3"></asp:ListItem>
                                            <asp:ListItem Text="Others" Value="4"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
            
                                <div class="row">
                                    <div>
                                       <asp:Label class="label" ID="Label7" runat="server" Text="Remarks" Visible="false"></asp:Label>

                                       <asp:TextBox class="inputFields" ID="TextBox2" runat="server" Visible="false"></asp:TextBox>
                                    </div>
                                </div>
         
                         
                            </div>
                        </asp:View>
                 </asp:MultiView>--%>
            
            
        </div>
        
        

    </form>
</body>
</html>
