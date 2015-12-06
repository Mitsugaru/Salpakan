using UnityEngine;
using System.Collections;

public interface IWinManager {
    bool WinOccurred { get; }

    PlayerInfo PlayerWon { get; }
}
