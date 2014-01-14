﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonInterfacesModule
{
    abstract class AbstractGameController : IGameController
    {

        private GameState _gameState;
        private List<IBot> _bots;
        private String _gameName;
        private GameType _gameType;
        private String _ownerName;
        private List<String> _playerNames;

        event BroadcastGameStateHandler IGameController.BroadcastGameState
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }//rozeslac GameState eventem

        event DeleteGameControllerHandler IGameController.DeleteGameController
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }//usunac gre
        //czekac na event od bota
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

        protected AbstractGameController(String ownerName, String gameName, GameType gameType, List<String> players, List<IBot> bots)
        {
            _ownerName = ownerName;
            _gameName = gameName;
            _gameType = gameType;
            _playerNames = players;
            _bots = bots;
        }

        public abstract bool MakeMove(string playerName, Move move);
    }
}