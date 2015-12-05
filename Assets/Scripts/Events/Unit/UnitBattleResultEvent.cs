using UnityEngine;
using System.Collections;

public class UnitBattleResultEvent : UnitEvent
{

    public UnitPiece Attacker
    {
        get
        {
            return Unit;
        }
    }

    private UnitPiece defender;
    public UnitPiece Defender
    {
        get
        {
            return defender;
        }
    }

    private BoardPosition attackerPosition;
    public BoardPosition AttackerPosition
    {
        get
        {
            return attackerPosition;
        }
    }

    private BoardPosition defenderPosition;
    public BoardPosition DefenderPosition
    {
        get
        {
            return defenderPosition;
        }
    }

    private BattleResult result;
    public BattleResult Result
    {
        get
        {
            return result;
        }
    }

    public UnitBattleResultEvent(UnitPiece attacker, UnitPiece defender, BoardPosition attackerPosition, BoardPosition defenderPosition, BattleResult result) : base(attacker)
    {
        this.defender = defender;
        this.result = result;
        this.attackerPosition = attackerPosition;
        this.defenderPosition = defenderPosition;
    }
}
