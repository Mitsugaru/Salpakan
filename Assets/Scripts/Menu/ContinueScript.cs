using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ContinueScript : MonoBehaviour
{

    [Inject]
    public IGameManager GameManager { get; set; }

    [Inject]
    public IUnitManager UnitManager { get; set; }

    public GameObject TransitionPanel;

    public Text TransitionText;

    private GameMode previous;

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
                        previous = GameMode.PlayerTwo;
                        TransitionPanel.SetActive(true);
                        TransitionText.text = GameManager.PlayerOne.Name + "'s" + System.Environment.NewLine + "turn";
                        GameManager.ChangeMode(GameMode.PlayerTransition);
                    }
                    break;
                }
            case GameMode.PlayerOne:
                {
                    //TODO verify that they've completed their turn
                    //TODO check win condition
                    previous = GameMode.PlayerOne;
                    TransitionPanel.SetActive(true);
                    TransitionText.text = GameManager.PlayerTwo.Name + "'s" + System.Environment.NewLine + "turn";
                    GameManager.ChangeMode(GameMode.PlayerTransition);
                    break;
                }
            case GameMode.PlayerTwo:
                {
                    //TODO verify that they've completed their turn
                    //TODO check win condition
                    previous = GameMode.PlayerTwo;
                    TransitionPanel.SetActive(true);
                    TransitionText.text = GameManager.PlayerOne.Name + "'s" + System.Environment.NewLine + "turn";
                    GameManager.ChangeMode(GameMode.PlayerTransition);
                    break;
                }
            case GameMode.PlayerTransition:
                {
                    if (previous.Equals(GameMode.PlayerOne))
                    {
                        GameManager.ChangeMode(GameMode.PlayerTwo);
                    }
                    else if (previous.Equals(GameMode.PlayerTwo))
                    {
                        GameManager.ChangeMode(GameMode.PlayerOne);
                    }
                    TransitionPanel.SetActive(false);
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
}
