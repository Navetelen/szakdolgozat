using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridDirection
{
    public readonly Vector2Int Vector;

    private GridDirection(int x, int y)
    {
        Vector = new Vector2Int(x, y);
    }

    //conversion operator between Vector2Int and GridDirection
    //GridDirectionből Vector2Int-et csináló operátor 
    //A Vector2Int egy kétdimenziós vektor kizárólag int adatokkal
    public static implicit operator Vector2Int(GridDirection direction)
    {
        return direction.Vector;
    }

    //ez a fordítottja, Vector2Int-ből csinál GridDirectiont
    public static GridDirection GetDirectionFromV2I(Vector2Int vector)
    {
        return CardInterCardDir.DefaultIfEmpty(None).FirstOrDefault(direction => direction == vector);
    }

    //irányok megadása
    public static readonly GridDirection None = new GridDirection(0, 0);
    public static readonly GridDirection N = new GridDirection(0, 1);
    public static readonly GridDirection S = new GridDirection(0, -1);
    public static readonly GridDirection E = new GridDirection(1, 0);
    public static readonly GridDirection W = new GridDirection(-1, 0);
    public static readonly GridDirection NE = new GridDirection(1, 1);
    public static readonly GridDirection NW = new GridDirection(-1, 1);
    public static readonly GridDirection SE = new GridDirection(1, -1);
    public static readonly GridDirection SW = new GridDirection(-1, -1);

    public static readonly List<GridDirection> CardinalDir = new List<GridDirection>
    {
        N,
        E,
        S,
        W
    };
    
    public static readonly List<GridDirection> CardInterCardDir = new List<GridDirection>
    {
        N,
        NE,
        E,
        SE,
        S,
        SW,
        W,
        NW
    };
    

    public static readonly List<GridDirection> AllDir = new List<GridDirection>
    {
        None,
        N,
        NE,
        E,
        SE,
        S,
        SW,
        W,
        NW
    };
}
