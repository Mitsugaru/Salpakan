using UnityEngine;
using System.Collections;

public interface IGameManager {
    GameMode CurrentMode { get; }

    PlayerInfo PlayerOne { get; }

    PlayerInfo PlayerTwo { get; }

    void ChangeMode(GameMode mode);
}
