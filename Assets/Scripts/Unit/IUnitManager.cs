using UnityEngine;
using System.Collections;

public interface IUnitManager
{

    UnitPiece GetUnitPieceForPosition(BoardPosition position);

    BoardPosition GetPositionForUnitPiece(UnitPiece piece);

    void AddPiece(BoardPosition position, UnitRank rank);

    /// <summary>
    /// Will attempt to move a piece to another position.
    /// If that position is already taken, the move operation will not occur.
    /// </summary>
    /// <param name="current"></param>
    /// <param name="future"></param>
    /// <param name="piece"></param>
    /// <returns></returns>
    bool MovePiece(BoardPosition current, BoardPosition future);

    void RemovePiece(BoardPosition position);

    int GetPlacementAmountForUnit(UnitRank rank);
}
