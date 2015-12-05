using UnityEngine;
using System.Collections;

public interface IBoardManager {

    Transform GetTransformForPosition(BoardPosition position);

    BoardPosition GetPositionForGO(GameObject go);

    bool PositionIsSelectable(BoardPosition position);

    bool PositionExists(BoardPosition position);
}
