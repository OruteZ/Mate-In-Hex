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

        public MoveFlag flags;
        public PieceType capturedPieceType;

        public void SetFlag(MoveFlag flag)
        {
            flags |= flag;
        }

        public void ClearFlag(MoveFlag flag)
        {
            flags &= ~flag;
        }

        public bool HasFlag(MoveFlag flag)
        {
            return (flags & flag) == flag;
        }

        public override string ToString()
        {
            return $"{color} {pieceType} {from} -> {to} {flags}";
        }
    }

    [System.Flags, System.Serializable]
    public enum MoveFlag 
    {
        None = 0,
        Capture = 1 << 0,
        Promotion = 1 << 1,
        Check = 1 << 2,
        Checkmate = 1 << 3,
        Stalemate = 1 << 4,
    }
}