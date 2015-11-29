using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitMenuItem : MonoBehaviour
{

    [Inject]
    public IEventManager EventManager { get; set; }

    [Inject]
    public IUnitManager UnitManager { get; set; }

    [Inject]
    public IUnitImageManager UnitImageManager { get; set; }

    public Image image;

    public Text unitName;

    public Text remaining;

    private Image background;

    private static readonly Color32 NORMAL_COLOR = new Color32(255, 255, 255, 100);

    private static readonly Color32 SELECTED_COLOR = new Color32(0, 255, 33, 176);

    private static readonly Color32 DISABLED_COLOR = new Color32(40, 40, 40, 200);

    private UnitRank rank;

    private MouseOverDetect mouseOver;

    private bool selected = false;

    // Use this for initialization
    void Start()
    {
        background = GetComponent<Image>();
        mouseOver = image.gameObject.GetComponent<MouseOverDetect>();
        EventManager.AddListener<UnitPlacementSelectedEvent>(HandleUnitPlacementSelected);
        EventManager.AddListener<UnitPlacedEvent>(HandleUnitPlaced);
        EventManager.AddListener<UnitRemovedEvent>(HandleUnitRemoved);
        EventManager.AddListener<GameModeChangedEvent>(HandleGameModeChanged);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && mouseOver.IsOver)
        {
            if (UnitManager.GetPlacementAmountForUnit(rank) > 0 && !selected)
            {
                background.color = SELECTED_COLOR;
                selected = true;
                EventManager.Raise(new UnitPlacementSelectedEvent(rank));
            }
            else if (selected)
            {
                background.color = NORMAL_COLOR;
                selected = false;
                EventManager.Raise(new UnitPlacementSelectedEvent(UnitRank.Unknown));
            }
        }
    }

    public void SetRank(UnitRank rank)
    {
        this.rank = rank;
        //Initialize the values
        unitName.text = UnitUtilities.ReadableRank(rank);
        image.sprite = UnitImageManager.GetRankImage(rank);
        remaining.text = UnitManager.GetPlacementAmountForUnit(rank).ToString();
    }

    private void HandleUnitPlacementSelected(UnitPlacementSelectedEvent e)
    {
        if (!e.Rank.Equals(rank))
        {
            background.color = NORMAL_COLOR;
            selected = false;
        }
    }

    private void HandleUnitPlaced(UnitPlacedEvent e)
    {
        if (e.Unit.Rank.Equals(rank))
        {
            int amount = UnitManager.GetPlacementAmountForUnit(rank);
            remaining.text = amount.ToString();
            if (amount <= 0)
            {
                image.color = DISABLED_COLOR;
                background.color = NORMAL_COLOR;
                selected = false;
            }
        }
    }

    private void HandleUnitRemoved(UnitRemovedEvent e)
    {
        if (e.Unit.Rank.Equals(rank))
        {
            int amount = UnitManager.GetPlacementAmountForUnit(rank);
            if (amount > 0)
            {
                remaining.text = amount.ToString();
                image.color = Color.white;
            }
        }
    }

    private void HandleGameModeChanged(GameModeChangedEvent e)
    {
        //Refresh
        if (e.Current.Equals(GameMode.PlayerOneSetup) || e.Current.Equals(GameMode.PlayerTwoSetup))
        {
            int amount = UnitManager.GetPlacementAmountForUnit(rank);
            selected = false;
            if (amount > 0)
            {
                remaining.text = amount.ToString();
                image.color = Color.white;
            }
            else
            {
                image.color = DISABLED_COLOR;
            }
        }
    }
}
