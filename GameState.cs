﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ChessClient
{
    public class GameState
    {
        public int gameID;
        public string FEN;
        public string status;//play, completed
        public string lastMove;// Pe2e4
        public string lastMoveColor;// black, white
        public string result;// draw, resign, checkmate, stalemate
    }
}
