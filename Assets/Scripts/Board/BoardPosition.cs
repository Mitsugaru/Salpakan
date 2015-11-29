using UnityEngine;
using System.Collections;

public class BoardPosition {

    public static readonly BoardPosition OFF_BOARD = new BoardPosition(-1, -1);

    private int x = 0;
    public int X
    {
        get
        {
            return x;
        }
    }
    private int y = 0;
    public int Y
    {
        get
        {
            return y;
        }
    }

    public BoardPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(object obj)
    {
        bool value = false;

        BoardPosition other = obj as BoardPosition;
        if(other != null)
        {
            value = X.Equals(other.X) && Y.Equals(other.Y);
        }

        return value;
    }

    public override int GetHashCode()
    {
        int hash = 23;
        hash = hash * 31 + x;
        hash = hash * 31 + y;
        return hash;
    }

    public override string ToString()
    {
        return "Position: " + x + "," + y;
    }
}
