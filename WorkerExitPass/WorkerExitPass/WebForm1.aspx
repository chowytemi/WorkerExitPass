<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WorkerExitPass.WebForm1" %>

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
</head>
    
<body>
    <form id="form1" runat="server">
        <h1>Early Exit Permit Form</h1>
        <div class="container">

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
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*" ControlToValidate="projectddl" CssClass="rfv"></asp:RequiredFieldValidator>
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

                    </div>
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
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*" ControlToValidate="ReasonDropdown" InitialValue="-1" CssClass="rfv"></asp:RequiredFieldValidator>

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
                       <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="*" ControlToValidate="remarkstb" CssClass="rfv"></asp:RequiredFieldValidator>

                       <asp:TextBox class="inputFields" ID="remarkstb" runat="server" Visible="false"></asp:TextBox>
                    </div>
                </div>
            </div>
          
            <div class="rowButtons">
                <div class="colCancelBtn">
                    <asp:Button class="button" ID="cancelBtn" runat="server" Text="Cancel" />
                </div>
                <div class="colSubmitBtn">
                    <asp:Button class="button" ID="submitBtn" runat="server" Text="Submit" OnClick="SubmitBtn_Click" />
                </div>
            </div>
        </div>
       


                    <%--<asp:TextBox class="inputFields" ID="TextBox1" runat="server" ReadOnly="true"></asp:TextBox>
                    <asp:TextBox class="inputFields" ID="TextBox2" runat="server" ReadOnly="true"></asp:TextBox>
                    <asp:TextBox class="inputFields" ID="TextBox3" runat="server" ReadOnly="true"></asp:TextBox>
                    <asp:TextBox class="inputFields" ID="TextBox4" runat="server" ReadOnly="true"></asp:TextBox>
                    <asp:TextBox class="inputFields" ID="TextBox5" runat="server" ReadOnly="true"></asp:TextBox>--%>

        <asp:Label ID="Label1" runat="server"></asp:Label>
        <asp:Label ID="Label2" runat="server"></asp:Label>
            
        <%-- <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"  AllowPaging="True" PageSize="30"  CssClass="auto-style13" DataKeyNames="createddate">
              <Columns>
        <asp:BoundField ItemStyle-Width="100px" DataField="createddate" HeaderText="createddate" ReadOnly="True" >
<ItemStyle Width="100px"></ItemStyle>
            </asp:BoundField> 
        <asp:BoundField ItemStyle-Width="100px" DataField="exittime" HeaderText="exittime" ReadOnly="True" >
<ItemStyle Width="100px"></ItemStyle>
            </asp:BoundField>
        <asp:BoundField ItemStyle-Width="100px" DataField="approve" HeaderText="approve" >
<ItemStyle Width="100px"></ItemStyle>
            </asp:BoundField>
       </Columns>
       </asp:GridView>--%>
                    
         
       
    </form>
</body>
</html>
