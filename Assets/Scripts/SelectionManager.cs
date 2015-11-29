using UnityEngine;
using System.Collections;
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

    public GameObject HighlightCube;

    public GameObject HoverCube;

    public LayerMask mask;

    private UnitRank placementRank = UnitRank.Unknown;

    private BoardPosition previous;

    protected override void Start()
    {
        base.Start();

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
            HoverCube.SetActive(false);
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
                        HoverCube.SetActive(false);
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
                    EventManager.Raise(new UnitSelectedEvent(unit));
                    break;
                }
            }
        }

        //Handle mouse click
        if (Input.GetMouseButtonDown(0))
        {
            if (hits.Length == 0)
            {
                HighlightCube.SetActive(false);
                EventManager.Raise(new BoardPositionSelectedEvent(BoardPosition.OFF_BOARD));
            }
            else if (selectable && !placementRank.Equals(UnitRank.Unknown))
            {
                UnitManager.AddPiece(position, placementRank);
            }
            else if (selectable && placementRank.Equals(UnitRank.Unknown))
            {
                MoveHighlight(tileVector);
                if (piece)
                {
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
    }

    private void MoveHighlight(Vector3 position)
    {
        Vector3 vec = position;
        vec.z = -3;
        HighlightCube.transform.position = vec;
        HighlightCube.SetActive(true);
    }

    private void MoveHover(Vector3 position)
    {
        Vector3 vec = position;
        vec.z = -4;
        HoverCube.transform.position = vec;
        HoverCube.SetActive(true);
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
