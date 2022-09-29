<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm3.aspx.cs" Inherits="WorkerExitPass.WebForm3" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>View Past Early Exit Permits</title>
    <link href="https://fonts.googleapis.com/css2?family=Open+Sans&display=swap" rel="stylesheet"/>
    <link href="https://fonts.googleapis.com/css2?family=Rubik:wght@700&display=swap" rel="stylesheet" />
    <style type="text/css">
        body {

        }
        h1 {
            color: #004B7A;
            font-family: 'Rubik', sans-serif;
            text-align: center;
            font-size: 2.375em;
        }
        th {
            font-family: 'Open Sans', sans-serif;
            font-size: 1em;
        }
        table {
            width: 90%;
            margin: auto;
            color: #004B7A;
            table-layout: fixed;
        }
        thead {
            background-color: #F1FAFF;
            border-radius: 0.625em  0.625em 0 0;
        }
        tbody {
            background-color: #FDFEFF;
            border-radius: 0.625em;
            text-align: center;
        }
        tr {
            height: 3em;
        }

        @media only screen and (max-width: 480px) {
            h1 {
                font-size: 1.5em;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h1>View Past Early Exit Permits</h1>
        <div>
            <table class="table">
                <thead>
                    <tr>
                        <th class="col">ID</th>
                        <th class="col">Date</th>
                        <th class="col">Time</th>
                        <th class="col">Status</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                    </tr>
                </tbody>
                
                
            </table>
        </div>
    </form>
</body>
</html>
