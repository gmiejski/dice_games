<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="Gui.Pages.Main" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../App_Themes/dice.css" rel="stylesheet" />
    
</head>
<body>
    
    <form id="gamesList" runat="server">
    <div>
        Witaj, <%= PlayerName %>!
        <asp:GridView runat="server" ID="availableGamesTable" AutoGenerateColumns="false">
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
                            OnClick="JoinGame_Click" CommandArgument="<%# Container.DisplayIndex %>" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:Panel runat="server" CssClass="horCentered">
            <asp:Button runat="server" Text="Załóż grę" OnClick="NewGame_Click" CssClass="bottom"/>
        </asp:Panel>
        <asp:Panel runat="server" CssClass="bottomRightCorner" >
            <asp:Button runat="server" Text="Wyloguj" OnClick="Logout_Click" />
        </asp:Panel>
    </div>
    </form>
</body>
</html>
