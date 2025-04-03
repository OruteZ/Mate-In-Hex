using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] 
public struct Hex
{
    public readonly bool Equals(Hex other)
    {
        return q == other.q && r == other.r && s == other.s;
    }

    public override bool Equals(object obj)
    {
        return obj is Hex other && Equals(other);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(q, r, s);
    }

    [SerializeField] private int q;
    [SerializeField] private int r;
    [SerializeField] private int s;
    
    public readonly int Q => q;
    public readonly int R => r;
    public readonly int S => s;

    public static readonly Hex None = new (1000, 1000);
    
    public Hex(int q, int r, int s)
    {
        this.q = q;
        this.r = r;
        this.s = s;
        if (q + r + s != 0) throw new ArgumentException("q + r + s must be 0");
    }
    
    public Hex(int q, int r) : this(q, r, -q - r) { }
    
    public readonly Hex Add(Hex b)
    {
        return new Hex(q + b.q, r + b.r, s + b.s);
    }


    public readonly Hex Subtract(Hex b)
    {
        return new Hex(q - b.q, r - b.r, s - b.s);
    }


    public readonly Hex Scale(int k)
    {
        return new Hex(q * k, r * k, s * k);
    }
        
    public Hex DiagonalNeighbor(HexDiagonalDirection direction)
    {
        return Add(Hex.diagonals[(int)direction]);
    }


    public readonly int Length()
    {
        return (int)((Math.Abs(q) + Math.Abs(r) + Math.Abs(s)) / 2);
    }


    public int Distance(Hex b)
    {
        return Subtract(b).Length();
    }

    public readonly bool IsDiagonalVector()
    {
        int a = Math.Abs(q);
        int b = Math.Abs(r);
        int c = Math.Abs(s);
        
        // 0이 포함되면 대각선 벡터가 아님
        if(a == 0 || b == 0 || c == 0)
            return false;
        
        int[] arr = new int[] { a, b, c };
        Array.Sort(arr); // 오름차순 정렬: [m, m, M]가 되어야 함
        
        // m과 M가 k, k, 2k 형태여야 대각선 벡터임.
        return arr[0] == arr[1] && arr[2] == 2 * arr[0];
    }


    public readonly bool IsStraightVector()
    {
        return q == 0 || r == 0 || s == 0;
    }
    
    public readonly Vector2 ToPixel()
    {
        float x = RADIUS * (1.5f * q);
        float y = RADIUS * (SQRT3 / 2.0f * q + SQRT3 * r);
        return new Vector2(x, y);
    }

    public readonly int GetTileKind()
    {
        int v = q + (r * 2);
        
        // v가 음수일경우 양수일때까지 3 더하기
        while (v < 0)
        {
            v += 3;
        }

        return v % 3;
    }
    #region Operator
    
    // overide != and ==
    public static bool operator ==(Hex a, Hex b)
    {
        return a.q == b.q && a.r == b.r && a.s == b.s;
    }
    
    public static bool operator !=(Hex a, Hex b)
    {
        return !(a == b);
    }

    public static Hex operator +(Hex a, Hex b)
    {
        return a.Add(b);
    }

    public static Hex operator -(Hex a, Hex b)
    {
        return a.Subtract(b);
    }

    public static Hex operator *(Hex a, int k)
    {
        return a.Scale(k);
    }
    
    public static Hex operator *(int k, Hex a)
    {
        return a.Scale(k);
    }

    public static Hex operator -(Hex a)
    {
        return new Hex(-a.q, -a.r, -a.s);
    }

    #endregion

    public override readonly string ToString()
    {
        return $"({q}, {r}, {s})";
    }
    
    
    #region Static
    private const float SQRT3 = 1.7320508075688772935274463415059f;
    private const float RADIUS = 1f;
    
    public static Hex[] directions = {
        new(0, 1, -1), // North
        new(1, 0, -1), // North-East
        new(1, -1, 0), // South-East
        new(0, -1, 1), // South
        new(-1, 0, 1), // South-West
        new(-1, 1, 0) // North-West
    };
    
    public static Hex Direction(HexDirection direction)
    {
        return Hex.directions[(int)direction];
    }

    public Hex Neighbor(HexDirection direction)
    {
        return Add(Direction(direction));
    }

    public static Hex[] diagonals = 
    {
        new(1, 1, -2), // North-East
        new(2, -1, -1), // East
        new(1, -2, 1), // South-East
        new(-1, -1, 2), // South-West
        new(-2, 1, 1), // West
        new(-1, 2, -1) // North-West
    };

    public static Hex Diagonal(HexDiagonalDirection direction)
    {
        return Hex.diagonals[(int)direction];
    }

    public static List<Hex> GetHexMap(int radius)
    {
        List<Hex> hexMap = new();
        for (int q = -radius; q <= radius; q++)
        {
            int r1 = Math.Max(-radius, -q - radius);
            int r2 = Math.Min(radius, -q + radius);
            for (int r = r1; r <= r2; r++)
            {
                hexMap.Add(new Hex(q, r));
            }
        }

        return hexMap;
    }

    public static Hex GetHexFromPixel(Vector2 pixel)
    {
        float q = pixel.x / (RADIUS * 1.5f);
        float r = pixel.y / (RADIUS * SQRT3) - (pixel.x / (RADIUS * 3f));

        return new Hex(Mathf.RoundToInt(q), Mathf.RoundToInt(r));
    }

    #endregion
}

public enum HexDirection
{
    N, NE, SE, S, SW, NW
}


public enum HexDiagonalDirection
{
    NE, E, SE, SW, W, NW
}

