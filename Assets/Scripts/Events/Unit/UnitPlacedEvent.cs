using UnityEngine;
using System.Collections;

public class UnitPlacedEvent : UnitEvent
{

    private BoardPosition position;
    public BoardPosition Position
    {
        get
        {
            return position;
        }
    }

    public UnitPlacedEvent(BoardPosition position, UnitPiece unit) : base(unit)
    {
        this.position = position;
    }
}
