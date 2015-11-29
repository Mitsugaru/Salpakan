using UnityEngine;
using System;
using System.Collections;
using strange.extensions.mediation.impl;

public class UnitMenuManager : View
{

    [Inject]
    public IRootContext RootContext { get; set; }

    public Transform ContentPanel;

    public GameObject MenuItem;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        foreach (UnitRank rank in Enum.GetValues(typeof(UnitRank)))
        {
            if(!rank.Equals(UnitRank.Unknown))
            {
                GameObject item = Instantiate(MenuItem);
                UnitMenuItem itemScript = item.GetComponent<UnitMenuItem>();
                if (itemScript != null)
                {
                    RootContext.Inject(itemScript);
                    itemScript.SetRank(rank);
                }
                item.transform.SetParent(ContentPanel);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
