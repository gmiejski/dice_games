<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="GUIModule.Pages.Main" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Lista gier</title>
    <link href="../App_Themes/dice.css" rel="stylesheet" />
    
</head>
<body>
    
    <form id="GamesList" runat="server">
    <div>
        <asp:Panel runat="server" CssClass="horCentered">
            <asp:Label runat="server" ID="HelloText" CssClass="helloText">
                Witaj, <%= PlayerName %>!
            </asp:Label>
        </asp:Panel>
        <asp:GridView runat="server" ID="AvailableGamesTable" CssClass="standardWindow horCentered" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField DataField="GameName" HeaderText="Nazwa" />
                <asp:BoundField DataField="OwnerName" HeaderText="Twórca" />
                <asp:TemplateField HeaderText="Gracze" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<%# Eval("PlayerNames.Count") %>'></asp:Label>
                        <asp:Label runat="server" Text='/'></asp:Label>
                        <asp:Label runat="server" Text='<%# Eval("NumberOfPlayers") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField ItemStyle-HorizontalAlign="Right" DataField="NumberOfBots" HeaderText="Boty" />
                <asp:BoundField DataField="GameType" HeaderText="Typ" />
                <asp:BoundField DataField="BotLevel" HeaderText="Poziom botów" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:Button runat="server" ID="joinGame" CssClass="joinGameButton" Text='Dołącz!'
                            OnClick="JoinGame_Click" CommandArgument='<%# Eval("GameName") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:Panel runat="server" CssClass="horCentered">
            <asp:Button runat="server" Text="Załóż grę" OnClick="NewGame_Click" CssClass="bottom"/>
        </asp:Panel>
        <asp:Panel runat="server" CssClass="bottomRightCorner" >
            <asp:Button runat="server" ID="LogoutButton" Text="Wyloguj" OnClick="Logout_Click" />
        </asp:Panel>
    </div>
    </form>
</body>
</html>
