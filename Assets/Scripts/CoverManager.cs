﻿using UnityEngine;
using System.Collections.Generic;
using strange.extensions.mediation.impl;

public class CoverManager : View, ICoverManager
{
    [Inject]
    public IEventManager EventManager { get; set; }

    [Inject]
    public IGameManager GameManager { get; set; }

    [Inject]
    public IBoardManager BoardManager { get; set; }

    [Inject]
    public IUnitManager UnitManager { get; set; }

    public GameObject CoverPanePrefab;

    public Transform CoverParent;

    private List<GameObject> covers = new List<GameObject>();

    // Use this for initialization
    protected override void Start()
    {
        //Initialize with max number of units for a given side
        for(int i = 0; i < 21; i++)
        {
            GameObject cover = Instantiate(CoverPanePrefab);
            cover.transform.SetParent(CoverParent);
            cover.SetActive(false);
            covers.Add(cover);
        }
        EventManager.AddListener<GameModeChangedEvent>(HandleGameModeChanged);
    }

    private void HandleGameModeChanged(GameModeChangedEvent e)
    {
        ClearMask();
        if (e.Current.Equals(GameMode.PlayerOne) || e.Current.Equals(GameMode.PlayerOneSetup))
        {
            MaskPlayer(GameManager.PlayerTwo);
        }
        else if (e.Current.Equals(GameMode.PlayerTwo) || e.Current.Equals(GameMode.PlayerTwoSetup))
        {
            MaskPlayer(GameManager.PlayerOne);
        }
    }

    private void ClearMask()
    {
        foreach(GameObject cover in covers)
        {
            cover.SetActive(false);
        }
    }

    private void MaskPlayer(PlayerInfo player)
    {
        UnitPiece[] pieces = new UnitPiece[player.Pieces.Count];
        player.Pieces.CopyTo(pieces, 0);
        for(int i = 0; i < pieces.Length; i++)
        {
            BoardPosition boardPosition = UnitManager.GetPositionForUnitPiece(pieces[i]);
            Transform transform = BoardManager.GetTransformForPosition(boardPosition);
            Vector3 coverPosition = transform.position;
            coverPosition.z = GOLayer.COVER_LAYER;
            GameObject cover = covers[i];
            cover.transform.position = coverPosition;
            cover.SetActive(true);
        }
    }
}