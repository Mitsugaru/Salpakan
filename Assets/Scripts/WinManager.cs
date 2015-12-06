using UnityEngine;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using System;

public class WinManager : View, IWinManager
{

    [Inject]
    public IEventManager EventManager { get; set; }

    [Inject]
    public IGameManager GameManager { get; set; }

    [Inject]
    public IUnitManager UnitManager { get; set; }

    private bool win = false;
    public bool WinOccurred
    {
        get
        {
            return win;
        }
    }

    private PlayerInfo winner = PlayerInfo.UNKNOWN;
    public PlayerInfo PlayerWon
    {
        get
        {
            return winner;
        }
    }

    //TODO 5-move perpetual
    //TODO 16-move perpetual

    public bool NoChallengeRule = true;
    public int MoveWithoutChallengeLimit = 30;
    private int moveWithoutChallengeCount = -1;
    private int moveCount = -1;
    private Dictionary<PlayerInfo, int> flagAtEnd = new Dictionary<PlayerInfo, int>();

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        EventManager.AddListener<UnitBattleResultEvent>(HandleUnitBattleResult);
        EventManager.AddListener<GameModeChangedEvent>(HandleGameModeChange);
    }

    public void HandleUnitBattleResult(UnitBattleResultEvent e)
    {
        //Check if defender is a flag
        if (e.Defender.Rank.Equals(UnitRank.Flag))
        {
            //Attacker won
            win = true;
            winner = e.Attacker.Owner;
        }
        else if (e.Attacker.Rank.Equals(UnitRank.Flag) && e.Result.Equals(BattleResult.Fail))
        {
            //In the weird case that the attacker is a flag and it failed...
            win = true;
            winner = e.Defender.Owner;
        }
        moveWithoutChallengeCount = -1;
    }

    public void HandleGameModeChange(GameModeChangedEvent e)
    {
        if (!flagAtEnd.ContainsKey(GameManager.PlayerOne))
        {
            flagAtEnd.Add(GameManager.PlayerOne, 0);
        }
        if (!flagAtEnd.ContainsKey(GameManager.PlayerTwo))
        {
            flagAtEnd.Add(GameManager.PlayerTwo, 0);
        }

        if (e.Current.Equals(GameMode.PlayerTransition))
        {
            moveWithoutChallengeCount++;
            moveCount++;
        }
        else if (e.Current.Equals(GameMode.PlayerOne) || e.Current.Equals(GameMode.PlayerTwo))
        {
            bool draw = false;
            //Check no challenge rule
            if (NoChallengeRule && moveWithoutChallengeCount >= MoveWithoutChallengeLimit)
            {
                draw = true;
                win = true;
            }
            if (!draw)
            {
                //Check for flag
                for (int i = 0; i < 9; i++)
                {
                    UnitPiece piece = UnitManager.GetUnitPieceForPosition(new BoardPosition(0, i));
                    if (piece.Rank.Equals(UnitRank.Flag) && piece.Owner.Equals(GameManager.PlayerTwo))
                    {
                        flagAtEnd[piece.Owner]++;
                    }

                    piece = UnitManager.GetUnitPieceForPosition(new BoardPosition(7, i));
                    if (piece.Rank.Equals(UnitRank.Flag) && piece.Owner.Equals(GameManager.PlayerOne))
                    {
                        flagAtEnd[piece.Owner]++;
                    }
                }
                foreach (KeyValuePair<PlayerInfo, int> pair in flagAtEnd)
                {
                    if (pair.Value >= 1)
                    {
                        //Meets win condition
                        win = true;
                        winner = pair.Key;
                    }
                }
            }
        }
    }

}
