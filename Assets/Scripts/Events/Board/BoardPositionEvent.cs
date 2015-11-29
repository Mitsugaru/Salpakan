using UnityEngine;
using System.Collections;

public class BoardPositionEvent : GameEvent {

    private BoardPosition position;
    public BoardPosition Position
    {
        get
        {
            return position;
        }
    }

    public BoardPositionEvent(BoardPosition position)
    {
        this.position = position;
    }
}
