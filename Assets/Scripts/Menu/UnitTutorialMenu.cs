using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UnitTutorialMenu : MonoBehaviour
{

    [Inject]
    public IUnitImageManager UnitImageManager { get; set; }

    public Transform Parent;

    public GameObject UnitImage;

    public Sprite check;

    public Sprite redx;

    public Sprite split;

    // Use this for initialization
    void Start()
    {
        //Generate first row of defender
        foreach (UnitRank rank in Enum.GetValues(typeof(UnitRank)))
        {
            if (!rank.Equals(UnitRank.Unknown))
            {
                GameObject cell = Instantiate(UnitImage);
                cell.transform.SetParent(Parent);
                Image i = cell.GetComponent<Image>();
                i.sprite = UnitImageManager.GetRankImage(rank);
            }
        }
        //Generate the attacker rows
        foreach (UnitRank attacker in Enum.GetValues(typeof(UnitRank)))
        {
            if (!attacker.Equals(UnitRank.Unknown))
            {
                //Generate attacker cell
                GameObject cell = Instantiate(UnitImage);
                cell.transform.SetParent(Parent);
                Image i = cell.GetComponent<Image>();
                i.sprite = UnitImageManager.GetRankImage(attacker);
                //Iterate over possibles
                foreach (UnitRank rank in Enum.GetValues(typeof(UnitRank)))
                {
                    if (!rank.Equals(UnitRank.Unknown))
                    {
                        GameObject rCell = Instantiate(UnitImage);
                        rCell.transform.SetParent(Parent);
                        Image ri = rCell.GetComponent<Image>();
                        BattleResult result = UnitUtilities.ResolveBattle(attacker, rank);
                        switch (result)
                        {
                            case BattleResult.Success:
                                {
                                    ri.sprite = check;
                                    break;
                                }
                            case BattleResult.Fail:
                                {
                                    ri.sprite = redx;
                                    break;
                                }
                            case BattleResult.Split:
                                {
                                    ri.sprite = split;
                                    break;
                                }
                        }
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
