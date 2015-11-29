using UnityEngine;
using System.Collections;

public class UnitPlacementSelectedEvent : GameEvent {

    private UnitRank rank;
    public UnitRank Rank
    {
        get
        {
            return rank;
        }
    }

    public UnitPlacementSelectedEvent(UnitRank selected)
    {
        this.rank = selected;
    }
}
