<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WorkerExitPass.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Early Exit Permit Form</title>
    <link href="https://fonts.googleapis.com/css2?family=Poppins&display=swap" rel="stylesheet"/>
    <link href="https://fonts.googleapis.com/css2?family=Open+Sans&display=swap" rel="stylesheet"/>
    <link href="https://fonts.googleapis.com/css2?family=Rubik&display=swap" rel="stylesheet" />
    <style type="text/css">
    
        .row {
            margin-top: 20px;
            display: flex;
            justify-content: center;
            align-items: center;
           
        }
        #dateInput, #timeInput {
            width: 150px;
        }
        body {
             background-color: #F1FAFF;
        }
        h1, .label {
            color: #004B7A;
            font-family: 'Rubik', sans-serif;
        }
        h1 {
            text-align: center;
            font-weight: bolder;
            font-size: 38px;
        }
        .label {
            font-weight: bold;
            font-size: 18px;
            line-height: 30px;
            display: block;
            margin-top: 20px;
        }
        .container {
            height: 800px;
            width: 1271px;
            background-color: white;
            margin: auto;
            box-shadow: 0px 2px 4px rgba(0, 75, 122, 0.25);
            border-radius: 10px;
        }

        .inputFields, .dropdown {
            height: 40px;
            width: 400px;
            border-radius: 5px;
            border: 1px solid #EFF0F6;
            font-family: 'Open Sans', sans-serif;
            font-size: 16px;
            display: flex;
            justify-content: center;
            text-indent: 8px;
            box-shadow: 0px 2px 4px rgba(0, 75, 122, 0.25);
        }
        
        .inputFields:hover, .dropdown:hover {
            border: 1px solid #004B7A;
        }
        .inputFields:focus, .dropdown:focus {
            outline: none;
        }
        .button {
            display: block;
            border-radius:15px;
            border-style: none;
            color: white;
            height: 60px;
            width: 180px;
            font-family: 'Rubik', sans-serif;
            font-size: 20px;
            font-weight: bold;

        }
        #cancelBtn {
            margin-left: 84.5px;
            float: left;
            background-color: #E83333;
        }
      
        #submitBtn {
            margin-right: 84.5px;
            float: right;
            background-color: #00AC26;
        }

        .rfv {
            color: red;
        }



    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Early Exit Permit Form</h1>
        <div class="container">

            <div class="row">
                <div class="col">
                    <asp:Label class="label" ID="date" runat="server" Text="Date"></asp:Label>
                    <input class="inputFields" id="dateInput" type="date" />
                </div>
          
                <div class="col2">
                    <asp:Label class="label" ID="time" runat="server" Text="Time"></asp:Label>
                    <input class="inputFields" id="timeInput" type="time" />
                </div>
            </div>
            
            <div class="row">
                <div>
                    <asp:Label class="label" ID="project" runat="server" Text="Project"></asp:Label>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*" ControlToValidate="ProjectDropdown" CssClass="rfv"></asp:RequiredFieldValidator>
                    <asp:DropDownList class="dropdown" ID="ProjectDropdown" runat="server" AutoPostBack="true">
                    </asp:DropDownList>
                </div>
            </div>
            
            <div class="row">
                <div>
                    <asp:Label class="label" ID="name" runat="server" Text="Name(s) of DM worker/contractor"></asp:Label>
                    <asp:TextBox class="inputFields" ID="nameInput" runat="server"></asp:TextBox>
                </div>
            </div>
           
            <div class="row">
                <div>
                    <asp:Label class="label" ID="companyName" runat="server" Text="Name of Company"></asp:Label>
                    <input class="inputFields" id="Text4" type="text" />
                </div>
            </div>
            
            <div class="row">
                <div>
                    <asp:Label class="label" ID="reason" runat="server" Text="Reason for Leaving Yard"></asp:Label>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*" ControlToValidate="ReasonDropdown" InitialValue="-1" CssClass="rfv"></asp:RequiredFieldValidator>
   
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
            
            <div class="row">
                <div>
                   <asp:Label class="label" ID="remarks" runat="server" Text="Remarks" Visible="false"></asp:Label>
                   <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="*" ControlToValidate="remarksInput" CssClass="rfv"></asp:RequiredFieldValidator>

                   <asp:TextBox class="inputFields" ID="remarksInput" runat="server" Visible="false"></asp:TextBox>
                </div>
            </div>
          
            <div class="rowButtons">
                <asp:Button class="button" ID="cancelBtn" runat="server" Text="Cancel" />
                <asp:Button class="button" ID="submitBtn" runat="server" Text="Submit" OnClick="SubmitBtn_Click" />
            </div>
        </div>


            
       
    </form>
</body>
</html>
