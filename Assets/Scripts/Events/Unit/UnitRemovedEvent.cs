using UnityEngine;
using System.Collections;

public class UnitRemovedEvent : UnitEvent {

    private BoardPosition position;
    public BoardPosition Position
    {
        get
        {
            return position;
        }
    }

    public UnitRemovedEvent(BoardPosition position, UnitPiece unit) : base(unit)
    {
        this.position = position;
    }
}
