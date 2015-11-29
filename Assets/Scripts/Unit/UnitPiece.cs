using UnityEngine;
using System.Collections;

public class UnitPiece
{
    public static readonly UnitPiece UNKNOWN = new UnitPiece(UnitRank.Unknown, null);

    private UnitRank rank;
    public UnitRank Rank
    {
        get
        {
            return rank;
        }
    }

    public PlayerInfo Owner
    {
        get;
        set;
    }

    private GameObject piece;
    public GameObject Piece
    {
        get
        {
            return piece;
        }
    }

    public UnitPiece(UnitRank rank, GameObject piece)
    {
        this.rank = rank;
        this.piece = piece;
    }

    public void Destroy()
    {
        piece = null;
    }
}
