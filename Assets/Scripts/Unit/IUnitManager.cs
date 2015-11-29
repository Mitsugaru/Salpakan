using UnityEngine;
using System.Collections;

public interface IUnitManager
{

    UnitPiece GetUnitPieceForPosition(BoardPosition position);

    void AddPiece(BoardPosition position, UnitRank rank);

    void RemovePiece(BoardPosition position);

    int GetPlacementAmountForUnit(UnitRank rank);
}
