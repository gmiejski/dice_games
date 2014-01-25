<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Game.aspx.cs" Inherits="GUIModule.Game" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../App_Themes/dice.css" rel="stylesheet" />
    <script src="../Scripts/jquery-2.0.3.min.js"></script>
    <script src="../Scripts/jquery.signalR-2.0.0.min.js"></script>
    <script src="/signalr/hubs"></script>
    <title>Gra</title>
</head>
<body id="gameBody">
    <form id="gameForm" runat="server">
        <asp:ScriptManager runat="server" ID="gameScriptManager">
        </asp:ScriptManager>
        <asp:UpdatePanel runat="server" ID="updatePanel" UpdateMode="Always">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="RefreshTable" EventName="click" />
            </Triggers>
            <ContentTemplate>
                <asp:Panel ID="GameStatsPanel" runat="server" CssClass="horCentered">
                    <asp:Label runat="server" CssClass="gameStat">Jesteś <%= PlayerName %></asp:Label><br />
                    <asp:Label runat="server" CssClass="gameStat">Nazwa gry: <%= GameName %></asp:Label><br />
                    <asp:Label runat="server" CssClass="gameStat">Stan: <asp:label runat="server" ID="GameState"><%= GameData.State %></asp:label></asp:Label><br />
                    <asp:Label runat="server" CssClass="gameStat" ID="WinnerLabel">Zwycięzca poprzedniej rundy: <%= String.Join(", ", GameData.Winner) %></asp:Label>
                </asp:Panel>
                <asp:GridView runat="server" ID="AwaitingPlayersList" CssClass="standardWindow horCentered" AutoGenerateColumns="false">
                    <Columns>
                        <asp:BoundField DataField="Key" HeaderText="Gracz" />
                        <asp:TemplateField HeaderText="Ogólny wynik">
                            <ItemTemplate>0</ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="#1">
                            <ItemTemplate>
                                <asp:Image runat="server" ImageUrl="~/Images/0.png" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="#2">
                            <ItemTemplate>
                                <asp:Image runat="server" ImageUrl="~/Images/0.png" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="#3">
                            <ItemTemplate>
                                <asp:Image runat="server" ImageUrl="~/Images/0.png" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="#4">
                            <ItemTemplate>
                                <asp:Image runat="server" ImageUrl="~/Images/0.png" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="#5">
                            <ItemTemplate>
                                <asp:Image runat="server" ImageUrl="~/Images/0.png" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:GridView runat="server" ID="PlayersList" cssClass="standardWindow horCentered"
                    AutoGenerateColumns="false" OnDataBound="PlayersDataBound" AllowSorting="true" OnSorting="PlayersSorting">
                    <Columns>
                        <asp:BoundField DataField="Key" HeaderText="Gracz" SortExpression="Key" />
                        <asp:BoundField DataField="Value.NumberOfWonRounds" HeaderText="Ogólny wynik" SortExpression="Value.NumberOfWonRounds" />
                        <asp:TemplateField HeaderText="#1"><ItemTemplate>
                                <asp:Image ID="dice1" runat="server" ImageUrl='<%# "~/Images/" + DataBinder.Eval(Container.DataItem, "Value.Dices[0]") + ".png" %>' />
                            </ItemTemplate></asp:TemplateField>
                        <asp:TemplateField HeaderText="#2"><ItemTemplate>
                                <asp:Image ID="dice2" runat="server" ImageUrl='<%# "~/Images/" + DataBinder.Eval(Container.DataItem, "Value.Dices[1]") + ".png" %>' />
                            </ItemTemplate></asp:TemplateField>
                        <asp:TemplateField HeaderText="#3"><ItemTemplate>
                                <asp:Image ID="dice3" runat="server" ImageUrl='<%# "~/Images/" + DataBinder.Eval(Container.DataItem, "Value.Dices[2]") + ".png" %>' />
                            </ItemTemplate></asp:TemplateField>
                        <asp:TemplateField HeaderText="#4"><ItemTemplate>
                                <asp:Image ID="dice4" runat="server" ImageUrl='<%# "~/Images/" + DataBinder.Eval(Container.DataItem, "Value.Dices[3]") + ".png" %>' />
                            </ItemTemplate></asp:TemplateField>
                        <asp:TemplateField HeaderText="#5"><ItemTemplate>
                                <asp:Image ID="dice5" runat="server" ImageUrl='<%# "~/Images/" + DataBinder.Eval(Container.DataItem, "Value.Dices[4]") + ".png" %>' />
                            </ItemTemplate></asp:TemplateField>
                        <asp:BoundField DataField="Value.CurrentResult" HeaderText="Kombinacja" SortExpression="Value.CurrentResultValue" />
                    </Columns>
                </asp:GridView>
                <asp:Panel runat="server" CssClass="horCentered">
                <asp:DataList runat="server" ID="userDice" RepeatDirection="Horizontal" CssClass="horCentered">
                    <ItemTemplate>
                        <asp:Panel runat="server" ID="toRoll" CssClass='<%# "userDiceSet dice_" + Container.DataItem %>' />
                    </ItemTemplate>
                </asp:DataList>
                <asp:Button runat="server" Text="Rzuć" ID="ThrowDice" />
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:Button runat="server" ID="RefreshTable"/>
        <div class="bottomRightCorner">
            <asp:Button runat="server" Text="Opuść grę" ID="LeaveGame" OnClick="LeaveGame_Click" />
        </div>
    </form>

    <script type="text/javascript">
        $(function () {
            var gameState = $.connection.gameHub;
            var playerName = "<%= EncodedPlayerName %>";
            var gameName = "<%= EncodedGameName %>";

            gameState.client.requestRefresh = function () {
                $("#RefreshTable").click();
            };

            gameState.client.endGame = function () {
                $.connection.hub.stop();
                $("#GameState").text("zakończona").css("color", "#f00");
                $("#ThrowDice").attr("disabled", "disabled");
            };
            
            //$.connection.hub.logging = true;
            $.connection.hub.start().done(function () {
                gameState.server.loginToGroup(playerName, gameName);
                $(document).on('click', '#ThrowDice', function (event) {
                    event.preventDefault();
                    var toSend = [];
                    for (var i = 0; i < 5; i++) {
                        if ($("#userDice_toRoll_" + i).hasClass("userDiceSetToRoll")) {
                            toSend.push(i);
                        }
                    }
                    gameState.server.throwDice(playerName, gameName, toSend);
                });
            });

        $(document).on('click', ".userDiceSet", function (event) {
            $(event.target).toggleClass("userDiceSetToRoll");
        });

        $("#LeaveGame").click(function () {
            if ($("#GameState").text() == "zakończona") {
                return true;
            }
            return confirm("Czy na pewno?");
        });
    });
    </script>
</body>
</html>
