using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess {
    [System.Serializable]
    public enum PieceType {
        Pawn,
        Rook,
        Knight,
        Bishop,
        Queen,
        King
    }
    
    [System.Serializable]
    public enum PieceColor {
        White,
        Black
    }
    
    [System.Serializable]
    public class Piece
    {
        public PieceColor color;
        public PieceType type;
        
        public Hex position;

        public PieceColor OpponentColor => color == PieceColor.White ? PieceColor.Black : PieceColor.White;

        public Piece(PieceColor white, PieceType t, Hex pos)
        {
            color = white;
            type = t;
            position = pos;
        }

        internal Piece Clone()
        {
            return new Piece(color, type, position);
        }
    }
}
