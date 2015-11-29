using UnityEngine;
using System.Collections;

public class UnitEvent : GameEvent
{

    private UnitPiece unit;
    public UnitPiece Unit
    {
        get
        {
            return unit;
        }
    }

    public UnitEvent(UnitPiece unit)
    {
        this.unit = unit;
    }
}
