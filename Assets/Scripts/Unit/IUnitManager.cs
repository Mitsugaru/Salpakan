using UnityEngine;
using System.Collections;

public interface IUnitManager
{

    UnitPiece GetUnitPieceForPosition(BoardPosition position);

    BoardPosition GetPositionForUnitPiece(UnitPiece piece);

    void AddPiece(BoardPosition position, UnitRank rank);

    void RemovePiece(BoardPosition position);

    int GetPlacementAmountForUnit(UnitRank rank);
}
