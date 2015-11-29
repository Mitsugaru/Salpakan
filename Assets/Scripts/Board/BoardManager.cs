using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using System;

public class BoardManager : View, IBoardManager
{

    [Inject]
    public IEventManager EventManager { get; set; }

    [Inject]
    public IGameManager GameManager { get; set; }

    public Dictionary<BoardPosition, GameObject> board = new Dictionary<BoardPosition, GameObject>();

    public Material TileMaterial;

    public Transform TileParent;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        //Generate the grid
        for (int row = 0; row < 8; row++)
        {
            for (int column = 0; column < 9; column++)
            {
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tile.transform.position = new Vector3(column - 4, row - 3.5f);
                tile.layer = LayerMask.NameToLayer("Board");
                Renderer tileRenderer = tile.GetComponent<Renderer>();
                tileRenderer.material = TileMaterial;
                if (row < 4)
                {
                    tileRenderer.material.color = Color.blue;
                }
                else
                {
                    tileRenderer.material.color = Color.red;
                }
                tile.name = "Tile-" + row + "," + column;
                tile.transform.SetParent(TileParent);
                BoardPosition position = new BoardPosition(row, column);
                board.Add(position, tile);
                EventManager.Raise(new BoardPositionCreatedEvent(position, tile));
            }
        }
    }

    public Transform GetTransformForPosition(BoardPosition position)
    {
        Transform transform = default(Transform);

        GameObject target = null;
        if(board.TryGetValue(position, out target))
        {
            transform = target.transform;
        }

        return transform;
    }

    public BoardPosition GetPositionForGO(GameObject go)
    {
        BoardPosition position = default(BoardPosition);

        foreach (KeyValuePair<BoardPosition, GameObject> tile in board)
        {
            if (go.Equals(tile.Value))
            {
                position = tile.Key;
                break;
            }
            Renderer r = tile.Value.GetComponent<Renderer>();
            if (r != null)
            {
                Bounds b = r.bounds;
                if (go.transform.position.x >= b.min.x && go.transform.position.x <= b.max.x && go.transform.position.y >= b.min.y && go.transform.position.y <= b.max.y)
                {
                    position = tile.Key;
                    break;
                }
            }
        }

        return position;
    }

    public bool PositionIsSelectable(BoardPosition position)
    {
        bool selectable = false;
        if (GameManager.CurrentMode.Equals(GameMode.PlayerOneSetup))
        {
            if (position.X < 3)
            {
                selectable = true;
            }
        }
        else if (GameManager.CurrentMode.Equals(GameMode.PlayerTwoSetup))
        {
            if (position.X > 4)
            {
                selectable = true;
            }
        }
        else
        {
            selectable = true;
        }
        return selectable;
    }
}
