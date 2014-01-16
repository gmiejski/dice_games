<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="GUIModule.App_Code.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../App_Themes/dice.css" rel="stylesheet" />
    <title>Logowanie</title>
</head>
<body>
    <form id="loginForm" runat="server">
        <asp:Panel runat="server" class="standardWindow centered ">
            <asp:Label runat="server" CssClass="horCentered">Proszę podać nazwę użytkownika</asp:Label><br />
            <asp:TextBox ID="LoginName" runat="server"></asp:TextBox>
            <asp:Button ID="LoginConfirm" runat="server" CssClass="loginButton" Text="Wejdź" OnClick="LoginConfirm_Click" />
        </asp:Panel>
    </form>
</body>
</html>
