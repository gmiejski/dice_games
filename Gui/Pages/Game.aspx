<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Game.aspx.cs" Inherits="Gui.Game" EnableEventValidation="false" %>

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
                <asp:AsyncPostBackTrigger ControlID="refreshTable" EventName="click" />
            </Triggers>
            <ContentTemplate>
                <asp:Panel ID="gameStatsPanel" runat="server" CssClass="horCentered">
                    <asp:Label runat="server" CssClass="gameStat">Jesteś <%= PlayerName %></asp:Label><br />
                    <asp:Label runat="server" CssClass="gameStat">Nazwa gry: <%= GameName %></asp:Label><br />
                    <asp:Label runat="server" CssClass="gameStat">Stan: <%= GameData.State %></asp:Label><br />
                    <asp:Label runat="server" CssClass="gameStat" ID="winnerLabel">Zwycięzca: <%= GameData.Winner %></asp:Label>
                </asp:Panel>
                <asp:GridView runat="server" ID="awaitingPlayersList" class="standardWindow horCentered" AutoGenerateColumns="false">
                    <Columns>
                        <asp:BoundField DataField="!" HeaderText="Gracz" />
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
                        <asp:TemplateField HeaderText="Kombinacja">
                            <ItemTemplate> --- </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:GridView runat="server" ID="playersList" class="standardWindow horCentered"
                    AutoGenerateColumns="false" OnDataBound="PlayersDataBound">
                    <Columns>
                        <asp:BoundField DataField="Key" HeaderText="Gracz" />
                        <asp:BoundField DataField="Value.NumberOfWonRounds" HeaderText="Ogólny wynik" />
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
                        <asp:BoundField DataField="Value.CurrentResult" HeaderText="Kombinacja" />
                    </Columns>
                </asp:GridView>
                <asp:Panel runat="server" CssClass="horCentered">
                <asp:DataList runat="server" ID="userDice" AutoGenerateColumns="true" RepeatDirection="Horizontal" CssClass="horCentered">
                    <ItemTemplate>
                        <asp:Panel runat="server" ID="toRoll" Text="a" CssClass='<%# "userDiceSet dice_" + Container.DataItem %>' />
                    </ItemTemplate>
                </asp:DataList>
                <asp:Button runat="server" Text="Rzuć" ID="throwDice" />
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:Button runat="server" ID="refreshTable"/>
        <div class="bottomRightCorner">
            <asp:Button runat="server" Text="Opuść grę" ID="leaveGame" OnClick="LeaveGame_Click" />
        </div>
    </form>

    <script type="text/javascript">
        $(function () {
            var gameState = $.connection.gameHub;
            var playerName = "<%= PlayerName %>";
            var gameName = "<%= GameName %>";

            gameState.client.requestRefresh = function () {
                $("#refreshTable").click();
            };

            $.connection.hub.start().done(function () {

                $(document).on('click', '#throwDice', function (event) {
                    event.preventDefault();
                    var toSend = [];
                    for (var i = 0; i < 5; i++) {
                        var isSelected = $("#userDice_toRoll_" + i).hasClass("userDiceSetToRoll") ? 1 : 0;
                        toSend.push(isSelected);
                    }
                    gameState.server.throwDice(playerName, gameName, toSend);
                });
            });

        $(document).on('click', ".userDiceSet", function (event) {
            $(event.target).toggleClass("userDiceSetToRoll");
        });

        $("#leaveGame").click(function () {
            return confirm("Czy na pewno?");
        });
    });
    </script>
</body>
</html>
