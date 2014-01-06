<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewGame.aspx.cs" Inherits="GUIModule.Pages.NewGame" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../App_Themes/dice.css" rel="stylesheet" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel runat="server">
            <asp:Table ID="newGameTable" runat="server" CssClass="horCentered standardWindow">
                <asp:TableRow>
                    <asp:TableCell>
                        Nazwa gry:
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="newGameName" runat="server"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                        Typ gry:
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:DropDownList ID="newGameType" runat="server">
                            <asp:ListItem>Poker</asp:ListItem>
                            <asp:ListItem>N+</asp:ListItem>
                            <asp:ListItem>N*</asp:ListItem>
                        </asp:DropDownList>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                        Liczba graczy:
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="newGamePlayers" runat="server"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                        Liczba botów:
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="newGameBots" runat="server"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                        Poziom botów:
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:RadioButton ID="BotLevelEasy" runat="server" GroupName="botLevel" Text="Łatwy" />
                        <asp:RadioButton ID="BotLevelHard" runat="server" GroupName="botLevel" Text="Trudny" />
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            <asp:Panel runat="server" CssClass="horCentered">
                <asp:Button runat="server" Text="Start" OnClick="CreateGame_Click"/>
            </asp:Panel>
        </asp:Panel>
        <asp:Button runat="server" CssClass="bottomRightCorner" OnClick="LeaveNewGame_Click" Text="Powrót"/>
    </form>
</body>
</html>
