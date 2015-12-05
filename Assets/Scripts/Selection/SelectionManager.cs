using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;

public class SelectionManager : View, ISelectionManager
{
    [Inject]
    public IEventManager EventManager { get; set; }

    [Inject]
    public IGameManager GameManager { get; set; }

    [Inject]
    public IBoardManager BoardManager { get; set; }

    [Inject]
    public IUnitManager UnitManager { get; set; }

    public Camera mainCamera;

    public GameObject HighlightCubePrefab;

    public GameObject HoverCubePrefab;

    public GameObject ValidCubePrefab;

    public Transform CubeParent;

    public LayerMask mask;

    private UnitRank placementRank = UnitRank.Unknown;

    private BoardPosition previous = BoardPosition.OFF_BOARD;

    private GameObject highlightCube;

    private GameObject hoverCube;

    private GameObject[] validCubes = new GameObject[4];

    private List<BoardPosition> validPositions = new List<BoardPosition>();

    private bool action = false;

    protected override void Start()
    {
        base.Start();

        highlightCube = Instantiate(HighlightCubePrefab);
        highlightCube.transform.SetParent(CubeParent);
        hoverCube = Instantiate(HoverCubePrefab);
        hoverCube.transform.SetParent(CubeParent);
        for (int i = 0; i < validCubes.Length; i++)
        {
            validCubes[i] = Instantiate(ValidCubePrefab);
            validCubes[i].transform.SetParent(CubeParent);
        }

        EventManager.AddListener<UnitPlacementSelectedEvent>(HandleUnitPlacementSelected);
        EventManager.AddListener<UnitPlacedEvent>(HandleUnitPlaced);
        EventManager.AddListener<UnitBattleResultEvent>(HandleUnitBattleResult);
        EventManager.AddListener<GameModeChangedEvent>(HandleGameModeChanged);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit[] hits = Physics.RaycastAll(mainCamera.ScreenPointToRay(Input.mousePosition), 50, mask);

        bool selectable = false;
        BoardPosition position = BoardPosition.OFF_BOARD;
        UnitPiece unit = UnitPiece.UNKNOWN;
        Vector3 tileVector = Vector3.zero;

        if (hits.Length == 0)
        {
            hoverCube.SetActive(false);
        }
        else
        {
            // Find the board first
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Board"))
                {
                    position = BoardManager.GetPositionForGO(hit.transform.gameObject);
                    selectable = BoardManager.PositionIsSelectable(position);
                    tileVector = hit.transform.position;
                    if (selectable && !action)
                    {
                        MoveHover(hit.transform.position);
                    }
                    else
                    {
                        hoverCube.SetActive(false);
                    }
                    break;
                }
            }
            // Because there's no order to raycast hit result, check if we hit a piece
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Pieces"))
                {
                    unit = UnitManager.GetUnitPieceForPosition(position);
                    break;
                }
            }
        }

        //Handle input based on game mode
        if (GameManager.CurrentMode.Equals(GameMode.PlayerOneSetup) || GameManager.CurrentMode.Equals(GameMode.PlayerTwoSetup))
        {
            HandleSetup(hits, selectable, position, unit, tileVector);
        }
        if (!action && (GameManager.CurrentMode.Equals(GameMode.PlayerOne) || GameManager.CurrentMode.Equals(GameMode.PlayerTwo)))
        {
            HandleGame(hits, selectable, position, unit, tileVector);
        }
    }

    private void HandleSetup(RaycastHit[] hits, bool selectable, BoardPosition position, UnitPiece unit, Vector3 tileVector)
    {
        //Handle mouse click
        if (Input.GetMouseButtonDown(0))
        {
            if (hits.Length == 0)
            {
                highlightCube.SetActive(false);
            }
            else if (selectable && !placementRank.Equals(UnitRank.Unknown))
            {
                UnitManager.AddPiece(position, placementRank);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            //Remove existing piece
            if (!unit.Rank.Equals(UnitRank.Unknown))
            {
                UnitManager.RemovePiece(position);
            }
        }
    }

    private void HandleGame(RaycastHit[] hits, bool selectable, BoardPosition position, UnitPiece unit, Vector3 tileVector)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (hits.Length == 0)
            {
                highlightCube.SetActive(false);
                previous = position;
            }
            else if (selectable && !unit.Equals(UnitPiece.UNKNOWN))
            {
                //We've selected a unit
                //Check if we're the owner
                bool owner = false;
                if (GameManager.CurrentMode.Equals(GameMode.PlayerOne))
                {
                    owner = unit.Owner.Equals(GameManager.PlayerOne);
                }
                else if (GameManager.CurrentMode.Equals(GameMode.PlayerTwo))
                {
                    owner = unit.Owner.Equals(GameManager.PlayerTwo);
                }

                if (owner)
                {
                    //If the previous position was off the board, this is our first selection
                    if (previous.Equals(BoardPosition.OFF_BOARD))
                    {
                        previous = position;
                        MoveHighlight(tileVector);

                        //Determine what the valid next positions are
                        ClearValid();
                        //Add current position, for deselection
                        validPositions.Add(position);
                        ValidateNextPosition(new BoardPosition(position.X - 1, position.Y));
                        ValidateNextPosition(new BoardPosition(position.X, position.Y - 1));
                        ValidateNextPosition(new BoardPosition(position.X + 1, position.Y));
                        ValidateNextPosition(new BoardPosition(position.X, position.Y + 1));
                        MoveValid();
                    }
                    else if (previous.Equals(position))
                    {
                        //We selected the same space, deselect
                        previous = BoardPosition.OFF_BOARD;
                        highlightCube.SetActive(false);
                        ClearValid();
                    }
                }
                else if (validPositions.Contains(position))
                {
                    //If our previous position contains a piece that we own, then we should do battle
                    UnitPiece previousPiece = UnitManager.GetUnitPieceForPosition(previous);
                    if (!previousPiece.Equals(UnitPiece.UNKNOWN))
                    {
                        bool previousOwner = false;
                        if (GameManager.CurrentMode.Equals(GameMode.PlayerOne))
                        {
                            previousOwner = previousPiece.Owner.Equals(GameManager.PlayerOne);
                        }
                        else if (GameManager.CurrentMode.Equals(GameMode.PlayerTwo))
                        {
                            previousOwner = previousPiece.Owner.Equals(GameManager.PlayerTwo);
                        }

                        if (previousOwner)
                        {
                            // instigate attack, with previous piece as attacker and current piece as defender
                            GameManager.HandleBattle(previousPiece, previous, unit, position);
                            previous = BoardPosition.OFF_BOARD;
                            highlightCube.SetActive(false);
                            ClearValid();
                        }
                    }
                }
            }
            else if (selectable && !position.Equals(BoardPosition.OFF_BOARD) && !previous.Equals(BoardPosition.OFF_BOARD))
            {
                //We've selected a place not off the board, the previous position was not off the board either, but there is no piece occupying the space
                if(validPositions.Contains(position))
                {
                    //Move the piece
                    UnitManager.MovePiece(previous, position);
                    ClearValid();
                    highlightCube.SetActive(false);
                    action = true;
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            //Just clear selection
            previous = BoardPosition.OFF_BOARD;
            highlightCube.SetActive(false);
            ClearValid();
        }
    }

    private void MoveHighlight(Vector3 position)
    {
        Vector3 vec = position;
        vec.z = GOLayer.HIGHLIGHT_LAYER;
        highlightCube.transform.position = vec;
        highlightCube.SetActive(true);
    }

    private void MoveHover(Vector3 position)
    {
        Vector3 vec = position;
        vec.z = GOLayer.HOVER_LAYER;
        hoverCube.transform.position = vec;
        hoverCube.SetActive(true);
    }

    private void MoveValid()
    {
        for (int i = 1; i < validPositions.Count; i++)
        {
            Transform target = BoardManager.GetTransformForPosition(validPositions[i]);
            if (!EqualityComparer<Transform>.Default.Equals(target, default(Transform)))
            {
                GameObject v = validCubes[i - 1];
                Vector3 vec = target.position;
                vec.z = GOLayer.HIGHLIGHT_LAYER;
                v.transform.position = vec;
                v.SetActive(true);
            }
        }
    }

    private void ClearValid()
    {
        validPositions.Clear();
        foreach (GameObject v in validCubes)
        {
            v.SetActive(false);
        }
    }

    private void ValidateNextPosition(BoardPosition position)
    {
        bool add = false;
        bool exists = BoardManager.PositionExists(position);
        if (exists)
        {
            UnitPiece existing = UnitManager.GetUnitPieceForPosition(position);
            if (!existing.Equals(UnitPiece.UNKNOWN))
            {
                bool owner = false;
                if (GameManager.CurrentMode.Equals(GameMode.PlayerOne))
                {
                    owner = existing.Owner.Equals(GameManager.PlayerOne);
                }
                else if (GameManager.CurrentMode.Equals(GameMode.PlayerTwo))
                {
                    owner = existing.Owner.Equals(GameManager.PlayerTwo);
                }
                if (!owner)
                {
                    //We're not the owner
                    add = true;
                }
            }
            else
            {
                // Unoccupied space
                add = true;
            }
        }
        if (add)
        {
            validPositions.Add(position);
        }
    }

    private void HandleUnitPlacementSelected(UnitPlacementSelectedEvent e)
    {
        placementRank = e.Rank;
    }

    private void HandleUnitPlaced(UnitPlacedEvent e)
    {
        //Check if the unit still can be placed. If not, we need to clear the selection.
        if (UnitManager.GetPlacementAmountForUnit(placementRank) <= 0)
        {
            placementRank = UnitRank.Unknown;
        }
    }

    private void HandleUnitBattleResult(UnitBattleResultEvent e)
    {
        action = true;
    }

    private void HandleGameModeChanged(GameModeChangedEvent e)
    {
        if (e.Current.Equals(GameMode.PlayerTransition))
        {
            highlightCube.SetActive(false);
            previous = BoardPosition.OFF_BOARD;
            action = false;
            ClearValid();
        }
    }
}
