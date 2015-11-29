using UnityEngine;
using System.Collections;

public class GameModeChangedEvent : GameEvent
{

    private GameMode previous;
    public GameMode Previous
    {
        get
        {
            return previous;
        }
    }

    private GameMode current;
    public GameMode Current
    {
        get
        {
            return current;
        }
    }

    public GameModeChangedEvent(GameMode previous, GameMode current)
    {
        this.previous = previous;
        this.current = current;
    }
}
