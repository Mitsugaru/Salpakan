using UnityEngine;
using System.Collections;

public interface IGameManager {
    GameMode CurrentMode { get; }

    PlayerInfo PlayerOne { get; }

    PlayerInfo PlayerTwo { get; }

    bool TurnComplete { get; }

    void ChangeMode(GameMode mode);

    void HandleBattle(UnitPiece attacker, BoardPosition attackerPosition, UnitPiece defender, BoardPosition defenderPosition);
}
