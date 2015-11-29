using UnityEngine;
using System.Collections;

public class BoardPositionCreatedEvent : BoardPositionEvent
{

    public GameObject tile;
    public GameObject Tile
    {
        get
        {
            return tile;
        }
    }

    public BoardPositionCreatedEvent(BoardPosition position, GameObject tile) : base(position)
    {
        this.tile = tile;
    }
}
