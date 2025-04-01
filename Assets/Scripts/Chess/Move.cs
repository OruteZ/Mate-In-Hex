using UnityEngine;

namespace Chess
{
    [System.Serializable]
    public struct Move
    {
        public PieceColor color;
        public PieceType pieceType;
        
        public Hex from;
        public Hex to;
    }
}