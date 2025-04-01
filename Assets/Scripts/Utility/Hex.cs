using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Hex
{
    public bool Equals(Hex other)
    {
        return q == other.q && r == other.r && s == other.s;
    }

    public override bool Equals(object obj)
    {
        return obj is Hex other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(q, r, s);
    }

    [SerializeField] private int q;
    [SerializeField] private int r;
    [SerializeField] private int s;
    
    public int Q => q;
    public int R => r;
    public int S => s;
    
    public Hex(int q, int r, int s)
    {
        this.q = q;
        this.r = r;
        this.s = s;
        if (q + r + s != 0) throw new ArgumentException("q + r + s must be 0");
    }
    
    public Hex(int q, int r) : this(q, r, -q - r) { }
    
    public Hex Add(Hex b)
    {
        return new Hex(q + b.q, r + b.r, s + b.s);
    }


    public Hex Subtract(Hex b)
    {
        return new Hex(q - b.q, r - b.r, s - b.s);
    }


    public Hex Scale(int k)
    {
        return new Hex(q * k, r * k, s * k);
    }
        
    public Hex DiagonalNeighbor(HexDiagonalDirection direction)
    {
        return Add(Hex.diagonals[(int)direction]);
    }


    public int Length()
    {
        return (int)((Math.Abs(q) + Math.Abs(r) + Math.Abs(s)) / 2);
    }


    public int Distance(Hex b)
    {
        return Subtract(b).Length();
    }
    
    public Vector2 ToPixel()
    {
        //function flat_hex_to_pixel(hex):
        // var x = size * (     3./2 * hex.q                    )
        // var y = size * (sqrt(3)/2 * hex.q  +  sqrt(3) * hex.r)
        // return Point(x, y)

        float x = RADIUS * (1.5f * q);
        float y = RADIUS * (SQRT3 / 2.0f * q + SQRT3 * r);
        return new Vector2(x, y);
    }

    public int GetTileKind()
    {
        int v = q + (r * 2);
        
        // v가 음수일경우 양수일때까지 3 더하기
        while (v < 0)
        {
            v += 3;
        }

        return v % 3;
    }
    
    // overide != and ==
    public static bool operator ==(Hex a, Hex b)
    {
        return a.q == b.q && a.r == b.r && a.s == b.s;
    }
    
    public static bool operator !=(Hex a, Hex b)
    {
        return !(a == b);
    }

    public override string ToString()
    {
        return $"({q}, {r}, {s})";
    }
    
    
    #region Static
    private const float SQRT3 = 1.7320508075688772935274463415059f;
    private const float RADIUS = 1f;
    
    public static Hex[] directions = {
        new Hex(0, 1, -1), // North
        new Hex(1, 0, -1), // North-East
        new Hex(1, -1, 0), // South-East
        new Hex(0, -1, 1), // South
        new Hex(-1, 0, 1), // South-West
        new Hex(-1, 1, 0) // North-West
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
        new Hex(2, -1, -1), new Hex(1, -2, 1), new Hex(-1, -1, 2),
        new Hex(-2, 1, 1), new Hex(-1, 2, -1), new Hex(1, 1, -2)
    };

    public static List<Hex> GetHexMap(int radius)
    {
        List<Hex> hexMap = new List<Hex>();
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

