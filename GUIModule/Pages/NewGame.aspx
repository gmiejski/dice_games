<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewGame.aspx.cs" Inherits="GUIModule.Pages.NewGame" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../App_Themes/dice.css" rel="stylesheet" />
    <title>Tworzenie gry</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel runat="server">
            <asp:Table ID="NewGameTable" runat="server" CssClass="horCentered standardWindow">
                <asp:TableRow>
                    <asp:TableCell>
                        Nazwa gry:
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="NewGameName" runat="server"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                        Typ gry:
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:DropDownList ID="NewGameType" runat="server">
                            <asp:ListItem Value="Poker">Poker</asp:ListItem>
                            <asp:ListItem Value="NPlus">N+</asp:ListItem>
                            <asp:ListItem Value="NStar">N*</asp:ListItem>
                        </asp:DropDownList>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                        Liczba graczy:
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="NewGamePlayers" runat="server"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                        Liczba botów:
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="NewGameBots" runat="server"></asp:TextBox>
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
                <asp:Button runat="server" Text="Start" ID="StartGame" OnClick="CreateGame_Click"/>
            </asp:Panel>
        </asp:Panel>
        <asp:Button runat="server" CssClass="bottomRightCorner" ID="LeaveNewGame" OnClick="LeaveNewGame_Click" Text="Powrót"/>
    </form>
</body>
</html>
