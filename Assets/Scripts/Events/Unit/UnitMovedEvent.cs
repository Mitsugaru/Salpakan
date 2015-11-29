using UnityEngine;
using System.Collections;

public class UnitMovedEvent : UnitEvent
{

    private BoardPosition from;
    public BoardPosition From
    {
        get
        {
            return from;
        }
    }

    private BoardPosition to;
    public BoardPosition To
    {
        get
        {
            return to;
        }
    }

    public UnitMovedEvent(BoardPosition from, BoardPosition to, UnitPiece piece) : base(piece)
    {
        this.from = from;
        this.to = to;
    }
}
