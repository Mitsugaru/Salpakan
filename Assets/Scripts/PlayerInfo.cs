using UnityEngine;
using System.Collections;

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

    public PlayerInfo(string name)
    {
        this.name = name;
    }
}
