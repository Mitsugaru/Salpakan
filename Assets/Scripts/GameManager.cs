﻿using UnityEngine;
using System.Collections;
using strange.extensions.mediation.impl;

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

    public GameObject PlayerParent;

	// Use this for initialization
	protected override void Start () {
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
        EventManager.AddListener<UnitSelectedEvent>(HandleUnitSelection);
	}

    public void ChangeMode(GameMode mode)
    {
        GameMode previous = current;
        current = mode;
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
}