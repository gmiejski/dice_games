﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonInterfacesModule
{
    public abstract class AbstractGameController : IGameController
    {

        protected GameState _gameState;
        protected List<IBot> _bots;
        protected String _gameName;
        protected GameType _gameType;
        protected String _ownerName;
        protected List<String> _playerNames;

        public event BroadcastGameStateHandler BroadcastGameState;
        public event DeleteGameControllerHandler DeleteGameController;

        protected virtual void OnBroadcastGameState(string gameName, GameState gameState)
        {
            if (BroadcastGameState != null)
            {
                BroadcastGameState(gameName, gameState);
            }
        }

        protected virtual void OnDelete(string gameName)
        {
            if (DeleteGameController != null)
            {
                DeleteGameController(gameName);
            }
        }

        public GameState GameState
        {
            get
            {
                return _gameState;
            }
            set
            {
                _gameState = value;
            }
        }

        public string GameName
        {
            get { return _gameName; }
        }

        protected AbstractGameController(String ownerName, String gameName, GameType gameType, List<String> players, List<IBot> bots)
        {
            if (ownerName == null || gameName == null)
            {
                throw new ArgumentNullException();
            }
            if (players.Count + bots.Count == 0)
            {
                throw new ArgumentException();
            }
            _ownerName = ownerName;
            _gameName = gameName;
            _gameType = gameType;
            _playerNames = players;
            _bots = bots;
            GameState = new GameState();
            GameState.PlayerStates = new Dictionary<string, PlayerState>();
            foreach (String player in _playerNames)
            {
                GameState.PlayerStates.Add(player, new PlayerState(new List<int>() { 0, 0, 0, 0, 0 }));
            }
            foreach (IBot bot in bots)
            {
                bot.BotMoved += new BotMovedHandler(BotMoved);
                GameState.PlayerStates.Add(bot.Name, new PlayerState(new List<int>() { 0, 0, 0, 0, 0 }));
            }
            GameState.IsOver = false;
            GameState.WinnerName = new List<string>() { players[0] };
            GameState.WhoseTurn = players[0];
        }


        public void BotMoved(string botName, Move move)
        {
            MakeMove(botName, move);
        }


        public abstract bool MakeMove(string playerName, Move move);
    }
}