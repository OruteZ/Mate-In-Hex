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
        
        public static readonly Move NONE = 
            new Move(PieceColor.White, PieceType.King, Hex.NONE, Hex.NONE, MoveFlag.None);

        private Move(PieceColor color, PieceType type, Hex from, Hex to, MoveFlag flag)
        {
            this.color = color;
            pieceType = type;
            this.from = from;
            this.to = to;
            flags = flag;
            capturedPieceType = PieceType.King; // default value, not used
        }
        
        public bool IsNoneValue()
        {
            return from.Equals(Hex.NONE) || to.Equals(Hex.NONE);
        }

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
        
        public override bool Equals(object obj)
        {
            if (obj is Move move)
            {
                return color == move.color &&
                       pieceType == move.pieceType &&
                       from.Equals(move.from) &&
                       to.Equals(move.to) &&
                       flags == move.flags;
            }
            
            return false;
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