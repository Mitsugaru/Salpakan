using UnityEngine;
using System.Collections;
using strange.extensions.mediation.impl;
using System;

public class GameManager : View, IGameManager
{
    [Inject]
    public IEventManager EventManager { get; set; }

    private GameMode current = GameMode.PlayerOneSetup;
    public GameMode CurrentMode
    {
        get
        {
            return current;
        }
    }

    private PlayerInfo playerOne;
    public PlayerInfo PlayerOne
    {
        get
        {
            return playerOne;
        }
    }


    private PlayerInfo playerTwo;
    public PlayerInfo PlayerTwo
    {
        get
        {
            return playerTwo;
        }
    }

    private bool turnComplete = false;
    public bool TurnComplete
    {
        get
        {
            return turnComplete;
        }
    }

    public GameObject PlayerParent;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        playerOne = new PlayerInfo("Player One");
        playerTwo = new PlayerInfo("Player Two");

        GameObject holderOne = new GameObject();
        holderOne.name = playerOne.Name;
        holderOne.transform.SetParent(PlayerParent.transform);
        playerOne.Holder = holderOne;

        GameObject holderTwo = new GameObject();
        holderTwo.name = playerTwo.Name;
        holderTwo.transform.SetParent(PlayerParent.transform);
        playerTwo.Holder = holderTwo;

        EventManager.AddListener<BoardPositionSelectedEvent>(HandleBoardPositionSelection);
        EventManager.AddListener<UnitPlacedEvent>(HandleUnitPlaced);
        EventManager.AddListener<UnitMovedEvent>(HandleUnitMoved);
        EventManager.AddListener<UnitRemovedEvent>(HandleUnitRemoved);
        EventManager.AddListener<UnitSelectedEvent>(HandleUnitSelection);
    }

    public void ChangeMode(GameMode mode)
    {
        GameMode previous = current;
        current = mode;
        if(mode.Equals(GameMode.PlayerTransition))
        {
            turnComplete = false;
        }
        EventManager.Raise(new GameModeChangedEvent(previous, current));
    }

    private void HandleBoardPositionSelection(BoardPositionSelectedEvent e)
    {
        //Debug.Log("Selected: " + e.Position.ToString());
    }

    private void HandleUnitSelection(UnitSelectedEvent e)
    {
        //Debug.Log("Selected: " + e.Unit.Rank);
    }

    private void HandleUnitPlaced(UnitPlacedEvent e)
    {
        if (CurrentMode.Equals(GameMode.PlayerOne) || CurrentMode.Equals(GameMode.PlayerOneSetup))
        {
            PlayerOne.Pieces.Add(e.Unit);
        }
        else if (CurrentMode.Equals(GameMode.PlayerTwo) || CurrentMode.Equals(GameMode.PlayerTwoSetup))
        {
            PlayerTwo.Pieces.Add(e.Unit);
        }
    }

    private void HandleUnitMoved(UnitMovedEvent e)
    {
        turnComplete = true;
    }

    private void HandleUnitRemoved(UnitRemovedEvent e)
    {
        if (CurrentMode.Equals(GameMode.PlayerOne) || CurrentMode.Equals(GameMode.PlayerOneSetup))
        {
            PlayerOne.Pieces.Remove(e.Unit);
        }
        else if (CurrentMode.Equals(GameMode.PlayerTwo) || CurrentMode.Equals(GameMode.PlayerTwoSetup))
        {
            PlayerTwo.Pieces.Remove(e.Unit);
        }
    }

    public void HandleBattle(UnitPiece attacker, BoardPosition attackerPosition, UnitPiece defender, BoardPosition defenderPosition)
    {
        BattleResult result = UnitUtilities.ResolveBattle(attacker.Rank, defender.Rank);
        // Based on result, update players
        switch (result)
        {
            case BattleResult.Success:
                {
                    // Remove from defender owner
                    defender.Owner.Pieces.Remove(defender);
                    break;
                }
            case BattleResult.Fail:
                {
                    // Remove from attacker owner
                    attacker.Owner.Pieces.Remove(attacker);
                    break;
                }
            case BattleResult.Split:
                {
                    //Remove both
                    attacker.Owner.Pieces.Remove(attacker);
                    defender.Owner.Pieces.Remove(defender);
                    break;
                }
            default:
                {
                    break;
                }
        }
        EventManager.Raise(new UnitBattleResultEvent(attacker, defender, attackerPosition, defenderPosition, result));
        turnComplete = true;
    }
}
