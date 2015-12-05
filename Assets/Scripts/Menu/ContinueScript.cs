using UnityEngine;
using System;
using System.Collections;

public class ContinueScript : MonoBehaviour
{

    [Inject]
    public IGameManager GameManager { get; set; }

    [Inject]
    public IUnitManager UnitManager { get; set; }

    // Update is called once per frame
    public void ContinueClicked()
    {
        switch (GameManager.CurrentMode)
        {
            case GameMode.PlayerOneSetup:
                {
                    bool remaining = false;
                    foreach (UnitRank rank in Enum.GetValues(typeof(UnitRank)))
                    {
                        if (!rank.Equals(UnitRank.Unknown) && UnitManager.GetPlacementAmountForUnit(rank) > 0)
                        {
                            remaining = true;
                            break;
                        }
                    }
                    if (!remaining)
                    {
                        Debug.Log("moving to player two setup");
                        GameManager.ChangeMode(GameMode.PlayerTwoSetup);
                    }
                    break;
                }
            case GameMode.PlayerTwoSetup:
                {
                    bool remaining = false;
                    foreach (UnitRank rank in Enum.GetValues(typeof(UnitRank)))
                    {
                        if (!rank.Equals(UnitRank.Unknown) && UnitManager.GetPlacementAmountForUnit(rank) > 0)
                        {
                            remaining = true;
                            break;
                        }
                    }
                    if (!remaining)
                    {
                        GameManager.ChangeMode(GameMode.PlayerOne);
                    }
                    break;
                }
            case GameMode.PlayerOne:
                {
                    //TODO verify that they've completed their turn
                    //TODO check win condition
                    GameManager.ChangeMode(GameMode.PlayerTwo);
                    break;
                }
            case GameMode.PlayerTwo:
                {
                    //TODO verify that they've completed their turn
                    //TODO check win condition
                    GameManager.ChangeMode(GameMode.PlayerOne);
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
}
