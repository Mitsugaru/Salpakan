using UnityEngine;
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

    private Dictionary<BoardPosition, GameObject> coverBoardPositions = new Dictionary<BoardPosition, GameObject>();

    // Use this for initialization
    protected override void Start()
    {
        //Initialize with max number of units for a given side
        for (int i = 0; i < 42; i++)
        {
            GameObject cover = Instantiate(CoverPanePrefab);
            cover.transform.SetParent(CoverParent);
            cover.SetActive(false);
            covers.Add(cover);
        }
        EventManager.AddListener<GameModeChangedEvent>(HandleGameModeChanged);
        EventManager.AddListener<UnitBattleResultEvent>(HandleUnitBattleResult);
    }

    public void ClearMask()
    {
        coverBoardPositions.Clear();
        foreach (GameObject cover in covers)
        {
            cover.SetActive(false);
        }
    }

    public void MaskPlayer(PlayerInfo player)
    {
        UnitPiece[] pieces = new UnitPiece[player.Pieces.Count];
        player.Pieces.CopyTo(pieces, 0);
        for (int i = 0; i < pieces.Length; i++)
        {
            BoardPosition boardPosition = UnitManager.GetPositionForUnitPiece(pieces[i]);
            Transform transform = BoardManager.GetTransformForPosition(boardPosition);
            Vector3 coverPosition = transform.position;
            coverPosition.z = GOLayer.COVER_LAYER;
            GameObject cover = covers[i];
            if (cover.activeInHierarchy)
            {
                for (int j = i + 1; j < covers.Count; j++)
                {
                    cover = covers[j];
                    if (!cover.activeInHierarchy)
                    {
                        break;
                    }
                }
            }
            cover.transform.position = coverPosition;
            cover.SetActive(true);
            coverBoardPositions.Add(boardPosition, cover);
        }
    }

    private void HandleGameModeChanged(GameModeChangedEvent e)
    {
        ClearMask();
        if (e.Current.Equals(GameMode.PlayerOneSetup))
        {
            GameManager.PlayerOne.Holder.SetActive(true);
            GameManager.PlayerTwo.Holder.SetActive(false);
        }
        else if (e.Current.Equals(GameMode.PlayerTwoSetup))
        {
            GameManager.PlayerOne.Holder.SetActive(false);
            GameManager.PlayerTwo.Holder.SetActive(true);
        }
        else if (e.Current.Equals(GameMode.PlayerOne))
        {
            MaskPlayer(GameManager.PlayerTwo);
        }
        else if (e.Current.Equals(GameMode.PlayerTwo))
        {
            MaskPlayer(GameManager.PlayerOne);
        }
        else if (e.Current.Equals(GameMode.PlayerTransition))
        {
            GameManager.PlayerOne.Holder.SetActive(true);
            GameManager.PlayerTwo.Holder.SetActive(true);
            ClearMask();
            MaskPlayer(GameManager.PlayerOne);
            MaskPlayer(GameManager.PlayerTwo);
        }
    }

    private void HandleUnitBattleResult(UnitBattleResultEvent e)
    {
        if (e.Result.Equals(BattleResult.Success) || e.Result.Equals(BattleResult.Split))
        {
            // get position of defender and disable cover
            GameObject cover;
            if (coverBoardPositions.TryGetValue(e.DefenderPosition, out cover))
            {
                cover.SetActive(false);
                coverBoardPositions.Remove(e.DefenderPosition);
            }
        }
    }
}
