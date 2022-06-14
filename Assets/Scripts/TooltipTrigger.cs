using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    public string content;
    public string header;
    public string cost;

    public void OnPointerEnter(PointerEventData eventData){
        TooltipSystem.Show(content, header,cost);
    }

    public void OnPointerExit(PointerEventData eventData){
        TooltipSystem.Hide();
    }
}
