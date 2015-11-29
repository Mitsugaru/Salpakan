using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using System;

public class UnitManager : View, IUnitManager
{

    [Inject]
    public IEventManager EventManager { get; set; }

    [Inject]
    public IBoardManager BoardManager { get; set; }

    [Inject]
    public IUnitMaterialManager MaterialManager { get; set; }

    [Inject]
    public IGameManager GameManager { get; set; }

    public GameObject UnitPlane;

    private Dictionary<BoardPosition, UnitPiece> pieces = new Dictionary<BoardPosition, UnitPiece>();

    private Dictionary<UnitRank, int> playerOnePlacementUnits = new Dictionary<UnitRank, int>();

    private Dictionary<UnitRank, int> playerTwoPlacementUnits = new Dictionary<UnitRank, int>();

    protected override void Start()
    {
        base.Start();

        SetupPlacementValues();
    }

    public void AddPiece(BoardPosition position, UnitRank rank)
    {
        //Check for existing piece, remove it if needed.
        //Debug.Log("atteming to add " + UnitUtilities.ReadableRank(rank) + " at " + position.ToString());
        bool equal = false;
        if (pieces.ContainsKey(position))
        {
            UnitPiece existing;
            if (pieces.TryGetValue(position, out existing))
            {
                if (existing.Rank.Equals(rank))
                {
                    //We're not going to remove then add the same exact piece to the same position
                    equal = true;
                }
                else
                {
                    RemovePiece(position);
                }
            }

        }
        if (!equal)
        {
            Transform target = BoardManager.GetTransformForPosition(position);
            if (!EqualityComparer<Transform>.Default.Equals(target, default(Transform)))
            {
                GameObject plane = Instantiate(UnitPlane);
                UnitPiece piece = GeneratePiece(rank, plane);
                if (piece.Owner != null)
                {
                    piece.Piece.transform.SetParent(piece.Owner.Holder.transform);
                    plane.name = UnitUtilities.ReadableRank(rank);
                    plane.layer = LayerMask.NameToLayer("Pieces");
                    Vector3 planePos = BoardManager.GetTransformForPosition(position).position;
                    planePos.z = -2;
                    plane.transform.position = planePos;
                    Renderer r = plane.GetComponent<Renderer>();
                    r.material = MaterialManager.GetRankMaterial(rank);
                    pieces.Add(position, piece);
                    EventManager.Raise(new UnitPlacedEvent(position, piece));
                }
                else
                {
                    piece.Destroy();
                    Destroy(plane);
                }
            }
        }
    }

    private UnitPiece GeneratePiece(UnitRank rank, GameObject plane)
    {
        UnitPiece piece = new UnitPiece(rank, plane);
        if (GameManager.CurrentMode == GameMode.PlayerOneSetup)
        {
            piece.Owner = GameManager.PlayerOne;
            playerOnePlacementUnits[rank] -= 1;
        }
        else if (GameManager.CurrentMode == GameMode.PlayerTwoSetup)
        {
            piece.Owner = GameManager.PlayerTwo;
            playerTwoPlacementUnits[rank] -= 1;
        }
        return piece;
    }

    public void RemovePiece(BoardPosition position)
    {
        UnitPiece piece;
        Debug.Log("attempting to get unit already at position " + position.ToString());
        if (pieces.TryGetValue(position, out piece))
        {
            bool result = pieces.Remove(position);
            Debug.Log("removed " + UnitUtilities.ReadableRank(piece.Rank) + ": " + result);
            if (GameManager.CurrentMode == GameMode.PlayerOneSetup)
            {
                playerOnePlacementUnits[piece.Rank] += 1;
            }
            else if (GameManager.CurrentMode == GameMode.PlayerTwoSetup)
            {
                playerTwoPlacementUnits[piece.Rank] += 1;
            }
            GameObject temp = piece.Piece;
            piece.Destroy();
            Destroy(temp);
            EventManager.Raise(new UnitRemovedEvent(position, piece));
        }
    }

    public UnitPiece GetUnitPieceForPosition(BoardPosition position)
    {
        UnitPiece piece = default(UnitPiece);
        if (!pieces.TryGetValue(position, out piece))
        {
            piece = UnitPiece.UNKNOWN;
        }
        return piece;
    }

    public int GetPlacementAmountForUnit(UnitRank rank)
    {
        int amount = 0;
        if (GameManager.CurrentMode == GameMode.PlayerOneSetup)
        {
            playerOnePlacementUnits.TryGetValue(rank, out amount);
        }
        else if (GameManager.CurrentMode == GameMode.PlayerTwoSetup)
        {
            playerTwoPlacementUnits.TryGetValue(rank, out amount);
        }
        return amount;
    }

    private void SetupPlacementValues()
    {
        foreach (UnitRank rank in Enum.GetValues(typeof(UnitRank)))
        {
            int amount = 0;
            switch (rank)
            {
                case UnitRank.Unknown:
                    {
                        break;
                    }
                case UnitRank.Spy:
                    {
                        amount = 2;
                        break;
                    }
                case UnitRank.Private:
                    {
                        amount = 6;
                        break;
                    }
                default:
                    {
                        amount = 1;
                        break;
                    }
            }
            if (amount > 0)
            {
                playerOnePlacementUnits.Add(rank, amount);
                playerTwoPlacementUnits.Add(rank, amount);
            }
        }
    }
}
