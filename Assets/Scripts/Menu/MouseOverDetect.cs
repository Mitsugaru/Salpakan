using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class MouseOverDetect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isOver = false;
    public bool IsOver
    {
        get
        {
            return isOver;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;
    }
}
