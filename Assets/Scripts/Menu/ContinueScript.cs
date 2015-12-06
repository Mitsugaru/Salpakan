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

    [Inject]
    public ICoverManager CoverManager { get; set; }

    [Inject]
    public IWinManager WinManager { get; set; }

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
                    // verify that they've completed their turn
                    if (GameManager.TurnComplete)
                    {
                        // check win condition
                        if (WinManager.WinOccurred)
                        {
                            HandleWin();
                        }
                        else
                        {
                            previous = GameMode.PlayerOne;
                            TransitionPanel.SetActive(true);
                            TransitionText.text = GameManager.PlayerTwo.Name + "'s" + System.Environment.NewLine + "turn";
                            GameManager.ChangeMode(GameMode.PlayerTransition);
                        }
                    }
                    break;
                }
            case GameMode.PlayerTwo:
                {
                    // verify that they've completed their turn
                    if (GameManager.TurnComplete)
                    {
                        // check win condition
                        if (WinManager.WinOccurred)
                        {
                            HandleWin();
                        }
                        else
                        {
                            previous = GameMode.PlayerTwo;
                            TransitionPanel.SetActive(true);
                            TransitionText.text = GameManager.PlayerOne.Name + "'s" + System.Environment.NewLine + "turn";
                            GameManager.ChangeMode(GameMode.PlayerTransition);
                        }
                    }
                    break;
                }
            case GameMode.PlayerTransition:
                {
                    if (WinManager.WinOccurred)
                    {
                        HandleWin();
                    }
                    else if (previous.Equals(GameMode.PlayerOne))
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

    private void HandleWin()
    {
        TransitionPanel.SetActive(true);
        if (WinManager.PlayerWon.Equals(PlayerInfo.UNKNOWN))
        {
            TransitionText.text = "DRAW";
        }
        else
        {
            TransitionText.text = WinManager.PlayerWon.Name + System.Environment.NewLine + "won!";
        }
        CoverManager.ClearMask();
    }
}
