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

            <div class="dateTimeRow">

                    <div class="dateCol">
                        <asp:Label class="label" ID="date" runat="server" Text="Date"></asp:Label>
                        <input class="input" id="dateInput" type="date" readonly="true"/>
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
          
            <div class="rowButtons">
<%--                <div class="colCancelBtn">--%>
                    <asp:Button class="button" ID="cancelBtn" runat="server" Text="Cancel" />
<%--                </div>
                <div class="colSubmitBtn">--%>
                    <asp:Button class="button" ID="submitBtn" runat="server" Text="Submit" OnClick="SubmitBtn_Click" />
<%--                </div>--%>
            </div>
        </div>

        <asp:Label ID="Label1" runat="server"></asp:Label>
        <asp:Label ID="Label2" runat="server"></asp:Label>
            
         <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"  AllowPaging="True" PageSize="30"  CssClass="auto-style13">
              <Columns>
                <asp:BoundField DataField="exitID" HeaderText="Exit ID" />
                <asp:BoundField DataField="Employee_Name" HeaderText="Requested by" />
                <asp:BoundField DataField="exittime" HeaderText="Requested time" />
                <asp:BoundField DataField="reason" HeaderText="Reason" />
            </Columns>
       </asp:GridView>
                    
<%--          <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False"  AllowPaging="True" PageSize="30"  CssClass="auto-style13" DataKeyNames="EXITID">
              <Columns>
        <asp:BoundField ItemStyle-Width="100px" DataField="exitID" HeaderText="exitID" ReadOnly="True" >
<ItemStyle Width="100px"></ItemStyle>
            </asp:BoundField> 
        <asp:BoundField ItemStyle-Width="100px" DataField="toexit" HeaderText="toexit" ReadOnly="True" >
<ItemStyle Width="100px"></ItemStyle>
            </asp:BoundField>
        <asp:BoundField ItemStyle-Width="100px" DataField="createdby" HeaderText="createdby" >
<ItemStyle Width="100px"></ItemStyle>
            </asp:BoundField>  
        <asp:BoundField ItemStyle-Width="100px" DataField="createddate" HeaderText="createddate" >
<ItemStyle Width="100px"></ItemStyle>
            </asp:BoundField>
        <asp:BoundField ItemStyle-Width="100px" DataField="reason" HeaderText="reason" >
<ItemStyle Width="100px"></ItemStyle>
            </asp:BoundField>
                   <asp:BoundField ItemStyle-Width="100px" DataField="exittime" HeaderText="exittime" >
<ItemStyle Width="100px"></ItemStyle>
            </asp:BoundField>
                   <asp:BoundField ItemStyle-Width="100px" DataField="projcode" HeaderText="projcode" >
<ItemStyle Width="100px"></ItemStyle>
            </asp:BoundField>
                   <asp:BoundField ItemStyle-Width="100px" DataField="Employee_Name" HeaderText="EmployeeName" >
<ItemStyle Width="100px"></ItemStyle>
            </asp:BoundField>
       </Columns>
       </asp:GridView>--%>
       
    </form>
</body>
</html>
