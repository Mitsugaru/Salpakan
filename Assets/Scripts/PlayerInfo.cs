using UnityEngine;
using System.Collections.Generic;

public class PlayerInfo {

    private string name;
    public string Name
    {
        get
        {
            return name;
        }
    }

    public GameObject Holder
    {
        get;
        set;
    }

    private HashSet<UnitPiece> pieces = new HashSet<UnitPiece>();
    public ICollection<UnitPiece> Pieces
    {
        get
        {
            return pieces;
        }
    }

    public PlayerInfo(string name)
    {
        this.name = name;
    }
    
}
