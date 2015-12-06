using UnityEngine;
using System.Collections;

public interface ICoverManager {

    void ClearMask();

    void MaskPlayer(PlayerInfo player);
}
