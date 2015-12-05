using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;

public class SelectionManager : View, ISelectionManager
{
    [Inject]
    public IEventManager EventManager { get; set; }

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

    private BoardPosition previous;

    private GameObject highlightCube;

    private GameObject hoverCube;

    private GameObject[] validCubes = new GameObject[4];

    protected override void Start()
    {
        base.Start();

        highlightCube = Instantiate(HighlightCubePrefab);
        highlightCube.transform.SetParent(CubeParent);
        hoverCube = Instantiate(HoverCubePrefab);
        hoverCube.transform.SetParent(CubeParent);
        for(int i = 0; i < validCubes.Length; i++)
        {
            validCubes[i] = Instantiate(ValidCubePrefab);
            validCubes[i].transform.SetParent(CubeParent);
        }

        EventManager.AddListener<UnitPlacementSelectedEvent>(HandleUnitPlacementSelected);
        EventManager.AddListener<UnitPlacedEvent>(HandleUnitPlaced);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit[] hits = Physics.RaycastAll(mainCamera.ScreenPointToRay(Input.mousePosition), 50, mask);

        bool selectable = false;
        bool piece = false;
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
                    if (selectable)
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
                    piece = true;
                    unit = UnitManager.GetUnitPieceForPosition(position);
                    break;
                }
            }
        }

        //Handle mouse click
        if (Input.GetMouseButtonDown(0))
        {
            if (hits.Length == 0)
            {
                highlightCube.SetActive(false);
                EventManager.Raise(new BoardPositionSelectedEvent(BoardPosition.OFF_BOARD));
            }
            else if (selectable && !placementRank.Equals(UnitRank.Unknown))
            {
                UnitManager.AddPiece(position, placementRank);
            }
            else if (selectable && placementRank.Equals(UnitRank.Unknown))
            {
                if (piece)
                {
                    MoveHighlight(tileVector);
                    EventManager.Raise(new UnitSelectedEvent(unit));
                }
                else
                {
                    EventManager.Raise(new BoardPositionSelectedEvent(position));
                }
            }

            // Keep track of previously selected position
            if (hits.Length == 0 || selectable)
            {
                previous = position;
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            //Remove existing piece
            if(piece)
            {
                UnitManager.RemovePiece(position);
            }
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
}
